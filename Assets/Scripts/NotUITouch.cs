using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class NotUITouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private Vector2 prevPoint;
    public PlayerWithGravity playerWithGravitySC;

    //public void OnPointerClick(PointerEventData data) {
    //    Debug.Log("FINGER DOWN");
    //    playerWithGravitySC.inputPresent = true;
    //}

    public void OnPointerUp(PointerEventData data) {
        playerWithGravitySC.inputPresent = false;
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        playerWithGravitySC.inputPresent = true;
    }
}