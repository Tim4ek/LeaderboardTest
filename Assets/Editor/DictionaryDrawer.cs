#if UNITY_EDITOR
using com.Common;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;


public abstract class DictionaryDrawer<TK, TV> : PropertyDrawer {
  private const float BUTTON_WIDTH = 18f;
  private const float ELEMENT_HEIGHT = 17f;

  private DictionarySerialize<TK, TV> _Dictionary;
  private bool _foldout, _foldoutFilters;
  private bool _useKeyFilter, _useValueFilter;
  private TK _keyFilter = default(TK);
  private TV _valueFilter = default(TV);

  enum DrawMode {
    NotMatch,
    PartlyMatch,
    Match
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    CheckInitialize(property, label);
    if (_foldout) {
      return (_Dictionary.Count + 2) * ELEMENT_HEIGHT + (_foldoutFilters ? 52f : ELEMENT_HEIGHT);
    }
    return ELEMENT_HEIGHT;
  }

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    GUI.changed = false;
    bool changed = false;
    CheckInitialize(property, label);
    position.height = ELEMENT_HEIGHT;

    var foldoutRect = position;
    foldoutRect.width -= 2 * BUTTON_WIDTH;
    changed = GUI.changed;
    EditorGUI.BeginChangeCheck();
    _foldout = EditorGUI.Foldout(foldoutRect, _foldout, label, true);
    if (EditorGUI.EndChangeCheck()) {
      EditorPrefs.SetBool(label.text, _foldout);
      if (!changed) {
        GUI.changed = false;
      }
    }

    if (!_foldout) {
      return;
    }
    var buttonRect = position;
    buttonRect.x = position.width - BUTTON_WIDTH + position.x - 1;
    buttonRect.width = BUTTON_WIDTH;

