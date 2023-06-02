using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MusicGrid : MonoBehaviour
{
    public static MusicGrid instance;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile selected;
    [SerializeField] private Tile empty;
    
    private Dictionary<int, List<int>> _currentNotes;
    private bool _intearactalbe = false;

    private float[] _betterKeys = { 0, 2, 4, 5, 7, 9, 11, 12};

    public bool Interactable => _intearactalbe;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More then one musicGrid");
            return;

        }
        instance = this;
        ActivateGrid(false);
    }

    private void Update()
    {
        ClickGrid();
    }

    private void ClickGrid()
    {
        if (Input.GetMouseButtonDown(0) && _intearactalbe)
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

    public Dictionary<int, List<int>> GetNotes()
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
        _intearactalbe = active;
    }
}