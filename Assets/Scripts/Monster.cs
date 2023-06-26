using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour
{
    [SerializeField] private Collider2D stand;
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
    private Camera _camera;
    private static readonly int Grabbing = Animator.StringToHash("grabbing");
    private static readonly int Bass = Animator.StringToHash("bass");
    private static readonly int Guitar = Animator.StringToHash("guitar");
    private static readonly int Drums = Animator.StringToHash("drums");
    private static readonly int Keytar = Animator.StringToHash("keytar");
    private static readonly int Idle = Animator.StringToHash("idle");
    public bool[][] Notes { get; set; }
    public DragAndDrop.Type InstHold => _instHold;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        SetKeySources();
        stand.gameObject.SetActive(false);
        Notes = new bool [8][];
        for (int i = Notes.Length-1; i >=0; i--)
        {
            Notes[i] = new bool[8];
        }
    }

    private void Start()
    {
        _camera = Camera.main;
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
        Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && _clickable && !MusicGrid.instance.Interactable)
        {
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == _collider)
            {
                Reset();
                return;
            }

            if (hit == stand)
            {
                MusicGrid.instance.GridOn(this);
            }
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
        SetInstrument(DragAndDrop.Type.Null);
        _source.clip = null;
        _canPlay = false;
        ButtonManager.instance.SetButtonActive(false);
        stand.gameObject.SetActive(false);
    }
    
    public void SetInstrument(DragAndDrop.Type inst)
    {
        _instHold = inst;
        SetInstClip();
        SetAnimation();
        if (_instHold is DragAndDrop.Type.GuitarGrid or DragAndDrop.Type.BassGrid or DragAndDrop.Type.DrumGrid or DragAndDrop.Type.KeytarGrid)
            stand.gameObject.SetActive(true);
    }

    private void StartTrack()
    {
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
        if (_instClip != null)
        {
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
        else
        {
            _source.Play();
            _canPlay = false;
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
        _animator.SetBool(Grabbing, false);
        
        switch (_instHold){
            case DragAndDrop.Type.BassGrid:
            case DragAndDrop.Type.Bass:
                _animator.SetTrigger(Bass);
                break;
            case DragAndDrop.Type.GuitarGrid:
            case DragAndDrop.Type.Guitar:
                _animator.SetTrigger(Guitar);
                break;
            case DragAndDrop.Type.DrumGrid:
            case DragAndDrop.Type.Drums:
                _animator.SetTrigger(Drums);
                break;
            case DragAndDrop.Type.Keytar:
            case DragAndDrop.Type.KeytarGrid:
                _animator.SetTrigger(Keytar);
                break;
            case DragAndDrop.Type.Null:
                _animator.SetTrigger(Idle);
                break;
            default:
                Debug.LogError("INVALID TYPE GIVEN");
                break;
        }
    }

    private void CalculateDistance(Vector2 mousePos)
    {
        if ((new Vector2(transform.position.x, transform.position.y) - mousePos).magnitude < 2 && !_grabbing)
        {
            _grabbing = true;
            _animator.SetBool(Grabbing, true);
        } else if ((new Vector2(transform.position.x, transform.position.y) - mousePos).magnitude > 2 && _grabbing)
        {
            _grabbing = false;
            _animator.SetBool(Grabbing, false);
        }
    }
}
