using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstrumentSign : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    
    private Vector3 _showPos, _hidePos, startPos, endPos;
    private float _t;
    private bool _shouldMove;
    private void Awake()
    {
        _showPos = transform.position;
        _hidePos = _showPos + (Vector3.up * transform.localScale.y * 5f);
    }

    private void OnEnable()
    {
        ButtonManager.OnPlay += Hide;
        ButtonManager.OnStop += Show;
    }

    private void OnDisable()
    {
        ButtonManager.OnPlay -= Hide;
        ButtonManager.OnStop -= Show;
    }

    private void Update()
    {
        if (!_shouldMove) return;
        _t += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPos, endPos, _t);
        if (_t < 1) return;
        _shouldMove = false;
        _t = 0;
    }

    private void Hide()
    {
        startPos = _showPos;
        endPos = _hidePos;
        GeneralSetup();
    }
    private void Show()
    {
        startPos = _hidePos;
        endPos = _showPos;
        GeneralSetup();
    }

    private void GeneralSetup()
    {
        _shouldMove = true;
        transform.position = startPos;
        _t = 0;
    }
}
