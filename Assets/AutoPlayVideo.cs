using UnityEngine;
using UnityEngine.Video;

public class AutoPlayVideo : MonoBehaviour
{
    [Header("Video Settings")]
    public string videoURL = "http://your.jellyfin.server/path/to/video.mp4";

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("No VideoPlayer component found on " + gameObject.name);
            return;
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoURL;
        videoPlayer.isLooping = false;
        videoPlayer.Play();
    }
}
