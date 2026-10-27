
using UnityEngine;
using UnityEngine.Video;

public class VideoControls : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void PlayVideo()
    {
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    public void RestartVideo()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
    }
}
