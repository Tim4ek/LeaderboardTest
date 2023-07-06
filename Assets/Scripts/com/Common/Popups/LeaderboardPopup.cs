using com.Common.GameViews;
using com.Common.Interfaces;
using com.Common.Models;
using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace com.Common.Popups {
  public class LeaderboardPopup : MonoBehaviour, IPopupInitialization, IDestroy {
    private IAppContext _appContext;
    private IResourcesLoadService _resourcesLoadService;
    private const string _usersPath = "Leaderboard";
    [SerializeField]
    private AnimationCurve _popupAnimCurve;
    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private TopLeadersController _topLeaders;
    [SerializeField]
    private Transform _scrollContainer;

    private UserModel _diamond;
    private UserModel _gold;
    private UserModel _silver;

    private List<OtherLeaderboardUserSlot> _otherSlots = new List<OtherLeaderboardUserSlot>();

    public event Action OnClosed;

    private readonly static Vector2 _resetPosition = new Vector2(0f, -2000f);
    public async Task Init(object param) {
      if (param != null) {
        _appContext = (IAppContext) param;
      }

      UpdateSlots();

      AddListeners();
      ResetAnimation();
      Deactivate();
      await Task.Yield();
    }

    private void UpdateSlots() {
      _resourcesLoadService = _appContext.Resolve<IResourcesLoadService>();
      string userData = _resourcesLoadService.LoadTextAssetByPath(_usersPath);
      UsersLeaderboardModels userModels = JsonConvert.DeserializeObject<UsersLeaderboardModels>(userData);
      if (userModels == null || userModels.leaderboard == null || userModels.leaderboard.Count == 0) {
        return;
      }
      UpdateIndex(userModels.leaderboard);
      GetTopUsers(userModels.leaderboard);
      SetTopUsers();
      SetOtherUsers(userModels.leaderboard);
    }

    private void UpdateIndex(Collection<UserModel> models) {
      for (int i = 0; i < models.Count; i++) {
        models[i].Index = i;
      }
    }

    private void SetTopUsers() {
      if (_topLeaders != null) {
        _topLeaders.Init(_appContext.Resolve<IPersonIconService>());
        _topLeaders.SetLeaders(_diamond);
        _topLeaders.SetLeaders(_gold);
        _topLeaders.SetLeaders(_silver);
      }
    }

    private void GetTopUsers(Collection<UserModel> models) {
      for (int i = 0; i < models.Count; i++) {
        if (models[i].Type == UserType.Diamond) {
          if (_diamond == null) {
            _diamond = models[i];
          } else if (_diamond.score < models[i].score) {
            _diamond = models[i];
          }
        } else if (models[i].Type == UserType.Gold) {
          if (_gold == null) {
            _gold = models[i];
          } else if (_gold.score < models[i].score) {
            _gold = models[i];
          }
        } else if (models[i].Type == UserType.Silver) {
          if (_silver == null) {
            _silver = models[i];
          } else if (_silver.score < models[i].score) {
            _silver = models[i];
          }
        }
      }
    }

    private async Task SetOtherUsers(Collection<UserModel> models) {
      for (int i = 0; i < models.Count; i++) {
        if (models[i].Index == _diamond.Index || models[i].Index == _gold.Index || models[i].Index == _silver.Index) {
          continue;
        } else {
          OtherLeaderboardUserSlotProvider slot = new OtherLeaderboardUserSlotProvider();
          OtherLeaderboardUserSlot otherSlot = await slot.Load();
          otherSlot.Init(_appContext.Resolve<IPersonIconService>());
          otherSlot.SetView(models[i]);
          otherSlot.gameObject.transform.SetParent(_scrollContainer, false);
          _otherSlots.Add(otherSlot);
        }
      }
    }

    private void AddListeners() {
      if (_closeButton != null) {
        _closeButton.onClick.AddListener(ClosePopup);
      }
    }

    private void ClosePopup() {
      ClosePopupAsync();
    }

    private async Task ClosePopupAsync() {
      bool isFinish = false;
      OnTweenHideAsync(delegate {
        isFinish = true;
      });
      while (!isFinish) {
        await Task.Yield();
      }
    }

    private void Activate() {
      if (_closeButton != null) {
        _closeButton.interactable = true;
      }
    }

    private void Deactivate() {
      if (_closeButton != null) {
        _closeButton.interactable = false;
      }
    }

    private void ResetAnimation() {
      this.gameObject.transform.localPosition = _resetPosition;
    }

    public async Task Show() {
      bool isFinish = false;
      OnTweenShowAsync(delegate {
        isFinish = true;
      });
      while (!isFinish) {
        await Task.Yield();
      }
    }

    public void OnTweenShowAsync(Action onFinish) {
      DOTween.Kill(GetInstanceID());
      this.gameObject.transform.localPosition = _resetPosition;
      Sequence anim = DOTween.Sequence().OnComplete((TweenCallback) delegate {
        if (onFinish != null) {
          onFinish();
        }
        Activate();
      }); ;
      anim.SetId(GetInstanceID());
      anim.Insert(0f, this.gameObject.transform.DOLocalMoveY(0, 0.7f).SetEase(_popupAnimCurve));
      anim.Play();
    }

    public void OnTweenHideAsync(Action onFinish) {
      DOTween.Kill(GetInstanceID());
      this.gameObject.transform.localPosition = Vector3.zero;
      Sequence anim = DOTween.Sequence().OnComplete((TweenCallback) delegate {
        if (OnClosed != null) {
          OnClosed.Invoke();
        }
        if (onFinish != null) {
          onFinish();
        }

      }); ;
      anim.SetId(GetInstanceID());
      anim.Insert(0f, this.gameObject.transform.DOLocalMoveY(-2000f, 0.7f).SetEase(Ease.InBack));
      anim.Play();
    }

    public void DestroyObject() {
      ResetAnimation();
      Deactivate();
    }
  }
}
