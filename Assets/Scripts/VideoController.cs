using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public event Action VideoLoadComplete = null;
    public event Action VideoLoadError = null;
    public event Action<float> VideoLoadProgress = null;
    
    
    [SerializeField] private VideoPlayer videoPlayer;
    private Coroutine videoLoop = null;
    
    [HideInInspector]
    public bool isPlaying => playing;
    [SerializeField]private bool playing = false;
    
    [HideInInspector]
    public bool isPaused => paused;
    [SerializeField]private bool paused = false;
    
    private Coroutine vidLoader = null;

    private string currentURL = string.Empty;

    [SerializeField] private List<Texture> videoTextures;
    [SerializeField] private RawImage image;

    public void StartVideo()
    {
        if (currentURL == String.Empty)
            return;

        if (videoLoop != null)
            StopCoroutine(videoLoop);
        
        videoLoop = StartCoroutine(PlayVideo());
    }

    public void PauseVideo()
    {
        paused = true;
    }
    
    public void ResumeVideo()
    {
        paused = false;
    }

    public void StopVideo()
    {
        playing = false;
        paused = false;
        
        currentURL = String.Empty;
        
        if(videoLoop != null)
            StopCoroutine(videoLoop);
    }

    public void FetchURL(string videoURL)
    {
        videoPlayer.Stop();
        videoTextures.Clear();
        currentURL = videoURL;
        videoPlayer.errorReceived += OnVideoErrorReceived;
        videoPlayer.prepareCompleted += CheckVideoComplete;
        videoPlayer.frameReady += OnFrameReady;
        videoPlayer.sendFrameReadyEvents = true;
        videoPlayer.url = videoURL;
        videoPlayer.Prepare();
    }
    
    private void OnVideoErrorReceived(VideoPlayer source, string message)
    {
        videoPlayer.errorReceived -= OnVideoErrorReceived;
        videoPlayer.prepareCompleted -= CheckVideoComplete;
        videoPlayer.frameReady -= OnFrameReady;
        NFTViewer.ShowMessage("Video URL Could not be loaded, not displaying video.");
        
        currentURL = String.Empty;

        if(vidLoader != null)
            StopCoroutine(vidLoader);
        
        VideoLoadError?.Invoke();
    }
    
    private void CheckVideoComplete(VideoPlayer source)
    {
        source.Pause();
    }

    private void OnFrameReady(VideoPlayer player, long frame)
    {
        var texture = player.texture;
        var newTexture = new Texture2D(texture.width, texture.height,TextureFormat.RGBA32, false, false);
        //newTexture.ReadPixels(new Rect(0, 0, newTexture.width, newTexture.height), 0, 0, false);
        //newTexture.Apply(false);
        Graphics.CopyTexture(texture, newTexture);
        videoTextures.Add(newTexture);
        
        if (videoTextures.Count < (int) player.frameCount)
        {
            player.Play();
            player.frame = frame + 1;
            VideoLoadProgress?.Invoke((float)frame / (long) player.frameCount);
        }
        else
        {
            videoPlayer.frameReady -= OnFrameReady;
            videoPlayer.prepareCompleted -= CheckVideoComplete;
            videoPlayer.errorReceived -= OnVideoErrorReceived;
            videoPlayer.Stop();
            videoPlayer.clip = null;
            VideoLoadComplete?.Invoke();
        }
        
        
    }
    
    private IEnumerator PlayVideo()
    {
        playing = true;
        paused = false;
        
        var frameCount = videoTextures.Count;
        var nextFrame = 0;
        var waitTime = new WaitForSecondsRealtime(0.04f);
        while (playing)
        {
            if (!paused)
            {
                if (nextFrame >= (long) frameCount)
                {
                    nextFrame = 0;
                }

                image.texture = videoTextures[nextFrame];
                nextFrame++;
            }
            yield return waitTime;
        }

        videoLoop = null;
    }
}
