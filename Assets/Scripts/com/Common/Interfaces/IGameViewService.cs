using System.Threading.Tasks;

namespace com.Common.Interfaces {
  public interface IGameViewService {
    IBaseGameView CurrentView {
      get;
    }
    Task<IBaseGameView> ShowLobbyView();
  }
}
