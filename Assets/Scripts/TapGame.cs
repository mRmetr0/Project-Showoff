using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

public class TapGame : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Collider2D[] tiles;
    private List<TapInfo> _pendingColliders = new List<TapInfo>();

    private float _bpmInSeconds = 0;
    private int _score = 0;
    private bool _canPlay = false;
    
    private double _nextTime;
    
    private void OnEnable()
    {
        ButtonManager.OnPlay += StartMusic;
        ButtonManager.OnStop += StopMusic;
    }

    private void OnDisable()
    {
        ButtonManager.OnPlay -= StartMusic;
        ButtonManager.OnStop -= StopMusic;
    }

    private void Update()
    {
        if (_canPlay)
        {
            if (AudioSettings.dspTime >= _nextTime)
            {
                _nextTime += _bpmInSeconds * 4;
                SetMonster();
            }
        }
        TapMonster();

        foreach (TapInfo info in _pendingColliders)
        {
            info.LerpPos(0.01f);
        }
    }

    private void SetMonster()
    {
        
        Random r = new();
        Collider2D monster = tiles[r.Next(0, tiles.Length)];

        do
        {
            monster = tiles[r.Next(0, tiles.Length)];
        } while (InPending(monster));

        GameObject note = Instantiate(notePrefab, monster.transform.position + new Vector3(0, 7, -2), Quaternion.identity);

        TapInfo info = ScriptableObject.CreateInstance<TapInfo>();
        info.Instantiate(monster, _nextTime, note);

        _pendingColliders.Add(info);
        
        if (_pendingColliders.Count > 2)
        {
            TapInfo toRemove = _pendingColliders[0];
            _pendingColliders.Remove(toRemove);
            Destroy(toRemove);
        }
    }

    private void TapMonster()
    {
        if (!Input.GetMouseButtonDown(0) || _pendingColliders.Count <= 0) return;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        if (colliders.Length <= 0) return;
        foreach (Collider2D clicked in colliders)
        {
            TapInfo info = ColliderFromPending(clicked);
            if (info != null)
            {
                _score += info.GetScore();
                Debug.Log(_score);
                _pendingColliders.Remove(info);
                Destroy(info);
                return;
            }
        }
    }

    public void StartMusic()
    {
        _bpmInSeconds = 60.0f / SoundManager.instance.bpm;
        _nextTime = AudioSettings.dspTime;
        _canPlay = true;
    }

    public void StopMusic()
    {
        _canPlay = false;
        for (int i = _pendingColliders.Count - 1; i >= 0; i--)
        {
            TapInfo info = _pendingColliders[i];
            _pendingColliders.Remove(info);
            Destroy(info);
        }

        _pendingColliders.Clear();
    }

    private bool InPending(Collider2D collider)
    {
        foreach (TapInfo info in _pendingColliders)
        {
            if (info.collider == collider) return true;
        }

        return false;
    }

    private TapInfo ColliderFromPending(Collider2D collider)
    {
        foreach (TapInfo info in _pendingColliders)
        {
            if (info.collider == collider) return info;
        }

        return null;
    }
}

class TapInfo : ScriptableObject
{
    public Collider2D collider;
    public GameObject note;
    public Vector3 startPos;
    public double beat;
    private float _t = 0;

    public TapInfo(Collider2D pCollider, double pBeat, GameObject pNote)
    {
        collider = pCollider;
        beat = pBeat;
        note = pNote;
        startPos = pNote.gameObject.transform.position;

    }

    public void Instantiate(Collider2D pCollider, double pBeat, GameObject pNote)
    {
        collider = pCollider;
        beat = pBeat;
        note = pNote;
        startPos = pNote.gameObject.transform.position;
    }

    public void LerpPos(float speed = 0.05f)
    {
        _t += speed;
        var position = collider.gameObject.transform.position;
        note.transform.position = Vector3.Lerp(startPos, new Vector3(position.x, position.y, -2), _t);
    }

    public int GetScore()
    {
        int score = (int)(_t * 10);
        return score;
    }

    private void OnDestroy()
    {
        Destroy(note);
    }
}