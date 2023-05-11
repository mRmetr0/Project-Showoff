using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    
    private bool canMove = false;
    private bool dragging = false;
    private Collider2D collider;
    private Vector3 basePos;
    
    void Start ()
    {
        collider = GetComponent<Collider2D>();
        basePos = transform.position;
    }

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            if (collider == Physics2D.OverlapPoint(mousePos))
            {
                canMove = true;
            }
            else
            {
                canMove = false;
            }

            if (canMove)
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

            canMove = false;
            dragging = false;
            transform.position = basePos;
        }
    }

    private void HandleReciever(GameObject reciever)
    {
        reciever.GetComponent<SpriteRenderer>().color = this.GetComponent<SpriteRenderer>().color;
        reciever.GetComponent<Monster>().SetClip(clip);
    }
}
