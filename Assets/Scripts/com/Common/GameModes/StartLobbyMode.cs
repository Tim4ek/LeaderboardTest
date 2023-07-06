using com.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace com.Common.GameModes {
  public class StartLobbyMode : IGameMode, IDeactivatable {

    private readonly IAppContext _appContext;
    private readonly IGameViewService _gameViewService;

    public event Action Finished;

    public StartLobbyMode(IAppContext appContext) {
      _appContext = appContext;
      _gameViewService = appContext.Resolve<IGameViewService>();
    }

    public void Activate() {
      ShowLobbyViewAsync();
    }

    private async Task ShowLobbyViewAsync() {
      IBaseGameView view = await _gameViewService.ShowLobbyView();
      view.Init(_appContext);
      await Task.Yield();
      view.Show();
    }

    public void Deactivate() {

    }
  }
}
