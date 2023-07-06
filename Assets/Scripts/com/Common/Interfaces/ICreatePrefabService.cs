using System.Threading.Tasks;
using UnityEngine;

namespace com.Common.Interfaces {
  public interface ICreatePrefabService {
    GameObject CreatePrefabByPath(string prefabPath);
  }
}
