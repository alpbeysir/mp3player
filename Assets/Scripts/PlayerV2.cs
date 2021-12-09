using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAudio.Wave;
using System.Threading;
using TMPro;

public class PlayerV2 : MonoBehaviour
{
    [SerializeField] private Slider seekBar;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private MaterialIcon playPauseIcon;

    private AudioFileReader audioFile;
    private WaveOutEvent outputDevice;
    private Thread playerThread;

    public int currentSongIndex;

    private Thread mainThread;
    private FileManager fileManager;
    private void Start()
    {
        fileManager = GetComponent<FileManager>();
        mainThread = Thread.CurrentThread;
    }
    public void Init(int index)
    {
        //Stop previous playback
        if (playerThread != null)
        {
            if (mainThread.Equals(Thread.CurrentThread))
                seekBar.value = 0;
            outputDevice.Stop();
        }

        //Initialize playback object
        audioFile = new AudioFileReader(fileManager.songs[index]);
        outputDevice = new WaveOutEvent();
        outputDevice.Init(audioFile);

        if (mainThread.Equals(Thread.CurrentThread))
        {
            seekBar.maxValue = audioFile.Length;
            //If pause icon is visible, set to play icon
            if (playPauseIcon.iconUnicode == "e037")
            {
                playPauseIcon.iconUnicode = "e034";
            }
        }

        //Start player thread
        ThreadStart playerThreadStart = new ThreadStart(delegate { Play(); });
        playerThread = new Thread(playerThreadStart);
        playerThread.Start();

        currentSongIndex = index;
    }
    private void Update()
    {
        if (outputDevice != null)
        {
            seekBar.SetValueWithoutNotify(audioFile.Position);
            if (audioFile.Length == audioFile.Position)
            {
                fileManager.Forward();
            }

            timeText.text = CalcTime((int)audioFile.CurrentTime.TotalSeconds);
        }
    }

    private void Play()
    {   
        outputDevice.Play();
        while (outputDevice.PlaybackState != PlaybackState.Stopped)
        {
            Thread.Sleep(1);
        }
        if (audioFile.Length == audioFile.Position)
        Init(currentSongIndex + 1);
    }

    public void PlayPauseButtonDown()
    {
        if (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            outputDevice.Pause();
            playPauseIcon.iconUnicode = "e037";
        }
        else
        {
            outputDevice.Play();
            playPauseIcon.iconUnicode = "e034";
        }
    }

    public void SeekBarClicked()
    {
        if (audioFile != null)
        {
            audioFile.Position = (long)seekBar.value;
        }
    }

    private string CalcTime(int time)
    {
        string minutes = (time / 60).ToString();
        if (minutes.Length == 1) minutes = "0" + minutes;
        string seconds = (time % 60).ToString();
        if (seconds.Length == 1) seconds = "0" + seconds;
        return minutes + ":" + seconds;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            seekBar.maxValue = audioFile.Length;
        }
    }
}
