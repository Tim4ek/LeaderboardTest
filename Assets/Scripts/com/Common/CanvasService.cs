using com.Common.Interfaces;
using UnityEngine;

namespace com.Common {
  public class CanvasService : ICanvasService {
    private readonly Transform _go;

    private readonly ICreatePrefabService _createPrefabService;

    private Transform _lobbyCanvas;

    private Transform _popupCanvas;

    public CanvasService(Transform go, ICreatePrefabService createPrefabService) {
      _go = go;
      _createPrefabService = createPrefabService;
    }

    public Transform GetLobbyCanvas() {
      if (_lobbyCanvas == null) {
        _lobbyCanvas = _createPrefabService.CreatePrefabByPath("CanvasAsset").transform;
        _lobbyCanvas.name = "LobbyCanvas";
        Canvas component = _lobbyCanvas.GetComponent<Canvas>();
        component.renderMode = RenderMode.ScreenSpaceCamera;
        component.worldCamera = CameraHelper.mainCamera;
        component.sortingOrder = 1;
        component.sortingLayerID = SortLayerHelper.DefaultSortingName;
        _lobbyCanvas.SetParent(_go, worldPositionStays: false);
        if (_popupCanvas != null) {
          _lobbyCanvas.SetSiblingIndex(_popupCanvas.GetSiblingIndex());
        }
      }
      return _lobbyCanvas;
    }

    public Transform GetPopupCanvas() {
      if (_popupCanvas == null) {
        _popupCanvas = _createPrefabService.CreatePrefabByPath("CanvasAsset").transform;
        _popupCanvas.name = "PopupCanvas";
        Canvas component = _popupCanvas.GetComponent<Canvas>();
        component.renderMode = RenderMode.ScreenSpaceCamera;
        component.worldCamera = CameraHelper.mainCamera;
        component.sortingOrder = 2;
        component.sortingLayerID = SortLayerHelper.PopupSortingName;
        _popupCanvas.SetParent(_go, worldPositionStays: false);
        _popupCanvas.SetAsLastSibling();
      }
      return _popupCanvas;
    }
  }
}