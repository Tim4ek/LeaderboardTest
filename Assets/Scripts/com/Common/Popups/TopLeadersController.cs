using com.Common.Interfaces;
using com.Common.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.Common.Popups {
  public class TopLeadersController : MonoBehaviour {
    [SerializeField]
    private List<TopLeaderSlot> topLeader;

    public void Init(IPersonIconService iconService) {
      if (topLeader == null || topLeader.Count == 0) {
        return;
      }
      for (int i = 0; i < topLeader.Count; i++) {
        topLeader[i].Init(iconService);
      }
    }

    public void SetLeaders(UserModel model) {
      if (topLeader == null || topLeader.Count == 0) {
        return;
      }
      for (int i = 0; i < topLeader.Count; i++) {
        if (model.Type == UserType.Diamond && topLeader[i].Type == UserType.Diamond) {
          topLeader[i].SetView(model);
        }
        if (model.Type == UserType.Gold && topLeader[i].Type == UserType.Gold) {
          topLeader[i].SetView(model);
        }
        if (model.Type == UserType.Silver && topLeader[i].Type == UserType.Silver) {
          topLeader[i].SetView(model);
        }
      }
    }
  }
}