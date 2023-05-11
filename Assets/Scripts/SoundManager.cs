using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private List<Monster> monsters = new List<Monster>();
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
            Debug.LogError("More then one manager present");
    }

    public List<Monster> GetActiveMonsters()
    {
        return monsters;
    }
}
