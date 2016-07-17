using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class overlordScript : MonoBehaviour {

    public Transform background;
    //public Transform cam;
    public Camera cam;
    public float screenLowerBoundary;

    private GameObject[] celestials;
    public int universeSpeed;

    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;

    public float xRndRange;
    
    public float yDistance;
    private Vector2 prefabPos;

    private float xRnd;
    private float yRnd;

    public Transform player;

    private int playerStartPosY;

    //private int score;
    //private int distanceTravelled;

    public Queue<GameObject> celestialsQueue;

    private int counter;

    public bool fuckedUp;

    /*TODO:
    */

    // Use this for initialization
    void Start() {
        celestialsQueue = new Queue<GameObject>();

        counter = 1;

        prefabPos = Vector2.zero;
        for (int i = 0; i < 5; i++) { 
            InstansiateCelestial();
        }
        fuckedUp = false;
	}

    // Update is called once per frame
    void FixedUpdate() {
        background.Rotate(Vector3.forward / 30);
        if (PlayerWithGravity.startCamMovment && !fuckedUp) { cam.transform.Translate(Vector3.up / 2000 * universeSpeed); }
        screenLowerBoundary = cam.ScreenToWorldPoint(new Vector2(0, 0)).y;
    }



    //MAKE IT CHOSE RANDOMLY WHICH CELESTIAL TO CHOSE
    public void InstansiateCelestial() {
        GetRandomValues();
        GameObject obj = (GameObject) Instantiate(prefab1, prefabPos, Quaternion.identity);
        obj.name = "prefab" + counter;
        celestialsQueue.Enqueue(obj);
        counter++;
        
    }

    void GetRandomValues() {
        xRnd = Random.Range(-xRndRange, xRndRange);
        //yRnd = Random.Range(yMin, yMax);
        prefabPos.y += yDistance;
        prefabPos.x = xRnd;
    }

	
	
    public void Restarter ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

