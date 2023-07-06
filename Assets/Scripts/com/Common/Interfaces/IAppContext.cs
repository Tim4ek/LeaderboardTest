using UnityEngine;

namespace com.Common.Interfaces {
  public interface IAppContext {
    Transform GameContainer {
      get;
    }
    T Resolve<T>();
  }
}