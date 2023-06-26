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
    [SerializeField] private RuleTile exit;
    
    [Space(5)][Header("Instrument grid tiles")]
    [SerializeField] private RuleTile guitarSelected;
    [SerializeField] private RuleTile guitarEmpty;
    [SerializeField] private RuleTile keytarSelected;
    [SerializeField] private RuleTile keytarEmpty;
    [SerializeField] private RuleTile drumsSelected;
    [SerializeField] private RuleTile drumsEmpty;
    [SerializeField] private RuleTile bassSelected;
    [SerializeField] private RuleTile bassEmpty;
    private RuleTile selected;
    private RuleTile empty;

    private Monster _monster;

    private RuleTile _toDraw;
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
        selected = guitarSelected;
        empty = guitarEmpty;
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
                _toDraw = empty;
            else
                _toDraw = selected;
        }

        if (_toDraw == null) return;
        if (Input.GetMouseButton(0))
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            RuleTile tile = tilemap.GetTile(mousePos) as RuleTile;
            if (tile == null) return;
            if (_toDraw == selected && tile == empty)
                _monster.PlayKeySound(mousePos.y);
            tilemap.SetTile(mousePos, _toDraw);
        }
    }

    public bool[][] GetNotes()
    {
        bool[][] notes = new bool [8][];
        for (int i = notes.Length-1; i >=0; i--)
        {
            notes[i] = new bool[8];
        }

        for (int x = 0; x < notes.Length; x++)
        {
            for (int y = 0; y < notes.Length; y++)
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
        for (int x = 0; x < pNotes.Length; x++)
        {
            for (int y = 0; y < pNotes.Length; y++)
            {
                RuleTile tile = pNotes[x][y] ? selected : empty;
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    private void SetClearGrid()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                RuleTile tile = tilemap.GetTile(new Vector3Int(x, y, 0)) as RuleTile;
                if (tile != null)
                    tilemap.SetTile(new Vector3Int(x, y, 0), empty);
            }
        }
    }

    private void SetTiles(DragAndDrop.Type type)
    {
        switch (type)
        {
            case(DragAndDrop.Type.GuitarGrid):
                selected = guitarSelected;
                empty = guitarEmpty;
                break;
            case(DragAndDrop.Type.BassGrid):
                selected = bassSelected;
                empty = bassEmpty;
                break;
            case(DragAndDrop.Type.DrumGrid):
                selected = drumsSelected;
                empty = drumsEmpty;
                break;
            case(DragAndDrop.Type.KeytarGrid):
                selected = keytarSelected;
                empty = keytarEmpty;
                break;
            case DragAndDrop.Type.Drums:
            case DragAndDrop.Type.Bass:
            case DragAndDrop.Type.Guitar:
            case DragAndDrop.Type.Keytar:
            case DragAndDrop.Type.Null:
            default:
                return;
        }
        SetClearGrid();
    }

    public void GridOn(Monster pMonster)
    {
        _monster = pMonster;
        ButtonManager.instance.SetButtonActive(false, true);
        SetTiles(_monster.InstHold);
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