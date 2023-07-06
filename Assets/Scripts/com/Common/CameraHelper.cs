using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Common {
  public class CameraHelper {
    private static Camera camera;

    public static Camera mainCamera {
      get {
        if (!(camera == null)) {
          return camera;
        }
        return GetMainCamera();
      }
    }

    private static Camera GetMainCamera() {
      camera = Camera.main;
      return camera;
    }
  }
}