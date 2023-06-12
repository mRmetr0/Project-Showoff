using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(AudioSource))][RequireComponent(typeof(AnimatorController))]
public class Monster : MonoBehaviour
{
    [Space(5)][Header ("Instrument AudioClips:")]
    [SerializeField] private AudioClip drums;
    [SerializeField] private AudioClip bass;
    [SerializeField] private AudioClip guitar;
    [SerializeField] private AudioClip keytar;
    [SerializeField] private AudioClip keytarGrid;
    
    private Collider2D _collider;
    private AudioSource _source;
    private DragAndDrop.Type _instHold = DragAndDrop.Type.Null;
    private Animator _animator;

    private Dictionary<int, List<int>> _currentNotes;
    private float _bpmInSeconds = 0;
    private int _beat = -1;
    private bool _canPlay = false;
    private bool _clickable = true;
    
    private double _nextTime;

    private readonly float _transpose = 5;

    private float[] _betterKeys = { 0, 2, 4, 5, 7, 9, 11, 12}; //White key, includes second octave

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _source = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _source.loop = true;
    }

    private void OnEnable()
    {
        ButtonManager.OnPlay += StartTrack;
        ButtonManager.OnStop += StopTrack;
    }

    private void OnDisable()
    {
        ButtonManager.OnPlay -= StartTrack;
        ButtonManager.OnStop -= StopTrack;
    }

    private void Update()
    {   
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && _collider == Physics2D.OverlapPoint(mousePos) && _clickable && !MusicGrid.instance.Interactable)
        {
            Reset();
        }
        
        PlayGrid();
    }

    private void playSound(AudioClip pClip, float pPitch, float pVolume = 1.0f)
    {
        AudioSource soundSource = this.AddComponent<AudioSource>();
        soundSource.pitch = pPitch;
        soundSource.volume = pVolume;
        soundSource.clip = pClip;
        soundSource.Play();
        Destroy(soundSource, pClip.length);
    }
    
    private void Reset()
    {
        StopTrack();
        _source.clip = null;
        _instHold = DragAndDrop.Type.Null;
        _canPlay = false;
        SetAnimation();
    }
    
    public void SetInstrument(DragAndDrop.Type inst)
    {
        _instHold = inst;
        SetAnimation();
    }

    private void StartTrack()
    {
        switch (_instHold)
        {
            case(DragAndDrop.Type.Drums):
                _source.clip = drums;
                break;
            case(DragAndDrop.Type.Bass):
                _source.clip = bass;
                break;
            case(DragAndDrop.Type.Guitar):
                _source.clip = guitar;
                break;
            case(DragAndDrop.Type.Keytar):
                _source.clip = keytar;
                break;
            case (DragAndDrop.Type.KeytarGrid):
                _source.clip = null;
                SetToPlay();
                break;
        }
        if (_source.clip != null)
            _source.Play();

        _clickable = false;
    }

    private void PlayGrid()
    {
        if (!_canPlay) return;
        if (AudioSettings.dspTime >= _nextTime)
        {
            _nextTime += _bpmInSeconds;
            _beat++;

            if (_beat >= _currentNotes.Count) 
            {
                _beat = 0;
            }
            
            List<int> notes = _currentNotes[_beat];
            foreach (int note in notes)
            {
                float pitch = Mathf.Pow(2, (_betterKeys[note]+_transpose)/12.0f);
                playSound(keytarGrid, pitch);
            }
        }
    }
    private void SetToPlay()
    {
        _bpmInSeconds = 60.0f / SoundManager.instance.bpm;
        _nextTime = AudioSettings.dspTime;
        _canPlay = true;
        _currentNotes = MusicGrid.instance.GetNotes();
        _beat = -1;
    }
    private void StopTrack()
    {
        _source.Stop();
        _canPlay = false;
        _clickable = true;
    }

    private void SetAnimation()
    {
        _animator.SetBool("playBass", false);
        _animator.SetBool("playGuitar", false);
        _animator.SetBool("playDrums", false);
        _animator.SetBool("playKeytar", false);
        
        switch (_instHold){
            case DragAndDrop.Type.Bass:
                _animator.SetBool("playBass", true);
                break;
            case DragAndDrop.Type.Guitar:
                _animator.SetBool("playGuitar", true);
                break;
            case DragAndDrop.Type.Drums:
                _animator.SetBool("playDrums", true);
                break;
            case DragAndDrop.Type.Keytar:
            case DragAndDrop.Type.KeytarGrid:
                _animator.SetBool("playKeytar", true);
                break;
            case DragAndDrop.Type.Null:
            default:
                break;
        }
    }
}
