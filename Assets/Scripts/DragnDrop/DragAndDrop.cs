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
    private Collider2D collider;
    private Vector3 basePos;
    private Color baseColor;
    private bool dragging = false;
    private bool usable = true;
    
    void Start ()
    {
        collider = GetComponent<Collider2D>();
        basePos = transform.position;
        baseColor = GetComponent<SpriteRenderer>().color;
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
            if (collider == Physics2D.OverlapPoint(mousePos) && usable)
            {
                dragging = true;
            }
        }

        if (dragging)
            this.transform.position = mousePos;
        
        if (Input.GetMouseButtonUp(0))
        {
            if (dragging)
            {
                Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
                foreach (Collider2D hit in colliders)
                {
                    if (hit.gameObject.tag == "Reciever")
                    {
                        HandleReciever(hit.gameObject);
                    }
                }
            }
            dragging = false;
            transform.position = basePos;
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
        usable = true;
        GetComponent<SpriteRenderer>().color = baseColor;
    }
    private void CannotPlay()
    {
        usable = false;
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
}
