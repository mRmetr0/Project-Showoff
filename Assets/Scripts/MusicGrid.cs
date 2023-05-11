using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicGrid : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [Min(0.0f)] [SerializeField] private double bpm;
    [SerializeField] private int length;
    [SerializeField] private bool play;
    [SerializeField] private Instrument[] instruments;
    
    private AudioSource _source;
    private float _time = 0;
    private int _beat = 0;

    private double _nextTick = 0.0f;
    private double _sampleRate = 0.0f;
    private bool _ticked = false;


    private void Start()
    {
        _source = GetComponent<AudioSource>();
        
        double startTick = AudioSettings.dspTime;
        _sampleRate = AudioSettings.outputSampleRate;
        _nextTick = startTick + (60.0 / bpm);

        bool[] notes = new bool[] { true, false, false, true };
    }

    private void LateUpdate()
    {
        block.gameObject.SetActive(false);
        
        if (_ticked && _nextTick >= AudioSettings.dspTime)
        {
            _ticked = false;
            OnTick();
        }
    }

    private void FixedUpdate()
    {
        
        double timePerTick = 60 / bpm;
        double dspTime = AudioSettings.dspTime;
        while (dspTime >= _nextTick)
        {
            _ticked = true;
            _nextTick += timePerTick;
        }
    }

    private void OnTick()
    {

        for (int i = instruments.Length - 1; i >= 0; i--)
        {
            bool[] note = instruments[i].testNotes;
            if (note[_beat])
            {
                block.gameObject.SetActive(true);
                _source.PlayOneShot(instruments[i].sound);
            }
        }

        _beat++;
        if (_beat > 4 * length - 1) _beat = 0;
    }
}
