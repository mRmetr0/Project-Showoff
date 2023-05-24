using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AudioSource))]
public class MusicGrid : MonoBehaviour
{
    public static MusicGrid instance;
    [SerializeField]private AudioClip clip;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile selected;
    [SerializeField] private Tile empty;
    [SerializeField] private float bpm;
    [SerializeField] private bool loop = false;

    private AudioSource _source;
    private Dictionary<int, List<int>> _currentNotes;
    private float _bpmInSeconds = 0;
    private int _beat = -1;
    private bool _canPlay = false;

    private double _nextTime;

    private readonly float _transpose = 0;

    private float[] _betterKeys = { 0, 2, 4, 5, 7, 9, 11, 12}; //White key, includes second octave
    
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("More then one musicGrid");
        
        _source = GetComponent<AudioSource>();
        ActivateGrid(false);
    }

    private void Update()
    {
        ClickGrid();
        PlayNotes();
    }

    private void SetToPlay()
    {
        _bpmInSeconds = 60.0f / bpm;
        _nextTime = AudioSettings.dspTime;
        _canPlay = true;
        _currentNotes = GetNotes();
        _beat = -1;
        //PrintDict(_currentNotes);
    }

    private void PlayNotes()
    {
        if (Input.GetKeyDown("w")) //Debug input
        {
            SetToPlay();
        }
        
        if (_canPlay)
        {
            if (AudioSettings.dspTime >= _nextTime)
            {
                _nextTime += _bpmInSeconds;
                _beat++;

                if (_beat >= _currentNotes.Count) 
                {
                    _canPlay = loop;
                    _beat = 0;
                    if (!_canPlay) return;
                }
                
                List<int> notes = _currentNotes[_beat];
                foreach (int note in notes)
                {
                    _source.pitch = Mathf.Pow(2, (_betterKeys[note]+_transpose)/12.0f);
                    _source.PlayOneShot(clip);
                }
            }
        }
    }

    public void StartNotes()
    { 
        SetToPlay();
    }

    public void StopNotes()
    {
        _canPlay = false;
    }

    private void ClickGrid()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Tile tile = tilemap.GetTile(mousePos) as Tile;
            if (tile == null)
            {
                ActivateGrid(false);
                return;
            }
            if (tile == selected)
                tilemap.SetTile(mousePos, empty);
            else
                tilemap.SetTile(mousePos, selected);
        }
    }

    private Dictionary<int, List<int>> GetNotes()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Dictionary<int, List<int>> notes = new Dictionary<int, List<int>>();
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            List<int> beatNotes = new List<int>();
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Tile tile = tilemap.GetTile(new Vector3Int(x, y, 0)) as Tile;
                if (tile == selected)
                {
                    beatNotes.Add(y);
                }
            }
            notes.Add(x, beatNotes);
        }
        Debug.Log(notes);
        return notes;
    }

    public void ActivateGrid(bool active)
    {
        grid.gameObject.SetActive(active);
    }
    
    private void PrintDict(Dictionary<int, List<int>> dict)
    {
        foreach (var info in dict)
        {
            Debug.Log($"Beat: {info.Key}");
            foreach (var thing in dict[info.Key])
            {
                Debug.Log($"note: {thing}");
            }
        }        
    }
}