using com.Common.Interfaces;
using UnityEngine;

namespace com.Common {
  public class ResourcesLoadService : IResourcesLoadService {
    public string LoadTextAssetByPath(string path) {
      TextAsset textAsset = Resources.Load<TextAsset>(path);
      if (textAsset == null) {
        Debug.LogError($"ResourcesLoadService | LoadTextAssetByPath | textAsset==null");
        return null;
      }
      return textAsset.text;
    }
  }
}
