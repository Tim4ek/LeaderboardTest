using com.Common.Interfaces;
using com.Common.Popups;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace com.Common.GameViews {
  public class LobbyView : MonoBehaviour, IBaseGameView {
    private IAppContext _appContext;
    private IPopupManagerService _popupService;

    [SerializeField]
    private Button _leaderboradButton;
    [SerializeField]
    private AnimationCurve buttonAnimCurve;

    private LeaderboardPopup _leaderboard;
    private readonly static Vector2 _resetButtonPos = new Vector2(0f, -1300f);
    public GameObject GObject => this.gameObject;

    public void Init(IAppContext appContext) {
      _appContext = appContext;
      _popupService = appContext.Resolve<IPopupManagerService>();
      AddListeners();
      Deactivate();
      ResetView();
      this.gameObject.SetActive(false);
    }

    private void AddListeners() {
      if (_leaderboradButton != null) {
        _leaderboradButton.onClick.AddListener(OpenLeaderboard);
      }
    }

    private void OpenLeaderboard() {
      OpenLeaderboardAsync();
    }

    private async Task OpenLeaderboardAsync() {
      Deactivate();
      _leaderboard = await _popupService.OpenPopup<LeaderboardPopup>("LeaderboardPopup", _appContext);
      _leaderboard.gameObject.transform.SetParent(_appContext.Resolve<ICanvasService>().GetPopupCanvas(), worldPositionStays: false);
      _leaderboard.OnClosed += OnClosedPopup;
      await _leaderboard.Show();
    }

    private void OnClosedPopup() {
      _leaderboard.OnClosed -= OnClosedPopup;
      _popupService.ClosePopup("LeaderboardPopup");
      _leaderboard = null;
      Activate();
    }

    private void Deactivate() {
      if (_leaderboradButton != null) {
        _leaderboradButton.interactable = false;
      }
    }

    private void Activate() {
      if (_leaderboradButton != null) {
        _leaderboradButton.interactable = true;
      }
    }

    private void ResetView() {
      if (_leaderboradButton != null) {
        _leaderboradButton.transform.localPosition = _resetButtonPos;
      }
    }

    public void Show() {
      this.gameObject.SetActive(true);
      ShowButton();
    }

    private void ShowButton() {
      if (_leaderboradButton != null) {
        _leaderboradButton.transform.localPosition = _resetButtonPos;

        Sequence anim = DOTween.Sequence().OnComplete((TweenCallback) delegate {
          Activate();
        });
        anim.Insert(0f, _leaderboradButton.transform.DOLocalMoveY(0, 0.7f).SetEase(buttonAnimCurve));
        anim.Play();
      }
    }

    public void DestroyObject() {
      if (_leaderboradButton != null) {
        _leaderboradButton.onClick.RemoveListener(OpenLeaderboard);
      }
    }
  }
}
