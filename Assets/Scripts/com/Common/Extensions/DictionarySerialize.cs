using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class DictionarySerialize<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable {
  private static class SimpleNumberHelper {
    public static readonly int[] SimpleNumber = new int[72] {
      3,
      7,
      11,
      17,
      23,
      29,
      37,
      47,
      59,
      71,
      89,
      107,
      131,
      163,
      197,
      239,
      293,
      353,
      431,
      521,
      631,
      761,
      919,
      1103,
      1327,
      1597,
      1931,
      2333,
      2801,
      3371,
      4049,
      4861,
      5839,
      7013,
      8419,
      10103,
      12143,
      14591,
      17519,
      21023,
      25229,
      30293,
      36353,
      43627,
      52361,
      62851,
      75431,
      90523,
      108631,
      130363,
      156437,
      187751,
      225307,
      270371,
      324449,
      389357,
      467237,
      560689,
      672827,
      807403,
      968897,
      1162687,
      1395263,
      1674319,
      2009191,
      2411033,
      2893249,
      3471899,
      4166287,
      4999559,
      5999471,
      7199369
    };

    public static bool IsSimpleNumber(int numberCandidate) {
      if ((numberCandidate & 1) != 0) {
        int numberValue = (int) Math.Sqrt(numberCandidate);
        for (int i = 3; i <= numberValue; i += 2) {
          if (numberCandidate % i == 0) {
            return false;
          }
        }
        return true;
      }
      return numberCandidate == 2;
    }

    public static int GetSimpleNumber(int minNumber) {
      if (minNumber < 0) {
        throw new ArgumentException("min < 0");
      }
      for (int i = 0; i < SimpleNumber.Length; i++) {
        int numberValue = SimpleNumber[i];
        if (numberValue >= minNumber) {
          return numberValue;
        }
      }
      for (int j = minNumber | 1; j < int.MaxValue; j += 2) {
        if (IsSimpleNumber(j) && (j - 1) % 101 != 0) {
          return j;
        }
      }
      return minNumber;
    }

    public static int ExpandSimpleNumber(int numberOldSize) {
      int numberValue = 2 * numberOldSize;
      if (numberValue > 2146435069 && 2146435069 > numberOldSize) {
        return 2146435069;
      }
      return GetSimpleNumber(numberValue);
    }
  }

  public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable {
    private readonly DictionarySerialize<TKey, TValue> m_dictionary;

    private int m_index;

    private KeyValuePair<TKey, TValue> m_current;

    public KeyValuePair<TKey, TValue> Current => m_current;

    object IEnumerator.Current => Current;

    public Enumerator(DictionarySerialize<TKey, TValue> serialize) {
      m_dictionary = serialize;
      m_current = default(KeyValuePair<TKey, TValue>);
      m_index = 0;
    }

    public bool MoveNext() {
      while (m_index < m_dictionary.m_count) {
        if (m_dictionary.m_hashCodes[m_index] >= 0) {
          m_current = new KeyValuePair<TKey, TValue>(m_dictionary.m_keys[m_index], m_dictionary.m_values[m_index]);
          m_index++;
          return true;
        }
        m_index++;
      }
      m_index = m_dictionary.m_count + 1;
      m_current = default(KeyValuePair<TKey, TValue>);
      return false;
    }

    void IEnumerator.Reset() {
      m_index = 0;
      m_current = default(KeyValuePair<TKey, TValue>);
    }

    public void Dispose() {}
  }

  [SerializeField, HideInInspector]

  private int[] m_buckets;

  [SerializeField, HideInInspector]

  private int[] m_hashCodes;

  [SerializeField, HideInInspector]

  private int[] m_next;

  [SerializeField, HideInInspector]

  private int m_count;

  [SerializeField, HideInInspector]

  private int m_freeList;

  [SerializeField, HideInInspector]

  private int m_freeCount;

  [SerializeField, HideInInspector]

  private TKey[] m_keys;

  [SerializeField, HideInInspector]

  private TValue[] m_values;

  private readonly IEqualityComparer<TKey> m_comparer;

  public Dictionary<TKey, TValue> AsDictionary => new Dictionary<TKey, TValue>(this);

  public int Count => m_count - m_freeCount;

  public TValue this[TKey k, TValue defaultV] {
    get {
      int num = FindKeyIndex(k);
      if (num >= 0) {
        return m_values[num];
      }
      return defaultV;
    }
  }

  public TValue this[TKey k] {
    get {
      int num = FindKeyIndex(k);
      if (num >= 0) {
        return m_values[num];
      }
      throw new KeyNotFoundException(k.ToString());
    }
    set {
      PopKey(k, value, isAdd : false);
    }
  }

  public ICollection<TKey> Keys => m_keys.Take(Count).ToArray();

  public ICollection<TValue> Values => m_values.Take(Count).ToArray();

  public bool IsReadOnly => false;

  public DictionarySerialize() : this(0, (IEqualityComparer<TKey>) null) {}

  public DictionarySerialize(int capacity) : this(capacity, (IEqualityComparer<TKey>) null) {}

  public DictionarySerialize(IEqualityComparer<TKey> comparer) : this(0, comparer) {}

  public DictionarySerialize(int cap, IEqualityComparer<TKey> comp) {
    if (cap < 0) {
      throw new ArgumentOutOfRangeException("capacity");
    }
    StartInit(cap);
    m_comparer = (comp ?? EqualityComparer<TKey>.Default);
  }

  public DictionarySerialize(IDictionary<TKey, TValue> serialize) : this(serialize, (IEqualityComparer<TKey>) null) {}

  public DictionarySerialize(IDictionary<TKey, TValue> serialize, IEqualityComparer<TKey> comp) : this((serialize != null) ? serialize.Count : 0, comp) {
    if (serialize == null) {
      throw new ArgumentNullException("dictionary");
    }
    foreach (KeyValuePair<TKey, TValue> item in serialize) {
      Add(item.Key, item.Value);
    }
  }

  public bool ValueContains(TValue tValue) {
    if (tValue == null) {
      for (int i = 0; i < m_count; i++) {
        if (m_hashCodes[i] >= 0 && m_values[i] == null) {
          return true;
        }
      }
    } else {
      EqualityComparer<TValue> m_default = EqualityComparer<TValue>.Default;
      for (int j = 0; j < m_count; j++) {
        if (m_hashCodes[j] >= 0 && m_default.Equals(m_values[j], tValue)) {
          return true;
        }
      }
    }
    return false;
  }

  public bool ContainsKey(TKey k) {
    return FindKeyIndex(k) >= 0;
  }

  public void Clear() {
    if (m_count > 0) {
      for (int i = 0; i < m_buckets.Length; i++) {
        m_buckets[i] = -1;
      }
      Array.Clear(m_keys, 0, m_count);
      Array.Clear(m_values, 0, m_count);
      Array.Clear(m_hashCodes, 0, m_count);
      Array.Clear(m_next, 0, m_count);
      m_freeList = -1;
      m_count = 0;
      m_freeCount = 0;
    }
  }

  public void Add(TKey key, TValue value) {
    PopKey(key, value, isAdd : true);
  }

  private void RebuildSize(int size, bool isForceNewHashCodes) {
    int[] array = new int[size];
    for (int i = 0; i < array.Length; i++) {
      array[i] = -1;
    }
    TKey[] array2 = new TKey[size];
    TValue[] array3 = new TValue[size];
    int[] array4 = new int[size];
    int[] array5 = new int[size];
    Array.Copy(m_values, 0, array3, 0, m_count);
    Array.Copy(m_keys, 0, array2, 0, m_count);
    Array.Copy(m_hashCodes, 0, array4, 0, m_count);
    Array.Copy(m_next, 0, array5, 0, m_count);
    if (isForceNewHashCodes) {
      for (int j = 0; j < m_count; j++) {
        if (array4[j] != -1) {
          array4[j] = (m_comparer.GetHashCode(array2[j]) & int.MaxValue);
        }
      }
    }
    for (int k = 0; k < m_count; k++) {
      int num = array4[k] % size;
      array5[k] = array[num];
      array[num] = k;
    }
    m_buckets = array;
    m_keys = array2;
    m_values = array3;
    m_hashCodes = array4;
    m_next = array5;
  }

  private void RebuildSize() {
    RebuildSize(SimpleNumberHelper.ExpandSimpleNumber(m_count), isForceNewHashCodes : false);
  }

  public bool Remove(TKey k) {
    if (k == null) {
      throw new ArgumentNullException("key");
    }
    int num = m_comparer.GetHashCode(k) & int.MaxValue;
    int num2 = num % m_buckets.Length;
    int num3 = -1;
    for (int num4 = m_buckets[num2]; num4 >= 0; num4 = m_next[num4]) {
      if (m_hashCodes[num4] == num && m_comparer.Equals(m_keys[num4], k)) {
        if (num3 < 0) {
          m_buckets[num2] = m_next[num4];
        } else {
          m_next[num3] = m_next[num4];
        }
        m_hashCodes[num4] = -1;
        m_next[num4] = m_freeList;
        m_keys[num4] = default(TKey);
        m_values[num4] = default(TValue);
        m_freeList = num4;
        m_freeCount++;
        return true;
      }
      num3 = num4;
    }
    return false;
  }

  private void PopKey(TKey k, TValue tv, bool isAdd) {
    if (k == null) {
      throw new ArgumentNullException("key");
    }
    if (m_buckets == null) {
      StartInit(0);
    }
    int num = m_comparer.GetHashCode(k) & int.MaxValue;
    int num2 = num % m_buckets.Length;
    int num3 = 0;
    for (int num4 = m_buckets[num2]; num4 >= 0; num4 = m_next[num4]) {
      if (m_hashCodes[num4] == num && m_comparer.Equals(m_keys[num4], k)) {
        if (isAdd) {
          throw new ArgumentException("Key already exists: " + k);
        }
        m_values[num4] = tv;
        return;
      }
      num3++;
    }
    int num5;
    if (m_freeCount > 0) {
      num5 = m_freeList;
      m_freeList = m_next[num5];
      m_freeCount--;
    } else {
      if (m_count == m_keys.Length) {
        RebuildSize();
        num2 = num % m_buckets.Length;
      }
      num5 = m_count;
      m_count++;
    }
    m_hashCodes[num5] = num;
    m_next[num5] = m_buckets[num2];
    m_keys[num5] = k;
    m_values[num5] = tv;
    m_buckets[num2] = num5;
  }

  private void StartInit(int cap) {
    int prime = SimpleNumberHelper.GetSimpleNumber(cap);
    m_buckets = new int[prime];
    for (int i = 0; i < m_buckets.Length; i++) {
      m_buckets[i] = -1;
    }
    m_keys = new TKey[prime];
    m_values = new TValue[prime];
    m_hashCodes = new int[prime];
    m_next = new int[prime];
    m_freeList = -1;
  }

  private int FindKeyIndex(TKey k) {
    if (k == null) {
      throw new ArgumentNullException("key");
    }
    if (m_buckets != null) {
      int num = m_comparer.GetHashCode(k) & int.MaxValue;
      for (int num2 = m_buckets[num % m_buckets.Length]; num2 >= 0; num2 = m_next[num2]) {
        if (m_hashCodes[num2] == num && m_comparer.Equals(m_keys[num2], k)) {
          return num2;
        }
      }
    }
    return -1;
  }

  public bool TryGetValue(TKey k, out TValue tv) {
    int num = FindKeyIndex(k);
    if (num >= 0) {
      tv = m_values[num];
      return true;
    }
    tv = default(TValue);
    return false;
  }

  public void Add(KeyValuePair<TKey, TValue> pair) {
    Add(pair.Key, pair.Value);
  }

  public bool Contains(KeyValuePair<TKey, TValue> pair) {
    int num = FindKeyIndex(pair.Key);
    if (num >= 0) {
      return EqualityComparer<TValue>.Default.Equals(m_values[num], pair.Value);
    }
    return false;
  }

  public void CopyTo(KeyValuePair<TKey, TValue>[] pair, int value) {
    if (pair == null) {
      throw new ArgumentNullException("array");
    }
    if (value < 0 || value > pair.Length) {
      throw new ArgumentOutOfRangeException($"index = {value} array.Length = {pair.Length}");
    }
    if (pair.Length - value < Count) {
      throw new ArgumentException($"The number of elements in the dictionary ({Count}) is greater than the available space from index to the end of the destination array {pair.Length}.");
    }
    for (int i = 0; i < m_count; i++) {
      if (m_hashCodes[i] >= 0) {
        pair[value++] = new KeyValuePair<TKey, TValue>(m_keys[i], m_values[i]);
      }
    }
  }

  public bool Remove(KeyValuePair<TKey, TValue> pair) {
    return Remove(pair.Key);
  }

  public Enumerator GetEnumerator() {
    return new Enumerator(this);
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
    return GetEnumerator();
  }
}