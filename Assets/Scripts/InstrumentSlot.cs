using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentSlot : MonoBehaviour
{
    [SerializeField] private DragAndDrop.Type type;
    public DragAndDrop.Type Type => type;
}
