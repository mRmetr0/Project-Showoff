using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName="InstrumentValue", menuName= "Values/InstrumentValue")]
public class Instrument : ScriptableObject
{
    public AudioClip sound;
    public Color color;
    public Sprite sprite;
    public bool[] testNotes;
}