    if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButtonRight)) {
      AddNewItem();
      // Save(property);
    }

    buttonRect.x -= BUTTON_WIDTH + 1;

    if (GUI.Button(buttonRect, new GUIContent("x", "Clear dictionary"), EditorStyles.miniButtonLeft)) {
      ClearDictionary();
      // Save(property);
    }

    changed = GUI.changed;
    position.y += ELEMENT_HEIGHT;
    position.y += DrawFindClause(position);
    if (!changed) {
      GUI.changed = false;
    }

    foreach (var item in _Dictionary) {
      var key = item.Key;
      var value = item.Value;

      position.y += ELEMENT_HEIGHT;

      var keyRect = position;
      keyRect.width /= 2;
      keyRect.width -= 4;
      EditorGUI.BeginChangeCheck();
      var newKey = DoField(IsMatchFind(key, _keyFilter, _useKeyFilter), keyRect, typeof(TK), key);
      if (EditorGUI.EndChangeCheck()) {
        try {
          _Dictionary.Remove(key);
          _Dictionary.Add(newKey, value);
        } catch (Exception e) {
          Debug.LogError(e.Message);
        }
        break;
      }

      var valueRect = position;
      valueRect.x = position.width / 2 + 15;
      valueRect.width = keyRect.width - BUTTON_WIDTH;
      EditorGUI.BeginChangeCheck();
      Rect spriteRect = new Rect(position.width / 2 + 15, position.y, keyRect.width - BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);
      value = DoField(IsMatchFind(value, _valueFilter, _useValueFilter), valueRect, typeof(TV), value, spriteRect);
      if (EditorGUI.EndChangeCheck()) {
        _Dictionary[key] = value;
        break;
      }

      var removeRect = valueRect;
      removeRect.x = valueRect.xMax + 2;
      removeRect.width = BUTTON_WIDTH;
      if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight)) {
        RemoveItem(key);
        break;
      }
    }

    buttonRect.x = position.width - BUTTON_WIDTH + position.x - 5;
    buttonRect.y = position.y + ELEMENT_HEIGHT;
    if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButtonRight)) {
      AddNewItem();
    }

    if (GUI.changed) {
      Save(property);
      Debug.Log("Asset saved");
    }
  }

  private float DrawFindClause(Rect position) {
    position.xMin += 9f;
    _foldoutFilters = EditorGUI.Foldout(position, _foldoutFilters, "Find");
    if (!_foldoutFilters) {
      return 0f;
    }
    position.y += 15f;
    Rect keyFilterRect = position;
    keyFilterRect.width -= 54;
    keyFilterRect.x += 54;
    _keyFilter = DoField(keyFilterRect, typeof(TK), _keyFilter);
    Rect useKeyFilterRect = position;
    useKeyFilterRect.width = 50;
    _useKeyFilter = EditorGUI.ToggleLeft(useKeyFilterRect, "Key", _useKeyFilter);
    Rect valueFilterRect = keyFilterRect;
    valueFilterRect.y += 18f;
    Rect spriteRect = new Rect(valueFilterRect.x, valueFilterRect.y, position.xMax - valueFilterRect.x, EditorGUIUtility.singleLineHeight);
    _valueFilter = DoField(valueFilterRect, typeof(TV), _valueFilter, spriteRect);
    Rect useValueFilterRect = useKeyFilterRect;
    useValueFilterRect.y += 18f;
    _useValueFilter = EditorGUI.ToggleLeft(useValueFilterRect, "Value", _useValueFilter);
    return 35f;
  }

  private DrawMode IsMatchFind<T>(T value, T filter, bool isFilterActive) {
    if (!isFilterActive) {
      return DrawMode.NotMatch;
    }
    if (value.Equals(filter)) {
      return DrawMode.Match;
    }
    if ((value is string) && !string.IsNullOrWhiteSpace(filter as string)) {
      if ((value as string).ToLower().Contains((filter as string).ToLower())) {
        return DrawMode.PartlyMatch;
      }
    }
    return DrawMode.NotMatch;
  }

  private void Save(SerializedProperty property) {
    EditorUtility.SetDirty(property.serializedObject.targetObject);
    AssetDatabase.SaveAssets();
  }

  private void RemoveItem(TK key) {
    _Dictionary.Remove(key);
  }

  private void CheckInitialize(SerializedProperty property, GUIContent label) {
    if (_Dictionary == null) {
      var target = property.serializedObject.targetObject;
      _Dictionary = fieldInfo.GetValue(target) as DictionarySerialize<TK, TV>;
      if (_Dictionary == null) {
        _Dictionary = new DictionarySerialize<TK, TV>();
        fieldInfo.SetValue(target, _Dictionary);
      }
      _foldout = EditorPrefs.GetBool(label.text);
    }
  }

  private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
    new Dictionary<Type, Func<Rect, object, object>>() { { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int) value) }, { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float) value) }, { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string) value) }, { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool) value) }, { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2) value) }, { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3) value) }, { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds) value) }, { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect) value) },
    };

  private static T DoField<T>(DrawMode mode, Rect rect, Type type, T value, Rect spriteRect = default(Rect)) {
    Color bgColor = GUI.backgroundColor;
    switch (mode) {
      case DrawMode.Match:
        GUI.backgroundColor = Color.green;
        break;
      case DrawMode.PartlyMatch:
        GUI.backgroundColor = Color.yellow;
        break;
    }
    T result = DoField<T>(rect, type, value, spriteRect);
    GUI.backgroundColor = bgColor;
    return result;
  }

  private static T DoField<T>(Rect rect, Type type, T value, Rect spriteRect = default(Rect)) {
    Func<Rect, object, object> field;
    if (_Fields.TryGetValue(type, out field)) {
      return (T) field(rect, value);
    }

    if (type.IsEnum) {
      return (T) (object) EditorGUI.EnumPopup(rect, (Enum) (object) value);
    }

    AddContextMenu(rect, value);

    if (type == typeof(Sprite)) {
      return (T) (object) EditorGUI.ObjectField(spriteRect, (UnityObject) (object) value, typeof(Sprite), false);
    }

    if (typeof(UnityObject).IsAssignableFrom(type)) {
      return (T) (object) EditorGUI.ObjectField(rect, (UnityObject) (object) value, type, true);
    }

    Debug.Log("Type is not supported: " + type);
    return value;
  }

  private static void AddContextMenu(Rect rect, object value) {
    if (value == null) {
      return;
    }
    Event currentEvent = Event.current;
    if (!rect.Contains(currentEvent.mousePosition) || currentEvent.type != EventType.ContextClick) {
      return;
    }
    GenericMenu contextMenu = new GenericMenu();
    contextMenu.AddItem(new GUIContent("Select asset"), false, OnSelectAsset, value);
    contextMenu.ShowAsContext();
    currentEvent.Use();
  }

  private static void OnSelectAsset(object value) {
    Selection.activeObject = value as UnityObject;
    EditorUtility.FocusProjectWindow();
  }

  private void ClearDictionary() {
    _Dictionary.Clear();
  }

  private void AddNewItem() {
    TK key;
    if (typeof(TK) == typeof(string))
      key = (TK) (object)
    "";
    else key = default(TK);

    var value = default(TV);
    try {
      _Dictionary.Add(key, value);
    } catch (Exception e) {
      Debug.Log(e.Message);
    }
  }
}

[CustomPropertyDrawer(typeof(AssetNameMap))]
public class StringToGameObjectDictionaryDrawer : DictionaryDrawer<string, GameObject> { }
#endif