using com.Common;
using com.Common.Interfaces;
using com.Common.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com {
  public class AppContext : MonoBehaviour, IAppContext {
    [SerializeField] 
    private Transform _gameContainer;
    [SerializeField]
    private AssetsPack _assetsPack;

    
    public Transform GameContainer=> _gameContainer;

    private Dictionary<Type, object> _registeredTypes;

    public void Construct() {
      _registeredTypes = new Dictionary<Type, object>();
      RegisterInstance<INetworkService>(GetNetworkService());
      RegisterInstance<IResourcesLoadService>(GetResourcesLoadService());
      RegisterInstance<ICreatePrefabService>(GetCreatePrefabService());
      RegisterInstance<ICanvasService>(GetCanvasService());
      RegisterInstance<IGameViewService>(GetGameViewService());
      RegisterInstance<IPopupManagerService>(GetPopupManagerService());
      RegisterInstance<IPersonIconService>(GetPersonIconService());
    }


    public T Resolve<T>() {
      return (T) _registeredTypes[typeof(T)];
    }

    private INetworkService GetNetworkService() {
      return new NetworkService();
    }

    private IPersonIconService GetPersonIconService() {
      return new PersonIconService(this);
    }

    private IPopupManagerService GetPopupManagerService() {
      return new PopupManagerServiceService();
    }

    private IResourcesLoadService GetResourcesLoadService() {
      return new ResourcesLoadService();
    }

    private ICreatePrefabService GetCreatePrefabService() {
      return new CreatePrefabService(_assetsPack);
    }

    private ICanvasService GetCanvasService() {
      return new CanvasService(_gameContainer, Resolve<ICreatePrefabService>());
    }

    private IGameViewService GetGameViewService() {
      return new GameViewService(this);
    }

    private void RegisterInstance<T>(T instance) {
      _registeredTypes.Add(typeof(T), instance);
    }

    private void RegisterInstance<T1, T2>(object instance) {
      _registeredTypes.Add(typeof(T1), instance);
      _registeredTypes.Add(typeof(T2), instance);
    }
  }
}