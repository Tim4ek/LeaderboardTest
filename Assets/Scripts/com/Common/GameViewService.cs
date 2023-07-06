using com.Common.GameViews;
using com.Common.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

namespace com.Common {
  public class GameViewService : IGameViewService {
    private readonly IAppContext _appContext;
    private readonly ICanvasService _canvasService;

    private  LobbyViewProvider _lobbyViewProvidere;

    private IBaseGameView _currentView;

    public IBaseGameView CurrentView {
      get {
        return _currentView;
      }
      private set {
        _currentView = value;
      }
    }

    public GameViewService(IAppContext appContext) {
      _appContext = appContext;
      _canvasService = appContext.Resolve<ICanvasService>();
      _lobbyViewProvidere = new LobbyViewProvider();
    }

    public async Task<IBaseGameView> ShowLobbyView() {
      LobbyView lobby = await _lobbyViewProvidere.Load();
      ShowView(lobby);
      return lobby;
    }

    private void ShowView<T>(T value) where T : IBaseGameView {      
      Show(value, _canvasService.GetLobbyCanvas());
    }

    private void Show<T>(T value, Transform transform) where T : IBaseGameView {
      OnDestroyIfNeed();
      _currentView = value;
      _currentView.GObject.transform.SetParent(transform, worldPositionStays: false);
      _currentView.GObject.transform.SetAsFirstSibling();      
    }

    private void OnDestroyIfNeed() {
      if (_currentView != null) {
        _currentView.DestroyObject();
        if (_currentView is LobbyView) {
          _lobbyViewProvidere.Unload();
        }
      }
    }
  }
}
