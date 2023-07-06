using com.Common.Interfaces;
using System;

namespace com.Common.Extensions {
  public static class AppModeExtensions {
    public static void Deactivate(this IGameMode gameMode) {
      if (gameMode is IDeactivatable deactivatable) {
        deactivatable.Deactivate();
      }
    }

    public static void Dispose(this IGameMode gameMode) {
      if (gameMode is IDisposable disposable) {
        disposable.Dispose();
      }
    }
  }
}