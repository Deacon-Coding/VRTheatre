using UnityEngine;
using UnityEngine.Video;

public class VideoSelectButton : MonoBehaviour
{
    private string videoId;
    private string title;
    private string token;
    private string serverUrl;

    public void Initialize(string id, string name, string accessToken, string baseUrl)
    {
        videoId = id;
        title = name;
        token = accessToken;
        serverUrl = baseUrl;
    }

    public void OnSelect()
    {
        string videoUrl = $"{serverUrl}/Videos/{videoId}/stream?api_key={token}";

        Debug.Log($"Playing: {title} at {videoUrl}");

        var player = FindFirstObjectByType<VideoPlayer>();
        player.source = VideoSource.Url;
        player.url = videoUrl;
        player.Play();
    }
}
