using UnityEngine;

namespace com.Common.Models {
  public class NetworkContrack {
    public bool isSuccess;

    public int responceCode;

    public string errorCode;

    public byte[] bytesData;

    public string textData;

    public long waitTime;

    public AudioClip audioData;

    public Texture2D textureData;

    public string urlPath;
  }
}
