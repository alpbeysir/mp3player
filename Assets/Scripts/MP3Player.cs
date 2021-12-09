using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using NLayer;
using System;


[RequireComponent(typeof(AudioSource))]
public class MP3Player : MonoBehaviour
{
    [SerializeField] private Slider seekBar;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private MaterialIcon playPauseIcon;
    [SerializeField] private AudioSource source;

    private MpegFile mpgFile;
    private float duration;
    private bool seeking;

    private bool initialized;

    public void Init(string mp3Path)
    {
        var memStream = new MemoryStream();
        var fileStream = File.Open(mp3Path, FileMode.Open);
        fileStream.CopyTo(memStream);
        fileStream.Close();

        mpgFile = new MpegFile(memStream);
        duration = (float)mpgFile.Duration.TotalSeconds;
        GetComponent<AudioSource>().clip = AudioClip.Create("loaded", (int)mpgFile.Length, mpgFile.Channels, mpgFile.SampleRate, true, delegate (float[] data) { mpgFile.ReadSamples(data, 0, data.Length); });

        seekBar.maxValue = duration;
        source.Play();

        if (!source.isPlaying)
        {
            PlayPauseButtonDown();
        }

        initialized = true;
    }
    private void Update()
    {
        if (!initialized)
        {
            return;
        }

        if (seeking)
        {
            mpgFile.Time = TimeSpan.FromSeconds(seekBar.value);
            GetComponent<AudioSource>().timeSamples = (int)mpgFile.Position;
            seeking = false;
            return;
        }
        seekBar.SetValueWithoutNotify((float)mpgFile.Time.TotalSeconds);

        int time = (int)(float)mpgFile.Time.TotalSeconds;
        string minutes = (time / 60).ToString();
        if (minutes.Length == 1) minutes = "0" + minutes;
        string seconds = (time % 60).ToString();
        if (seconds.Length == 1) seconds = "0" + seconds;
        timeText.text = minutes + ":" + seconds;

        if (mpgFile.Time.TotalSeconds > duration - 0.15f)
        {
            //End-of-file
            GetComponent<FileManager>().Forward();
        }
    }
    public void PlayPauseButtonDown()
    {
        if (source.isPlaying)
        {
            source.Pause();
            playPauseIcon.iconUnicode = "e037";
        }
        else
        {
            source.UnPause();
            playPauseIcon.iconUnicode = "e034";
        } 
    }
    public void SeekBarClicked()
    {
        seeking = true;
    }
}
