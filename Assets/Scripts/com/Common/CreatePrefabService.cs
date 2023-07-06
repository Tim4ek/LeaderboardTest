using com.Common.Interfaces;
using com.Common.ScriptableObjects;
using System.Threading.Tasks;
using UnityEngine;

namespace com.Common {
  public class CreatePrefabService : ICreatePrefabService {
    public readonly AssetsPack _assetsPack;

    public CreatePrefabService(AssetsPack assetsPack) {      
      _assetsPack = assetsPack;
    }

    public GameObject CreatePrefabByPath(string prefabPath) {
      GameObject value = null;
      if (_assetsPack.assetNameMap.TryGetValue(prefabPath, out value)) {
        return UnityEngine.Object.Instantiate(value);
      }
      throw new UnityException("No such reference: " + prefabPath);
    }
  }
}
