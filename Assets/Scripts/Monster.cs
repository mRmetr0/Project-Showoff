using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Monster : MonoBehaviour
{
    private Collider2D collider;
    private AudioSource source;
    private AudioClip clip = null;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {   
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && collider == Physics2D.OverlapPoint(mousePos))
        {
            Reset();
        }

        List<Monster> monsters = SoundManager.instance.GetActiveMonsters();
        if (monsters.Count > 0 && !monsters[0].source.isPlaying && clip != null)
        {
            source.Stop();
            source.PlayOneShot(clip);
        }
    }

    private void Reset()
    {
        SoundManager.instance.GetActiveMonsters().Remove(this);
        this.GetComponent<SpriteRenderer>().color = Color.white;
        source.Stop();
        clip = null;
    }
    
    public void SetClip(AudioClip pClip)
    {
        List<Monster> monsters = SoundManager.instance.GetActiveMonsters();
        if (!monsters.Contains(this))
            monsters.Add(this);
        source.Stop();
        clip = pClip;
    }
}
