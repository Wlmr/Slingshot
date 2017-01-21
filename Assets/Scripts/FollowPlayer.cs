using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
    public Transform player;

	// Use this for initialization
	void Start () {
        transform.position = player.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (player.gameObject.activeSelf) {
            transform.position = player.position;
        }
    }
}
