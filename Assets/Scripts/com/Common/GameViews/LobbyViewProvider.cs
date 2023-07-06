
using System.Threading.Tasks;

namespace com.Common.GameViews {
  public class LobbyViewProvider : LocalAssetLoader {
    private static string prefabName = "LobbyView";
    public Task<LobbyView> Load() {
      return LoadInternal<LobbyView>(prefabName);
    }

    public void Unload() {
      UnloadInternal();
    }
  }
}
