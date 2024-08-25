using System.Collections;
using System.Collections.Generic;
using NOOD.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class HoverAndPress : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public UnityAction OnHover;
        public UnityAction OnPress;

        [SerializeField] private bool _hover, _press;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_hover) return;
            OnHover?.Invoke();
            SoundManager.PlaySound(SoundEnum.BtnHover);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_press) return;
            SoundManager.PlaySound(SoundEnum.BtnClick);
        }
    }
}
