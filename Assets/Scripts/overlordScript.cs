using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class overlordScript : MonoBehaviour {

    public Transform background;
    public Camera cam;
    public float screenLowerBoundary;
    //public int universeSpeed;
    private int playerStartPosY;
    public static bool fuckedUp;
 
  
    
    
    void Start() {
        fuckedUp = false;
    }
    
    void FixedUpdate() {
        //background.Rotate(Vector3.forward / 30);   
        screenLowerBoundary = cam.ScreenToWorldPoint(new Vector2(0, 0)).y;
    }
    public void Restarter (){ SceneManager.LoadScene(SceneManager.GetActiveScene().name);}
}

