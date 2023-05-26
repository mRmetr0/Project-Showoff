using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
   public static ButtonManager instance;
   
   private bool _playing = false;

   public static Action OnPlay;
   public static Action OnStop;

   private void Awake ()
   {
      if (instance == null)
         instance = this;
      else 
         Debug.LogError("Found two button managers in one scene!");
   }

   public void StartStop()
   {
      if (_playing)
         OnStop?.Invoke();
      else 
         OnPlay?.Invoke();

      _playing = !_playing;
   }
}
