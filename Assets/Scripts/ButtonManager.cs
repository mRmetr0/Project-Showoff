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

   public static Action onPlay;
   public static Action onStop;

   private void Awake()
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
         onStop?.Invoke();
         _image.sprite = startSprite;
      }
      else
      {
         onPlay?.Invoke();
         _image.sprite = stopSprite;
      }

      _playing = !_playing;
   }

   public void SetButtonActive(bool active, bool force = false)
   {
      if (!active && !force)
      {
         foreach (Monster monster in Monster.monsters)
         {
            if (monster.InstHold != DragAndDrop.Type.Null) 
               return;
         }
      }

      playButton.interactable = active;
      playButton.gameObject.SetActive(active);
   }
}
