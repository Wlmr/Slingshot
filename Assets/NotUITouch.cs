using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NotUITouch : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    private Vector2 prevPoint;
    private Vector2 newPoint;
    private Vector2 screenTravel;

    public void OnPointerDown(PointerEventData data) {
        Debug.Log("FINGER DOWN");
        prevPoint = data.position;
    }

    public void OnDrag(PointerEventData data) {
        newPoint = data.position;
        screenTravel = newPoint - prevPoint;
        _processSwipe();
    }

    public void OnPointerUp(PointerEventData data) {
        Debug.Log("FINEGR UP...");
    }

    private void _processSwipe() {
        // your code here
        Debug.Log("screenTravel left-right.. " + screenTravel.x.ToString("f2"));
    }

}