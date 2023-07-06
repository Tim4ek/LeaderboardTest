using com.Common.Interfaces;
using com.Common.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.Common.Popups {
  public class OtherLeaderboardUserSlot : MonoBehaviour {
    private IPersonIconService _iconService;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _score;
    [SerializeField]
    private Image _avatar;
    [SerializeField]
    private TextMeshProUGUI _loading;
    [SerializeField]
    private TextMeshProUGUI _number;
    [SerializeField]
    private Transform _diamond;
    [SerializeField]
    private Transform _gold;
    [SerializeField]
    private Transform _silver;
    [SerializeField]
    private Transform _bronze;
    private UserModel _model;

    public void Init(IPersonIconService iconService) {
      _iconService = iconService;
    }

    public void SetView(UserModel model) {
      _model = model;
      _name.text = model.name;
      _score.text = model.score.ToString();
      _number.text = (model.Index + 1).ToString();
      SetUserTypeView();
      SetAvatar();
    }

    private void SetAvatar() {
      _loading.gameObject.SetActive(true);
      _iconService.GetPersonIcon(_model.avatar, SetPlayerIcon);
    }

    private void SetPlayerIcon(Sprite playerIcon) {              
      _loading.gameObject.SetActive(false);
      _avatar.sprite = playerIcon;
      _avatar.transform.localScale = new Vector3(100f / playerIcon.textureRect.width, 100f / playerIcon.textureRect.height, 1f);      
    }

    private void SetUserTypeView() {
      _diamond.gameObject.SetActive(_model.Type == UserType.Diamond);
      _gold.gameObject.SetActive(_model.Type == UserType.Gold);
      _silver.gameObject.SetActive(_model.Type == UserType.Silver);
      _bronze.gameObject.SetActive(_model.Type == UserType.Bronze);
    }
  }
}