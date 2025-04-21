using UnityEngine;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Chop-to-the-top-bootscreen.mov");
        videoPlayer.url = path;
        videoPlayer.Play();
    }
}
