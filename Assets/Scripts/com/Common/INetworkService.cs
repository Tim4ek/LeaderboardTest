using com.Common.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.Common {
  public interface INetworkService {
	void OnLoadTexture(string urlTexture, Action<Texture2D> onLoadedSuccess, Action<string> onLoadedFail = null);
	void GetRequestSend(string url, Action<NetworkContrack> onSuccess, Action<NetworkContrack> onFailure);

	void PostRequestSend(string url, Action<NetworkContrack> onSuccess, Action<NetworkContrack> onFailure, Dictionary<string, string> postData);

	void JsonPostRequestSend(string url, Action<NetworkContrack> onSuccess, Action<NetworkContrack> onFailure, object json);
  }
}