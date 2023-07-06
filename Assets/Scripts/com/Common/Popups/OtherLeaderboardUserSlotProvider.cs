using com.Common.Popups;
using System.Threading.Tasks;

namespace com.Common.GameViews {
  public class OtherLeaderboardUserSlotProvider : LocalAssetLoader {
    private static string prefabName = "LeaderSlotContainer";
    public Task<OtherLeaderboardUserSlot> Load() {
      return LoadInternal<OtherLeaderboardUserSlot>(prefabName);
    }

    public void Unload() {
      UnloadInternal();
    }
  }
}
