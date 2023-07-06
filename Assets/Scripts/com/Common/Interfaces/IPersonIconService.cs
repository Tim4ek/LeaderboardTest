
using System;
using UnityEngine;

namespace com.Common.Interfaces {
  public interface IPersonIconService {
    void GetPersonIcon(string path, Action<Sprite> onLoadedCallback, bool save = false);
  }
}
