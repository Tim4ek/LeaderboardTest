using UnityEngine;

namespace com.Common.ScriptableObjects {
  [CreateAssetMenu(fileName = "AssetsPack", menuName = "Custom/AssetsPack", order = 1)]
  public class AssetsPack : ScriptableObject {
    [Header("PREFABS")]
    public AssetNameMap assetNameMap = new AssetNameMap();
  }
}