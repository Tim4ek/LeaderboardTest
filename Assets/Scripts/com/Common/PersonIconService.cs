using com.Common.Extensions;
using com.Common.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.Common {
  public class PersonIconService : IPersonIconService {
    private readonly IAppContext _appContext;
    private readonly INetworkService _networkService;
    private readonly Dictionary<string, Sprite> _personIcon = new Dictionary<string, Sprite>();

	public PersonIconService(IAppContext appContext) {
	  _appContext = appContext;
	  _networkService = appContext.Resolve<INetworkService>();
	}

	private void OnSaveIconIfNeed(string path, Texture2D texture2D, Action<Sprite> onLoadedCallback, bool save) {
	  if (save && PlayerPrefs.HasKey(path)) {
		PlayerPrefs.SetString(path, Convert.ToBase64String(texture2D.EncodeToJPG(100)));
	  }
	  if (_personIcon.ContainsKey(path)) {
		texture2D.UnityDestroy();
		onLoadedCallback(_personIcon[path]);
	  } else {
		Sprite sprite = InitTexture(texture2D);
		_personIcon.Add(path, sprite);
		onLoadedCallback(sprite);
	  }
	}

	private Sprite InitTexture(Texture2D texture) {
	  return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
	}

	public void GetPersonIcon(string path, Action<Sprite> onLoadedCalback, bool save = false) {
	  if (_personIcon.ContainsKey(path)) {
		onLoadedCalback(_personIcon[path]);
	  } else if (save && PlayerPrefs.HasKey(path)) {
		Sprite sprite = InitPersonIconFromSave(path);
		_personIcon.Add(path, sprite);
		onLoadedCalback(sprite);
	  } else {
		_networkService.OnLoadTexture(path, delegate (Texture2D texture)
		{
		  OnSaveIconIfNeed(path, texture, onLoadedCalback, save);
		});
	  }
	}

	private Sprite InitPersonIconFromSave(string path) {
	  Texture2D texture2D = new Texture2D(100, 100, TextureFormat.ETC_RGB4, mipChain: false);
	  texture2D.LoadImage(Convert.FromBase64String(PlayerPrefs.GetString(path)));
	  return InitTexture(texture2D);
	}
  }
}
