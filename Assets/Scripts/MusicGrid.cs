using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MusicGrid : MonoBehaviour
{
    public static MusicGrid instance;
    public static Action<bool> onActivate;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private RuleTile selected;
    [SerializeField] private RuleTile empty;
    [SerializeField] private RuleTile exit;

    private Monster _monster;

    private RuleTile ToDraw;
    private bool _interactable;

    public bool Interactable => _interactable;
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More then one musicGrid");
            return;
        }
        instance = this;
        tilemap.CompressBounds();
    }

    private void Start()
    {
        ActivateGrid(false);
        SetClearGrid();
    }

    private void Update()
    {
        ClickGrid();
    }

    private void ClickGrid()
    {
        if (!_interactable) return;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            RuleTile tile = tilemap.GetTile(mousePos) as RuleTile;
            if (tile == null) return;
            if (tile == exit)
            {
                GridOff();
                return;
            }
            if (tile == selected)
                ToDraw = empty;
            else
                ToDraw = selected;
        }

        if (ToDraw == null) return;
        if (Input.GetMouseButton(0))
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            RuleTile tile = tilemap.GetTile(mousePos) as RuleTile;
            if (tile == null) return;
            tilemap.SetTile(mousePos, ToDraw);
            // if (ToDraw != selected) return;
            // _monster.PlayKeySound(mousePos.y);
        }
    }

    public bool[][] GetNotes()
    {
        BoundsInt bounds = tilemap.cellBounds;
        bool[][] notes = new bool [8][];
        for (int i = notes.Length-1; i >=0; i--)
        {
            notes[i] = new bool[8];
        }

        for (int x = 0; x < notes.Length-1; x++)
        {
            for (int y = 0; y < notes.Length -1; y++)
            {
                RuleTile tile = tilemap.GetTile(new Vector3Int(x, y, 0)) as RuleTile;
                notes[x][y] = (tile == selected);
            }
        }
        return notes;
    }

    public void SetNotes(bool [][] pNotes)
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = 0; x < pNotes.Length-1; x++)
        {
            for (int y = 0; y < pNotes.Length -1; y++)
            {
                RuleTile tile = pNotes[x][y] ? selected : empty;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    private void SetClearGrid()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                RuleTile tile = tilemap.GetTile(new Vector3Int(x, y, 0)) as RuleTile;
                if (tile != null)
                    tilemap.SetTile(new Vector3Int(x, y, 0), empty);
            }
        }
    }

    public void GridOn(Monster pMonster)
    {
        _monster = pMonster;
        ButtonManager.instance.SetButtonActive(false, true);
        if (pMonster.Notes == null) SetClearGrid();
        else SetNotes(pMonster.Notes);
        ActivateGrid(true);
    }

    public void GridOff()
    {
        ButtonManager.instance.SetButtonActive(true);
        ActivateGrid(false);
        if (_monster != null)
            _monster.Notes = GetNotes();
        SetClearGrid();
    }

    private void ActivateGrid(bool active)
    {
        grid.gameObject.SetActive(active);
        _interactable = active;
    }
}