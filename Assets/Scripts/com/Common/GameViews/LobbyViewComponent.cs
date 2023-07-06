
using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.Common.GameViews {
  public class LobbyViewComponent : MonoBehaviour, ILobbyViewComponent {
    [SerializeField]
    private Button _leaderboardButton;

    public Button LeaderboardButton => _leaderboardButton;
  }
}
