using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadAndUploadImage : MonoBehaviour
{
    Texture2D tex;
    string path;

    void Start()
    {
        StartCoroutine(DownloadImage("http://localhost:80/NewMinter/heroes1.png"));
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
            tex.Apply();
            byte[] bytes = tex.EncodeToPNG();
            path = Application.persistentDataPath + "/heroes1.png";
            System.IO.File.WriteAllBytes(path, bytes);
        }
    }
}
