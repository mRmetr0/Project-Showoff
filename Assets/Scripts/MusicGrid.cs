using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MusicGrid : MonoBehaviour
{
    public static MusicGrid instance;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private RuleTile selected;
    [SerializeField] private RuleTile empty;

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
        GridOff();
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
            if (tile == null)
            {
                //ActivateGrid(false);
                GridOff();
                return;
            }
            if (tile == selected)
                ToDraw = empty;
            else
                ToDraw = selected;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            RuleTile tile = tilemap.GetTile(mousePos) as RuleTile;
            if (tile == null) return;
            tilemap.SetTile(mousePos, ToDraw);
            if (ToDraw != selected) return;
            _monster.PlayKeySound(mousePos.y);
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

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
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
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                RuleTile tile = pNotes[x][y] ? selected : empty;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    private void SetClearGrid()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), empty);
            }
        }
    }

    public void GridOn(Monster pMonster)
    {
        _monster = pMonster;
        if (pMonster.Notes == null) SetClearGrid();
        else SetNotes(pMonster.Notes);

        
        ActivateGrid(true);
    }

    public void GridOff()
    {
        ActivateGrid(false);
        if (_monster != null)
            _monster.Notes = GetNotes();
        SetClearGrid();
    }

    public void ActivateGrid(bool active)
    {
        grid.gameObject.SetActive(active);
        _interactable = active;
    }
}