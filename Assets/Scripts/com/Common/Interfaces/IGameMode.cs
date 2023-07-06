using System;

namespace com.Common.Interfaces {
  public interface IGameMode {
    event Action Finished;

    void Activate();
  }
}