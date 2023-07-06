using com.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace com.Common {
  public class NetworkService : INetworkService {
    public class NetworkManagerComponent : MonoBehaviour { }

    private readonly MonoBehaviour monoBeh;

    public NetworkService() {
      monoBeh = new GameObject("NetworkManager").AddComponent<NetworkManagerComponent>();
    }

    public NetworkService(MonoBehaviour coroutineStarter) {
      monoBeh = coroutineStarter;
    }

    public void OnLoadTexture(string urlTexture, Action<Texture2D> onLoadedSuccess, Action<string> onLoadedFail = null) {
      OnSendRequest(urlTexture, delegate (NetworkContrack sr) {
        onLoadedSuccess(sr.textureData);
      }, delegate (NetworkContrack er) {
        if (onLoadedFail != null) {
          onLoadedFail(er.errorCode);
        }
      });
    }

    private void OnSendRequest(string urlTexture, Action<NetworkContrack> onLoadedSuccess, Action<NetworkContrack> onLoadedFail) {
      UnityWebRequest texture = UnityWebRequestTexture.GetTexture(urlTexture, nonReadable: false);
      monoBeh.StartCoroutine(RequestRoutine(texture, onLoadedSuccess, onLoadedFail));
    }

    private IEnumerator RequestRoutine(UnityWebRequest textureRequest, Action<NetworkContrack> onLoadedSuccess, Action<NetworkContrack> onLoadedFail) {
      
      textureRequest.SendWebRequest();
      while (!textureRequest.isDone) {
        yield return new WaitForEndOfFrame();
      }      
      CallReceivers(textureRequest, onLoadedSuccess, onLoadedFail);
    }

    private void CallReceivers(UnityWebRequest textureRequest, Action<NetworkContrack> onLoadedSuccess, Action<NetworkContrack> onLoadedFail) {
      if (IsReceiverSuccess(textureRequest) ) {
        onLoadedSuccess(SetTexturesReceiver(textureRequest));
      } else {
        onLoadedFail(SetTextureReceiverError(textureRequest));
      }
    }

    private bool IsReceiverSuccess(UnityWebRequest textureRequest) {
      if (textureRequest.responseCode >= 200 && textureRequest.responseCode < 300) {
        return !textureRequest.isNetworkError;
      }
      return false;
    }

    private NetworkContrack SetTexturesReceiver(UnityWebRequest textureRequest) {
      if (!(textureRequest.downloadHandler is DownloadHandlerAudioClip)) {
        bool flag = textureRequest.downloadHandler is DownloadHandlerTexture;
      }
      NetworkContrack requestResponse = new NetworkContrack();      
      requestResponse.isSuccess = true;
      requestResponse.responceCode = (int) textureRequest.responseCode;
      requestResponse.urlPath = textureRequest.url;
      requestResponse.urlPath = textureRequest.url;
      if (textureRequest.downloadHandler != null) {
        requestResponse.bytesData = textureRequest.downloadHandler.data;
        requestResponse.textData = ((textureRequest.downloadHandler is DownloadHandlerAudioClip) ? null : textureRequest.downloadHandler.text);
        requestResponse.audioData = ((textureRequest.downloadHandler is DownloadHandlerAudioClip) ? ((DownloadHandlerAudioClip) textureRequest.downloadHandler).audioClip : null);
        requestResponse.textureData = ((textureRequest.downloadHandler is DownloadHandlerTexture) ? ((DownloadHandlerTexture) textureRequest.downloadHandler).texture : null);
      }
      textureRequest.Dispose();
      return requestResponse;
    }

    private NetworkContrack SetTextureReceiverError(UnityWebRequest textureRequest) {
      NetworkContrack result = new NetworkContrack {
        isSuccess = false,
        responceCode = (int) textureRequest.responseCode,
        errorCode = textureRequest.error,
        urlPath = textureRequest.url,
        textData = textureRequest.downloadHandler.text
      };
      textureRequest.Dispose();
      return result;
    }

    public void GetRequestSend(string url, Action<NetworkContrack> onSuccess, Action<NetworkContrack> onFailure) {
      UnityWebRequest webRequest = UnityWebRequest.Get(url);
      monoBeh.StartCoroutine(RequestRoutine(webRequest, onSuccess, onFailure));
    }

    public void PostRequestSend(string url, Action<NetworkContrack> onSuccess, Action<NetworkContrack> onFailure, Dictionary<string, string> postData) {
      UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, postData);
      unityWebRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
      monoBeh.StartCoroutine(RequestRoutine(unityWebRequest, onSuccess, onFailure));
    }

    public void JsonPostRequestSend(string url, Action<NetworkContrack> successCallback, Action<NetworkContrack> failCallback, object body) {
      string b = JsonConvert.SerializeObject(body);
      UnityWebRequest request = new UnityWebRequest(url);
      request.method = "POST";
      request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(b));
      request.downloadHandler = new DownloadHandlerBuffer();
      request.SetRequestHeader("Content-Type", "application/json");
      monoBeh.StartCoroutine(RequestRoutine(request, successCallback, failCallback));
    }
  }
}