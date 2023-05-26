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
        Trumpet, 
        Guitar,
        Keytar,
        KeytarGrid,
        Null
    }
    
    private Collider2D _collider;
    private Vector3 _basePos;
    private Color _baseColor;
    private bool _dragging = false;
    private bool _usable = true;
    
    void Start ()
    {
        _collider = GetComponent<Collider2D>();
        _basePos = transform.position;
        _baseColor = GetComponent<SpriteRenderer>().color;
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
            this.transform.position = mousePos;
        
        if (Input.GetMouseButtonUp(0))
        {
            if (_dragging)
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
                foreach (Collider2D hit in colliders)
                {
                    if (hit.gameObject.tag == "Reciever")
                    {
                        HandleReciever(hit.gameObject);
                    }
                    
                    if (type == Type.KeytarGrid)
                        MusicGrid.instance.ActivateGrid(true);
                }
            }
            _dragging = false;
            transform.position = _basePos;
        }
    }

    private void HandleReciever(GameObject reciever)
    {
        reciever.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        //reciever.GetComponent<Monster>().SetClip(clip);
        reciever.GetComponent<Monster>().SetInstrument(type);
        reciever.GetComponent<Monster>().GetInstrument().sprite = this.GetComponent<SpriteRenderer>().sprite;
    }

    private void CanPlay()
    {
        _usable = true;
        GetComponent<SpriteRenderer>().color = _baseColor;
    }
    private void CannotPlay()
    {
        _usable = false;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
