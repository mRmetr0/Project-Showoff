using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Monster : MonoBehaviour
{
    private Collider2D collider;
    private AudioSource source;
    [SerializeField]private SpriteRenderer instrument;
    private void Start()
    {
        //instrument = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        Debug.Log(instrument);
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
    
    public void SetClip(AudioClip pClip)
    {
        List<Monster> monsters = SoundManager.instance.GetActiveMonsters();
        if (!monsters.Contains(this))
            monsters.Add(this);
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
