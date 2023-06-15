using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEditor.PackageManager;
using UnityEngine;

[RequireComponent(typeof(AnimatorController))]
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
    private AudioSource[] _keySources;
    private DragAndDrop.Type _instHold = DragAndDrop.Type.Null;
    private Animator _animator;

    private int _beat = -1;
    private bool _canPlay = false;
    private bool _clickable = true;
    private bool _grabbing;
    
    private readonly float _transpose = 0;
    private float[] _betterKeys = { 0, 2, 4, 5, 7, 9, 11, 12}; //White key, includes second octave

    public static List<Monster> monsters;
    public bool[][] Notes { get; set; }
    public DragAndDrop.Type InstHold => _instHold;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        SetKeySources();
    }

    private void Start()
    {
        if (monsters == null)
            monsters = new List<Monster>();
        monsters.Add(this);
    }

    private void OnEnable()
    {
        SoundManager.onBeat += PlayBeat;
        ButtonManager.onPlay += StartTrack;
        ButtonManager.onStop += StopTrack;
        DragAndDrop.onDragging += CalculateDistance;
    }

    private void OnDisable()
    {
        SoundManager.onBeat -= PlayBeat;
        ButtonManager.onPlay -= StartTrack;
        ButtonManager.onStop -= StopTrack;
        DragAndDrop.onDragging -= CalculateDistance;
    }

    private void Update()
    {   
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && _collider == Physics2D.OverlapPoint(mousePos) && _clickable && !MusicGrid.instance.Interactable)
        {
            Reset();
        }
    }

    private void SetKeySources()
    {
        _keySources = new AudioSource[_betterKeys.Length];
        for (int i = 0; i < _betterKeys.Length; i++)
        {
            AudioSource source = this.AddComponent<AudioSource>();
            source.pitch = Mathf.Pow(2, (_betterKeys[i] + _transpose) / 12.0f);
            _keySources[i] = source;
        }
    }

    public void PlayKeySound(int key)
    {
        AudioSource source = _keySources[key];
        source.PlayOneShot(_instClip);
    }

    private void Reset()
    {
        StopTrack();
        _source.clip = null;
        _instHold = DragAndDrop.Type.Null;
        _canPlay = false;
        SetAnimation();
        ButtonManager.instance.SetButtonActive(false);
    }
    
    public void SetInstrument(DragAndDrop.Type inst)
    {
        _instHold = inst;
        SetInstClip();
        SetAnimation();
    }

    private void StartTrack()
    {
        if (_source.clip != null) //TODO: make audio track sync with first beat;
            _source.Play(); 
        if (_instClip != null)
            SetToPlay();

        _clickable = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetInstClip()
    {
        _source.clip = null;
        _instClip = null;
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
                _instClip = keytarGrid;
                break;
            case (DragAndDrop.Type.DrumGrid):
                _instClip = drumGrid;
                break;
            case (DragAndDrop.Type.GuitarGrid):
                _instClip = guitarGrid;
                break;
            case (DragAndDrop.Type.BassGrid):
                _instClip = bassGrid;
                break;
            case (DragAndDrop.Type.Null):
                break;
            default:
                Debug.LogError("Incorrect type given.");
                break;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
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
                    PlayKeySound(i);
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
        _animator.SetBool("grabbing", false);
        
        switch (_instHold){
            case DragAndDrop.Type.BassGrid:
            case DragAndDrop.Type.Bass:
                _animator.SetTrigger("bass");
                break;
            case DragAndDrop.Type.GuitarGrid:
            case DragAndDrop.Type.Guitar:
                _animator.SetTrigger("guitar");
                break;
            case DragAndDrop.Type.DrumGrid:
            case DragAndDrop.Type.Drums:
                _animator.SetTrigger("drums");
                break;
            case DragAndDrop.Type.Keytar:
            case DragAndDrop.Type.KeytarGrid:
                _animator.SetTrigger("keytar");
                break;
            case DragAndDrop.Type.Null:
                _animator.SetTrigger("idle");
                break;
            default:

                break;
        }
    }

    private void CalculateDistance(Vector2 mousePos)
    {
        if ((new Vector2(transform.position.x, transform.position.y) - mousePos).magnitude < 1 && !_grabbing)
        {
            _grabbing = true;
            _animator.SetBool("grabbing", true);
        } else if ((new Vector2(transform.position.x, transform.position.y) - mousePos).magnitude > 1 && _grabbing)
        {
            _grabbing = false;
            _animator.SetBool("grabbing", false);
        }
    }
}
