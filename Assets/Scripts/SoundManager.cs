using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] float _bpm;
    public static Action onBeat;
    public float bpm => _bpm;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("More then one sound manager");
        float bpmSeconds = 60 / _bpm;
        InvokeRepeating("HitBeat", 1, bpmSeconds );
    }

    private void HitBeat()
    {
        onBeat?.Invoke();
    }
}
