using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
   public static ButtonManager instance;
   [SerializeField] private Button playButton;
   [SerializeField] private Sprite startSprite, stopSprite;

   private Image _image;
   private bool _playing = false;

   public static Action OnPlay;
   public static Action OnStop;

   private void Awake ()
   {
      if (instance == null)
      {
         instance = this;
         _image = playButton.GetComponent<Image>();
      }
      else 
         Debug.LogError("Found two button managers in one scene!");
   }

   public void StartStop()
   {
      if (_playing)
      {
         OnStop?.Invoke();
         _image.sprite = startSprite;
      }
      else
      {
         OnPlay?.Invoke();
         _image.sprite = stopSprite;
      }
      _playing = !_playing;
   }
}
