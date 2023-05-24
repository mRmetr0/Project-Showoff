using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(AudioSource))]
public class MusicGrid : MonoBehaviour
{
    [SerializeField]private AudioClip clip;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile selected;
    [SerializeField] private Tile empty;
    [SerializeField] private float timing;

    private AudioSource _source;
    private Dictionary<int, List<int>> _currentNotes;
    private float _currentTime = .0f;
    private int _currentKey;
    private int _beat;
    private bool _scaleUp = true;
    private bool _canPlay = false;

    private readonly float transpose = 0;
    private float _note = -1;

    private float[] _betterKeys = { 0, 2, 4, 5, 7, 9, 11, 12}; //White key, includes second octave
    
    

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        // _source.clip = clip;
        // _source.Play();
        _currentKey = 0;
    }

    private void Update()
    {
        ClickGrid();
        
        PlayNotes();
    }

    private void PlayNotes()
    {
        if (Input.GetKeyDown("w"))
        {
            _canPlay = true;
            _currentNotes = GetNotes();
            _currentTime = 0;
            _beat = -1;
            //PrintDict(_currentNotes);
        }
        if (_canPlay)
        {
            if (_currentTime >= timing)
            {
                _currentTime = 0;
                _beat++;

                List<int> notes = _currentNotes[_beat];
                
                foreach (int note in notes)
                {
                    _source.pitch = Mathf.Pow(2, (_betterKeys[note]+transpose)/12.0f);
                    _source.PlayOneShot(clip);
                }
                
                if (_beat >= _currentNotes.Count-1) 
                {
                    _canPlay = false;
                    _beat = 0;
                }
            }
            _currentTime += Time.deltaTime;
        }
    }

    private void ClickGrid()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Tile tile = tilemap.GetTile(mousePos) as Tile;
            if (tile == null) return;
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


    private void KeyScaling()
    {
        if (_currentTime >= timing)
        {
            _currentTime = 0;
            _currentKey += _scaleUp ? 1 : -1;
            
            if (_currentKey >= _betterKeys.Length - 1)
                _scaleUp = false;
            
                        
            if (_currentKey <= 0)
                _scaleUp = true;
                
            _source.pitch = Mathf.Pow(2, (_betterKeys[_currentKey]+transpose)/12.0f);
        }

        _currentTime += Time.deltaTime;
    }
}