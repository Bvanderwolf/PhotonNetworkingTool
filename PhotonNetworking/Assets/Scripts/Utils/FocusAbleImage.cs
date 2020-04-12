namespace Utils
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System;

    public class FocusAbleImage : Selectable
    {
        private Action<GameObject> PointerEntered;

        private Action<GameObject> PointerExited;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            PointerEntered?.Invoke(this.gameObject);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            PointerExited?.Invoke(this.gameObject);
        }

        public void AddListeners(Action<GameObject> OnEntered, Action<GameObject> OnExited)
        {
            PointerEntered += OnEntered;
            PointerExited += OnExited;
        }

        public void RemoveListeners()
        {
            PointerEntered = null;
            PointerExited = null;
        }
    }
}