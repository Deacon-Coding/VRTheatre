using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class JellyfinMovieBrowser : MonoBehaviour
{
    public JellyfinAuth authenticator; 
    public GameObject buttonPrefab;             // Assign a UI prefab (TextMeshPro + Collider + Gaze Button)
    public Transform buttonContainer;           // Where to place the buttons (empty GameObject)
    public string serverUrl = "http://192.168.0.135:8096";

    for (int i = 0; i<response.Items.Count; i++)
    {
        var item = response.Items[i];

        int row = i / columns;
        int col = i % columns;

        Vector3 localPos = new Vector3(col * spacingX, -row * spacingY, 0);

        GameObject btn = Instantiate(buttonPrefab, buttonContainer);
        btn.transform.localPosition = localPos;

        btn.name = item.Name;

        Text label = btn.GetComponentInChildren<Text>();
        if (label != null) label.text = item.Name;

        var clickHandler = btn.AddComponent<VideoSelectButton>();
        clickHandler.Initialize(item.Id, item.Name, authenticator.accessToken, serverUrl);
    }

void Start()
    {
        StartCoroutine(WaitForAuthAndLoadMovies());
    }

    IEnumerator WaitForAuthAndLoadMovies()
    {
        // Wait for authenticator to finish
        while (string.IsNullOrEmpty(authenticator.accessToken))
            yield return null;

        yield return StartCoroutine(LoadMovies());
    }

    IEnumerator LoadMovies()
    {
        string url = $"{serverUrl}/Users/{authenticator.userId}/Items?IncludeItemTypes=Movie&api_key={authenticator.accessToken}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            Debug.LogError("Error fetching movies: " + request.error);
            yield break;
        }

        JellyfinItemResponse response = JsonUtility.FromJson<JellyfinItemResponse>(request.downloadHandler.text);

        float yOffset = 0;
        foreach (var item in response.Items)
        {
            GameObject btn = Instantiate(buttonPrefab, buttonContainer);
            btn.transform.localPosition = new Vector3(0, -yOffset, 0);
            yOffset += 0.3f;

            btn.name = item.Name;
            btn.GetComponentInChildren<Text>().text = item.Name;

            // Add button click behavior
            var clickHandler = btn.AddComponent<VideoSelectButton>();
            clickHandler.Initialize(item.Id, item.Name, authenticator.accessToken, serverUrl);
        }
    }
}

[Serializable]
public class JellyfinItem
{
    public string Id;
    public string Name;
    public string Type;
}

[Serializable]
public class JellyfinItemResponse
{
    public List<JellyfinItem> Items;
}
