using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject prefab;
    [SerializeField] private new GameObject camera;
    [SerializeField] private GameObject canvas;
    [Space()]
    [SerializeField] private GameObject pathSelect;
    [SerializeField] private TMP_InputField pathInput;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color selectedColor;

    private string songDir;
    public List<string> songs;

    private int lastHighlighted;
    private PlayerV2 player;
    void Start()
    {
        Application.targetFrameRate = 60;
        player = GetComponent<PlayerV2>();

        Init();
    }
    private void Init()
    {
        songDir = PlayerPrefs.GetString("path");
        if (songDir == string.Empty || !Directory.Exists(songDir))
        {
            ShowPathSelect();
            return;
        }

        songs = new List<string>();

        foreach (string dir in Directory.GetDirectories(songDir))
        {
            songs.AddRange(Directory.GetFiles(dir));
        }
        songs.AddRange(Directory.GetFiles(songDir));

        var temp = new List<string>();
        foreach (string file in songs)
        {
            if (Path.GetExtension(file) == ".mp3")
            {
                temp.Add(file);
            }
        }
        songs = temp;

        foreach (string songPath in songs)
        {
            string songName = Path.GetFileNameWithoutExtension(songPath);
            var spawned = Instantiate(prefab, root.transform);
            spawned.GetComponentInChildren<TextMeshProUGUI>().text = songName;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(delegate { SongDisplayClicked(songs.IndexOf(songPath)); });
            spawned.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }

    private void SongDisplayClicked(int songIndex)
    {
        //Player
        HighlightSongDisplay(songIndex);
        player.Init(songIndex);
    }
    private void HighlightSongDisplay(int index)
    {
        if (GetDisplayWithIndex(player.currentSongIndex) != null)
            GetDisplayWithIndex(player.currentSongIndex).GetComponent<Image>().color = normalColor;

        GetDisplayWithIndex(index).GetComponent<Image>().color = selectedColor;
    }
    private GameObject GetDisplayWithIndex(int index)
    {
        return root.transform.GetChild(index).gameObject;
    }

    public void Back()
    {
        if (player.currentSongIndex != 0)
            SongDisplayClicked(player.currentSongIndex - 1);
        else
            SongDisplayClicked(songs.Count - 1);

    }
    public void Forward()
    {
        if (player.currentSongIndex != songs.Count - 1)
            SongDisplayClicked(player.currentSongIndex + 1);
        else 
            SongDisplayClicked(0);

    }

    public void ShowPathSelect()
    {
        pathSelect.SetActive(true);
        pathInput.text = PlayerPrefs.GetString("path");
    }
    public void HidePathSelect()
    {
        pathSelect.SetActive(false);
        if (Directory.Exists(pathInput.text))
        {
            PlayerPrefs.SetString("path", pathInput.text);
            PlayerPrefs.Save();
            Init();
        }
        else
        {
            ShowPathSelect();
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus && lastHighlighted != player.currentSongIndex)
        {
            HighlightSongDisplay(player.currentSongIndex);
        }
    }
}
