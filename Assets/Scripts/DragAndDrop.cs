using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragAndDrop : MonoBehaviour
{
    public static Action<Vector2> onDragging; 
    
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
    private Collider2D _collider;
    private Vector3 _basePos;
    private bool _dragging = false;
    public bool Usable { get; set; }

    void Start ()
    {
        _basePos = transform.localPosition;
        _collider = GetComponent<Collider2D>();
        Usable = true;
    }

    void Update()
    {
        if (MusicGrid.instance.Interactable) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (_collider == Physics2D.OverlapPoint(mousePos) && Usable)
            {
                _dragging = true;
            }
        }

        if (_dragging)
        {
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            onDragging?.Invoke(new Vector2(mousePos.x, mousePos.y));
        }


        if (!Input.GetMouseButtonUp(0)) return;
        if (_dragging)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
            foreach (Collider2D hit in colliders)
            {
                if (hit.gameObject.tag == "Reciever")
                {
                    HandleReciever(hit.gameObject);
                    if (type == Type.KeytarGrid || type == Type.GuitarGrid || type == Type.DrumGrid || type == Type.BassGrid)
                        MusicGrid.instance.GridOn(hit.GetComponent<Monster>());
                }
                
            }
        }
        _dragging = false;
        transform.position = transform.parent.position + _basePos + Vector3.forward*-0.1f;
    }

    private void HandleReciever(GameObject reciever)
    {
        reciever.GetComponent<Monster>().SetInstrument(type);
        ButtonManager.instance.SetButtonActive(true);
    }
}
