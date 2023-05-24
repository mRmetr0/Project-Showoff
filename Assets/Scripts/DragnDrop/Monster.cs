using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Monster : MonoBehaviour
{
    [SerializeField] private SpriteRenderer instrument;
    [Space(5)][Header ("Instrument Audioclips:")]
    [SerializeField] private AudioClip drums;
    [SerializeField] private AudioClip trumpet;
    [SerializeField] private AudioClip guitar;
    [SerializeField] private AudioClip keytar;
    
    private Collider2D collider;
    private AudioSource source;
    
    private void Start()
    {
        //instrument = this.gameObject.GetComponentInChildren<SpriteRenderer>();
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
        if (Input.GetMouseButtonDown(0) && collider == Physics2D.OverlapPoint(mousePos))
        {
            Reset();
        }
    }

    private void Reset()
    {
        this.GetComponent<SpriteRenderer>().color = Color.white;
        instrument.sprite = null;
        source.Stop();
        source.clip = null;
    }
    
    public void SetInstrument(DragAndDrop.Type inst)
    {
        switch (inst)
        {
            case(DragAndDrop.Type.Drums):
                source.clip = drums;
                break;
            
            case(DragAndDrop.Type.Trumpet):
                source.clip = trumpet;
                break;
            
            case(DragAndDrop.Type.Guitar):
                source.clip = guitar;
                break;
            
            case(DragAndDrop.Type.Keytar):
                source.clip = keytar;
                break;
        }
    }
    
    public void SetClip(AudioClip pClip)
    {
        source.clip = pClip;
    }

    public SpriteRenderer GetInstrument()
    {
        return instrument;
    }

    private void StartTrack()
    {
        if (source.clip != null)
            source.Play();
    }

    private void StopTrack()
    {
        source.Stop();
    }
}
