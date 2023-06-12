using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MusicGrid : MonoBehaviour
{
    public static MusicGrid instance;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile selected;
    [SerializeField] private Tile empty;

    private DragAndDrop.Type _inst = DragAndDrop.Type.Null;
    private Dictionary<DragAndDrop.Type, Dictionary<int, List<int>>> _instGrids = new ();
    private Dictionary<int, List<int>> _currentNotes;
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
        if (Input.GetMouseButtonDown(0) && _interactable)
        {
            Vector3Int mousePos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Tile tile = tilemap.GetTile(mousePos) as Tile;
            if (tile == null)
            {
                //ActivateGrid(false);
                GridOff();
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
        return notes;
    }

    public void SetNotes()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Dictionary<int, List<int>> notes = _instGrids[_inst];
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            List<int> beatNotes = notes[x];
            if (beatNotes.Count == 0) continue;
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                if (!beatNotes.Contains(y)) continue;
                tilemap.SetTile(new Vector3Int(x, y, 0), selected);
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

    public void GridOn(DragAndDrop.Type pInst)
    {
        if (pInst == DragAndDrop.Type.Null)
        {
            Debug.LogError("Recieved NULL instrument!");
            return;
        }

        _inst = pInst;
        if (!_instGrids.ContainsKey(pInst)) SetClearGrid();
        else SetNotes();
        
        ActivateGrid(true);
    }

    public void GridOff()
    {
        ActivateGrid(false);
        if (_inst == DragAndDrop.Type.Null) return;
        if (_instGrids.ContainsKey(_inst))
            _instGrids[_inst] = GetNotes();
        else
            _instGrids.Add(_inst, GetNotes());
        SetClearGrid();
    }

    public void ActivateGrid(bool active)
    {
        grid.gameObject.SetActive(active);
        _interactable = active;
    }

    public Dictionary<int, List<int>> GetInstNotes(DragAndDrop.Type pInst)
    {
        if (_instGrids.ContainsKey(pInst))
            return _instGrids[pInst];
        Debug.LogError("Grid was requested that did not exist!");
        return null;
    }
}