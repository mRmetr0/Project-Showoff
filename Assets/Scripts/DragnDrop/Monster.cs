using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Monster : MonoBehaviour
{
    [SerializeField] private SpriteRenderer instrument;
    [Space(5)][Header ("Instrument AudioClips:")]
    [SerializeField] private AudioClip drums;
    [SerializeField] private AudioClip trumpet;
    [SerializeField] private AudioClip guitar;
    [SerializeField] private AudioClip keytar;
    [SerializeField] private AudioClip keytarGrid;
    
    private Collider2D collider;
    private AudioSource source;
    private DragAndDrop.Type InstHold = DragAndDrop.Type.Null;
    
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
        collider = GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();
        source.loop = true;
        
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
        if (Input.GetMouseButtonDown(0) && collider == Physics2D.OverlapPoint(mousePos) && _clickable && !MusicGrid.instance.Interactable)
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
        instrument.sprite = null;
        source.clip = null;
        InstHold = DragAndDrop.Type.Null;
        _canPlay = false;
    }
    
    public void SetInstrument(DragAndDrop.Type inst)
    {
        InstHold = inst;
    }

    public SpriteRenderer GetInstrument()
    {
        return instrument;
    }

    private void StartTrack()
    {
        switch (InstHold)
        {
            case(DragAndDrop.Type.Drums):
                source.clip = drums;
                break;
            case(DragAndDrop.Type.Bass):
                source.clip = trumpet;
                break;
            case(DragAndDrop.Type.Guitar):
                source.clip = guitar;
                break;
            case(DragAndDrop.Type.Keytar):
                source.clip = keytar;
                break;
            case (DragAndDrop.Type.KeytarGrid):
                source.clip = null;
                SetToPlay();
                break;
        }
        if (source.clip != null)
            source.Play();

        _clickable = false;
    }

    private void PlayGrid()
    {
        if (_canPlay)
        {
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
        source.Stop();
        _canPlay = false;
        _clickable = true;
    }
}
