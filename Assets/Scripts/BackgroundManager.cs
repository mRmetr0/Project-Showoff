using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPrefab;
    [Header("TitleScreen")] 
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject leftCurtain, rightCurtain;
    [SerializeField] private float curtanSpeed;
    [SerializeField] private float popUpTimer;

    private List<InstTutorial> _tutorials = new();
    private InstTutorial _pointer;
    private Vector3 _startPosR;
    private Vector3 _endPosR;
    private Vector3 _startPosL;
    private Vector3 _endPosL;
    private Vector3 _startPosT;
    private Vector3 _endPosT;
    private float _curtanTime;
    private float _timer;
    private bool _open;
    private bool _pointerNotNull;
    private bool _isPlaying;

    private void Awake()
    {
        _startPosR = rightCurtain.transform.position;
        _endPosR = _startPosR + Vector3.right * 10;
        _startPosL = leftCurtain.transform.position;
        _endPosL = _startPosL + Vector3.left * 10;
        _startPosT = title.transform.position;
        _endPosT = _startPosT + Vector3.up * 10;
        title.gameObject.SetActive(true);
        leftCurtain.gameObject.SetActive(true);
        rightCurtain.gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        ButtonManager.onPlay += TurnOff;
        ButtonManager.onStop += TurnOn;
    }

    private void OnDisable()
    {
        ButtonManager.onPlay -= TurnOff;
        ButtonManager.onStop -= TurnOn;
    }

    private void OnDestroy()
    {
        if (_pointer != null) 
            Destroy(_pointer);
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            _open = true;
            _timer = popUpTimer;
            if (_pointer != null) Destroy(_pointer);
        }
        
        if (_curtanTime <= 1 && _open) 
        {
            _curtanTime += curtanSpeed * Time.deltaTime;
            rightCurtain.transform.position = Vector3.Lerp(_startPosR, _endPosR, _curtanTime);
            leftCurtain.transform.position = Vector3.Lerp(_startPosL, _endPosL, _curtanTime);
            title.transform.position = Vector3.Lerp(_startPosT, _endPosT, _curtanTime);
        }
        
        HandleTutorials();

    }

    private void AddTutorial()
    {
        if (DragAndDrop.dragAndDrops.Count == 0 || Monster.monsters.Count == 0) return;
        InstTutorial tutorial = ScriptableObject.CreateInstance<InstTutorial>();
        tutorial.Instantiate(tutorialPrefab , DragAndDrop.dragAndDrops.ToArray(), Monster.monsters.ToArray());
        _pointer = tutorial;
        Debug.Log($"pointer {_pointer}, bool: {_pointer == null}, {_pointerNotNull}");
    }

    private void HandleTutorials()
    {
        if (_isPlaying) return;
        if (_pointer != null)
        {
            _pointer.Move();
            return;
        }
        if (_timer <= 0)
        {
            AddTutorial();
            return;
        }
        _timer -= Time.deltaTime;
    }

    private void TurnOff()
    {
        if (_pointer != null) Destroy(_pointer);
        _isPlaying = true;
    }

    private void TurnOn()
    {
        _isPlaying = false;
        _timer = popUpTimer;
    }
}

class InstTutorial : ScriptableObject
{
    private GameObject _pointer;
    private Vector3[] _starts, _ends;
    private Vector3 _startPos, _endPos;
    private Random _rand;
    private float _t;
    public void Instantiate(GameObject pPrefab, DragAndDrop[] pStarts, Monster[] pEnds)
    {
        _rand = new();
        List<Vector3> startPos = new();
        List<Vector3> endPos = new();
        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = new (pStarts[i].transform.position.x, pStarts[i].transform.position.y, -.5f);
            startPos.Add( pos);
        }
        for (int i = 0; i < 4; i++)
        {
            Vector3 pos = new (pEnds[i].transform.position.x, pEnds[i].transform.position.y, -.5f);
            endPos.Add(pos);
        }

        _starts = startPos.ToArray();
        _ends = endPos.ToArray();
        Reset();
        
        _pointer = Instantiate(pPrefab, _startPos, Quaternion.identity);
    }

    public void Move()
    {
        _t += 0.3f * Time.deltaTime;
        float t = _t;
        if (t <= 1) t = CalcSpeed(_t);
        _pointer.transform.position = Vector3.Lerp(_startPos, _endPos, t);
        if (_t >= 1.5)
            Reset();
    }

    private void Reset()
    {
        _startPos = _starts[_rand.Next(0, _starts.Length)];
        _endPos = _ends[_rand.Next(0, _ends.Length)];
        _t = 0;
    }

    private void OnDestroy()
    {
        Destroy(_pointer);
    }

    private float CalcSpeed(float t)
    {
        float inSin = (t * (Mathf.PI / 2) * 2 - (Mathf.PI / 2));
        float rest = (inSin / 2) + 0.5f;
        return rest;
    }
}