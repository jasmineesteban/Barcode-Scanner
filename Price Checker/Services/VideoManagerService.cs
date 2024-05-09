using AxWMPLib;
using System.Collections.Generic;
using System.IO;
using WMPLib;

public class VideoManagerService
{
    private System.Timers.Timer playbackTimer;
    private readonly bool isPlaying = false;
    private Queue<string> videoQueue = new Queue<string>();
    private List<string> videoFilePaths = new List<string>();
    private AxWindowsMediaPlayer mediaPlayer;


    public VideoManagerService(AxWindowsMediaPlayer player)
    {
        mediaPlayer = player;
        mediaPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(AxWindowsMediaPlayer1_PlayStateChange);
        LoadVideoFilePaths();

        videoQueue = new Queue<string>(videoFilePaths);

        playbackTimer = new System.Timers.Timer();
        playbackTimer.Interval = 11000; //11 secs
        playbackTimer.Elapsed += PlaybackTimer_Elapsed;
        playbackTimer.Start();

        PlayNextVideo();
    }

    private void LoadVideoFilePaths()
    {
        // Specify the directory where your videos are stored
        string assetsFolder = @"C:\ESTEBAN_JASMINE_PUPSMB\C#\Barcode-Scanner\PriceScannerV1\Price Checker\assets\Videos";

        // Fetch all video files in the specified directory
        videoFilePaths = new List<string>(Directory.GetFiles(assetsFolder, "*.mp4"));
    }

    public void PlayNextVideo()
    {
        if (videoQueue.Count > 0)
        {
            string videoPath = videoQueue.Dequeue();
            mediaPlayer.URL = videoPath;
            mediaPlayer.Ctlcontrols.play(); // Start playing the next video automatically
            mediaPlayer.uiMode = "none"; // Hide the controls
            mediaPlayer.stretchToFit = true; // Stretch the video to fit the player size
        }
        else
        {
            // Repopulate the queue with the video file paths
            videoQueue = new Queue<string>(videoFilePaths);

            // Play the first video in the queue
            if (videoQueue.Count > 0)
            {
                string videoPath = videoQueue.Dequeue();
                mediaPlayer.URL = videoPath;
                mediaPlayer.Ctlcontrols.play();
                mediaPlayer.uiMode = "none"; // Hide the controls
                mediaPlayer.stretchToFit = true; // Stretch the video to fit the player size
            }
        }
    }
    private void AxWindowsMediaPlayer1_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
    {
        if (e.newState == 8) // 8 represents MediaEnded state
        {
            PlayNextVideo(); // Call PlayNextVideo recursively to start the next video
        }
    }

    private void PlaybackTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (!isPlaying || mediaPlayer.playState == WMPPlayState.wmppsMediaEnded)
        {
            PlayNextVideo();
        }
    }
}