using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class overlordScript : MonoBehaviour {

    public Transform background;
    public Transform cam;
    private GameObject[] celestials;
    

    /*TODO:
    1.      SÄTT ETT SCRIPT PÅ VARJE CELESTIAL SOM SLÅR OM TAG SÅ FORT DEN KOMMER UR ELLER IN I BILD.  I BILD: "celestial" — UTANFÖR BILD: "nonactive celestial"
    */

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        background.Rotate(Vector3.forward/50);
        if (PlayerWithGravity.startCamMovment) { cam.Translate(Vector3.up / 2000); }
        
        
	}

    public void Restarter ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

