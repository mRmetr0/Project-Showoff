using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private AudioClip keytarGrid, guitarGrid, drumGrid, bassGrid;

    private AudioClip _instClip;
    private Collider2D _collider;
    private AudioSource _source;
    private DragAndDrop.Type _instHold = DragAndDrop.Type.Null;
    private Animator _animator;

    private int _beat = -1;
    private bool _canPlay = false;
    private bool _clickable = true;
    
    private readonly float _transpose = 5;
    private float[] _betterKeys = { 0, 2, 4, 5, 7, 9, 11, 12}; //White key, includes second octave

    public bool[][] Notes { get; set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _source = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _source.loop = true;
    }

    private void OnEnable()
    {
        SoundManager.onBeat += PlayBeat;
        ButtonManager.onPlay += StartTrack;
        ButtonManager.onStop += StopTrack;
    }

    private void OnDisable()
    {
        SoundManager.onBeat -= PlayBeat;
        ButtonManager.onPlay -= StartTrack;
        ButtonManager.onStop -= StopTrack;
    }

    private void Update()
    {   
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && _collider == Physics2D.OverlapPoint(mousePos) && _clickable && !MusicGrid.instance.Interactable)
        {
            Reset();
        }
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
                _instClip = keytarGrid;
                SetToPlay();
                break;
            case (DragAndDrop.Type.DrumGrid):
                _source.clip = null;
                _instClip = drumGrid;
                SetToPlay();
                break;
            case (DragAndDrop.Type.GuitarGrid):
                _source.clip = null;
                _instClip = guitarGrid;
                SetToPlay();
                break;
            case (DragAndDrop.Type.BassGrid):
                _source.clip = null;
                _instClip = bassGrid;
                SetToPlay();
                break;
        }
        if (_source.clip != null) //TODO: make audio track sync with first beat;
            _source.Play(); 

        _clickable = false;
    }

    private void PlayBeat()
    {
        if (!_canPlay) return;
        _beat++;
        if (_beat >= Notes.Length)
            _beat = 0;
        for (int i = 0; i < Notes.Length; i++)
        {
            bool note = Notes[_beat][i];
            if (note)
            {
                if (i < _betterKeys.Min() || i > _betterKeys.Max())
                {
                    Debug.LogError($"NOTE NOT IN KEY LIST. NOTE: {i}");
                }
                else
                {
                    float pitch = Mathf.Pow(2, (_betterKeys[i]+_transpose)/12.0f);
                    playSound(_instClip, pitch);
                    Debug.Log("Should play sound");
                }
            }
        }
    }

    private void SetToPlay()
    {
        _canPlay = true;
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
            case DragAndDrop.Type.BassGrid:
            case DragAndDrop.Type.Bass:
                _animator.SetBool("playBass", true);
                break;
            case DragAndDrop.Type.GuitarGrid:
            case DragAndDrop.Type.Guitar:
                _animator.SetBool("playGuitar", true);
                break;
            case DragAndDrop.Type.DrumGrid:
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
