using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapGame : MonoBehaviour
{
    [SerializeField] private Collider2D[] tile;
    private List<Collider2D> _pendingColliders;
    private int _score = 0;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        foreach (Collider2D clicked in colliders)
        {
            if (clicked == _pendingColliders[0])
            {
                _score++;
                return;
            }
        }

    }
}