using UnityEngine;
using System.Collections;

public class celestialScript : MonoBehaviour {

    float rotationalSpeed;
    public int mass;

    GameObject player;
    CircleCollider2D col;

    private overlordScript overlord;


	// Use this for initialization
	void Start () {
        rotationalSpeed = Random.Range(1, 3);
        player = GameObject.Find("player");
        mass = 150000;
        col = GetComponent<CircleCollider2D>();
        overlord = GameObject.Find("OVERLORD").GetComponent<overlordScript>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        transform.Rotate(Vector3.forward * rotationalSpeed/100);
        if ((gameObject.transform.position.y + col.radius) < overlord.screenLowerBoundary) {
            Destroy(gameObject);
        }
    }



}
