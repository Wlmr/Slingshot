using UnityEngine;
using System.Collections;

public class celestialScript : MonoBehaviour {

    float rotationalSpeed;
    public int mass;

   // GameObject player;
    CircleCollider2D col;

    private overlordScript overlord;


	// Use this for initialization
	void Start () {
        rotationalSpeed = Random.Range(-3, 3);
        mass = 150000;
        col = GetComponent<CircleCollider2D>();
        overlord = GameObject.Find("Overlord").GetComponent<overlordScript>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.Rotate(Vector3.forward * rotationalSpeed/50);
        if ((this.transform.position.y + col.radius) < overlord.screenLowerBoundary) { 
            Destroy(this.gameObject);
        }
    }



}
