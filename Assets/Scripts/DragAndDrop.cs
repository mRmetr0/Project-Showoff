using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragAndDrop : MonoBehaviour
{
    public Type type;
    public enum Type
    {
        Drums,
        Bass, 
        Guitar,
        Keytar,
        KeytarGrid,
        GuitarGrid,
        BassGrid,
        DrumGrid,
        Null
    }

    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private Vector3 _basePos;
    private bool _dragging = false;
    private bool _usable = true;
    
    void Start ()
    {
        _basePos = transform.localPosition;
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        ButtonManager.OnPlay += CannotPlay;
        ButtonManager.OnStop += CanPlay;
    }

    private void OnDisable()
    {
        ButtonManager.OnPlay -= CannotPlay;
        ButtonManager.OnStop -= CanPlay;
    }
    
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (_collider == Physics2D.OverlapPoint(mousePos) && _usable)
            {
                _dragging = true;
            }
        }

        if (_dragging)
            this.transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);

        if (!Input.GetMouseButtonUp(0)) return;
        if (_dragging)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
            foreach (Collider2D hit in colliders)
            {
                if (hit.gameObject.tag == "Reciever")
                {
                    HandleReciever(hit.gameObject);
                }
                
                if (type == Type.KeytarGrid || type == Type.GuitarGrid || type == Type.DrumGrid || type == Type.BassGrid)
                    MusicGrid.instance.GridOn(type);
            }
        }
        _dragging = false;
        transform.position = transform.parent.position + _basePos + Vector3.forward*-0.1f;
    
    }

    private void HandleReciever(GameObject reciever)
    {
        reciever.GetComponent<Monster>().SetInstrument(type);
    }

    private void CanPlay()
    {
        _usable = true;
    }
    private void CannotPlay()
    {
        _usable = false;
    }
}
