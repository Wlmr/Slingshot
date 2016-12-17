using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public Transform player;
    private Transform startCelestial;
    
    // Use this for initialization
    void Start () {
        float startTime = Time.time;
        if (isFirstTimePlaying()) {
            PlayerPrefs.SetInt("firstTime", 0);
        }else {
            
        }
    }

    bool isFirstTimePlaying() {
        return !PlayerPrefs.HasKey("firstTime");
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}
