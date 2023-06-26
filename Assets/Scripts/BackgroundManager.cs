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

    private List<InstTutorial> _tutorials = new();
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
        title.gameObject.SetActive(true);
        leftCurtain.gameObject.SetActive(true);
        rightCurtain.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _open = true;
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
            InstTutorial tutorial = ScriptableObject.CreateInstance<InstTutorial>();
            GameObject pointer = Instantiate(tutorialPrefab);
            Random r = new();
            Vector3 instPos = DragAndDrop.dragAndDrops[r.Next(0, DragAndDrop.dragAndDrops.Count - 1)].transform.position;
            Vector3 monstPos = Monster.monsters[r.Next(0, Monster.monsters.Count-1)].transform.position;
            tutorial.Instantiate(pointer , instPos, monstPos);
            _tutorials.Add(tutorial);
    }

    private void HandleTutorials()
    {   
        foreach (InstTutorial inst in _tutorials)
        {
            if (inst.toDestroy)
            {
                _tutorials.Remove(inst);
                Destroy(inst);
                return;
            }

            inst.Move();
        }
    }
}

class InstTutorial : ScriptableObject
{
    private GameObject _pointer;
    private Vector3 _startPos, _endPos;
    private float _t;
    public bool toDestroy;
    public void Instantiate (GameObject pPointer, Vector3 pStartPos, Vector3 pEndPos)
    {
        _pointer = pPointer;
        _startPos = pStartPos;
        _endPos = pEndPos;
    }

    public void Move()
    {
        _t += 0.01f * Time.deltaTime;
        _pointer.transform.position = Vector3.Lerp(_startPos, _endPos, _t);
        if (_t >= 1)
            toDestroy = true;
    }

    private void OnDestroy()
    {
        Destroy(_pointer);
    }
}