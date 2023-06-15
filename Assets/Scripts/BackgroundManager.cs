using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("TitleScreen")] 
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject leftCurtain, rightCurtain;
    [SerializeField] private float curtanSpeed;
    [Header("TreeEyes")] 
    [SerializeField] private GameObject[] treeEyes;

    private Vector3 _startPosR;
    private Vector3 _endPosR;
    private Vector3 _startPosL;
    private Vector3 _endPosL;
    private Vector3 _startPosT;
    private Vector3 _endPosT;
    private float _curtanTime;
    private bool _open;

    private void Awake()
    {
        _startPosR = rightCurtain.transform.position;
        _endPosR = _startPosR + Vector3.right * 10;
        _startPosL = leftCurtain.transform.position;
        _endPosL = _startPosL + Vector3.left * 10;
        _startPosT = title.transform.position;
        _endPosT = _startPosT + Vector3.up * 10;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _open = true;
        if (_curtanTime <= 1 && _open) 
        {
            _curtanTime += curtanSpeed;
            rightCurtain.transform.position = Vector3.Lerp(_startPosR, _endPosR, _curtanTime);
            leftCurtain.transform.position = Vector3.Lerp(_startPosL, _endPosL, _curtanTime);
            title.transform.position = Vector3.Lerp(_startPosT, _endPosT, _curtanTime);
        }
    }
}

class Mover
{
    private GameObject obj;
    private Vector3 startPos, endPos;
    private float _t;
    public Mover(GameObject pObj, Vector3 pEndPos)
    {
        obj = pObj;
        endPos = pEndPos;
    }

    public void Move()
    {
        
        
    }
}