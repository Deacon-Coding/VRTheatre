using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class JellyfinLoginRequest
{
    public string Username;
    public string Pw;
}

[Serializable]
public class JellyfinLoginResponse
{
    public string AccessToken;
    public JellyfinUser User;
}

[Serializable]
public class JellyfinUser
{
    public string Id;
    public string Name;
}

public class JellyfinAuth : MonoBehaviour
{
    [Header("Jellyfin Settings")]
    public string serverUrl = "http://192.168.0.135:8096";  // Replace with your server IP
    public string username = "guest";
    public string password = "";

    [HideInInspector] public string accessToken;
    [HideInInspector] public string userId;

    void Start()
    {
        StartCoroutine(Authenticate());
    }

    IEnumerator Authenticate()
    {
        string url = $"{serverUrl}/Users/AuthenticateByName";
        var loginRequest = new JellyfinLoginRequest
        {
            Username = username,
            Pw = password
        };

        string jsonData = JsonUtility.ToJson(loginRequest);
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError($"Login failed: {request.error}");
        }
        else
        {
            var response = JsonUtility.FromJson<JellyfinLoginResponse>(request.downloadHandler.text);
            accessToken = response.AccessToken;
            userId = response.User.Id;

            Debug.Log($"Authenticated as {response.User.Name}. Access Token: {accessToken}");
        }
    }
}
