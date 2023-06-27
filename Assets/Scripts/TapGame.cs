using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;
using Random = System.Random;

[RequireComponent(typeof(AudioSource))]
public class TapGame : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject outlinePrefab;
    [SerializeField] private Collider2D[] tiles;
    [SerializeField] private AudioClip feedbackClip;
    [SerializeField] private AudioClip missedNoteClip;
    [SerializeField] private int winScore;
    [SerializeField][Range(0, 10)] private int cheerThreshold;
    [SerializeField] [Range(0.0f, 10.0f)] private int feedback;
    
    private List<TapInfo> _pendingColliders = new ();
    private AudioSource _source;

    private float _bpmInSeconds = 0;
    private int _score = 0;
    private int _tileAmount;
    private bool _canPlay = false;
    
    private double _nextTime;
    
    private void OnEnable() 
    {
        ButtonManager.onPlay += StartMusic;
        ButtonManager.onStop += StopMusic;
        _source = GetComponent<AudioSource>();
    }
    private void OnDisable()
    {
        ButtonManager.onPlay -= StartMusic;
        ButtonManager.onStop -= StopMusic;
    }
    private void Update()
    {
        if (_canPlay)
        {
            if (AudioSettings.dspTime >= _nextTime)
            {
                _nextTime += _bpmInSeconds*4;
                SetMonster();
            }
        }
        TapMonster();

        
        for (int i = _pendingColliders.Count - 1; i >= 0; i--)
        {
            TapInfo info = _pendingColliders[i];
            info.LerpPos(0.005f);
            if (info.toDelete)
            {
                _pendingColliders.Remove(info);
                Destroy(info);
                _source.PlayOneShot(missedNoteClip);
            }
        }
    }
    private void SetMonster()
    {
        if (_pendingColliders.Count > 2) return;
        
        Random r = new();
        Collider2D monster = tiles[r.Next(0, tiles.Length)];

        do
        {
            monster = tiles[r.Next(0, tiles.Length)];
        } while (InPending(monster));

        GameObject note = Instantiate(notePrefab, monster.transform.position + new Vector3(0, 7, -2), Quaternion.identity);
        GameObject outline = null;
        outline = Instantiate(outlinePrefab, monster.transform.position + (Vector3.forward * -0.5f), Quaternion.identity);

        TapInfo info = ScriptableObject.CreateInstance<TapInfo>();
        info.Instantiate(monster, _nextTime, note, outline);

        _pendingColliders.Add(info);
        _tileAmount++;
    }
    private void TapMonster()
    {
        if (!Input.GetMouseButtonDown(0) || _pendingColliders.Count <= 0) return;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
        if (colliders.Length <= 0) return;
        foreach (Collider2D clicked in colliders)
        {
            TapInfo info = ColliderInPending(clicked);
            if (info != null)
            {
                int score = info.GetScore();
                if (score >= feedback) _source.PlayOneShot(feedbackClip);
                _score += score;
                Debug.Log($"Score: {_score}");
                _pendingColliders.Remove(info);
                Destroy(info);
                if (_score >= winScore) ButtonManager.instance.StartStop();
                return;
            }
        }
    }
    public void StartMusic()
    {
        _bpmInSeconds = 60.0f / SoundManager.instance.bpm;
        _nextTime = AudioSettings.dspTime;
        _canPlay = true;
        _tileAmount = 0;
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

        Debug.Log($"{_score} / {_tileAmount} = {_score / _tileAmount} > {cheerThreshold}; {_score / _tileAmount > cheerThreshold}");
        if (_score / _tileAmount > cheerThreshold)
            Debug.Log("GOOD SCORE");
    }
    private bool InPending(Collider2D collider)
    {
        foreach (TapInfo info in _pendingColliders)
        {
            if (info.collider == collider) return true;
        }
        return false;
    }

    private TapInfo ColliderInPending(Collider2D collider)
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
    public GameObject outline;
    public Vector3 startPos;
    private Vector3 _outlineSize;
    private SpriteRenderer _renderer;
    private float _t = 0;
    private bool _moveUp = true;
    public bool toDelete;

    public void Instantiate(Collider2D pCollider, double pBeat, GameObject pNote, GameObject pOutLine = null)
    {
        collider = pCollider;
        note = pNote;
        startPos = pNote.gameObject.transform.position;
        outline = pOutLine;
        if (outline == null) Debug.LogError("No outline found for TapInfo!!");
        _outlineSize = outline.transform.localScale;
        _renderer = outline.GetComponent<SpriteRenderer>();
    }

    public void LerpPos(float speed = 0.5f)
    {
        //Lerp of note position:
        if (_moveUp)
        {
            _t += speed;
            var position = collider.gameObject.transform.position;
            note.transform.position = Vector3.Lerp(startPos, new Vector3(position.x, position.y, -2), _t);
            if (_t > 1) _moveUp = false;
            //Lerp of outline size (and alpha????):
            outline.transform.localScale = Vector3.Lerp(new Vector3(3,3,0), _outlineSize, _t);
            _renderer.color = new Color(1, 1, 1, _t);
            //
        }
        else
        {
            _t -= speed*3;
            if (_t <= 0)
            {
                toDelete = true;
            }
        }
        
    }

    public int GetScore()
    {
        int score;
        if (_t > 1) score = (int)((1f - (_t - 1f)) * 10f);
        else score = (int)(_t * 10);
        score = Mathf.Clamp(score, 0, 11);
        Debug.Log($"Added score: {score}");
        return score;
    }

    private void OnDestroy()
    {
        Destroy(note);
        if (outline != null) Destroy(outline.gameObject);
    }
}