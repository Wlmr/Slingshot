using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FindMusiken : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey("Musiken")) {
            GetComponent<AudioSource>().mute = (PlayerPrefs.GetInt("Musiken") == 1) ? false : true;
        }  
	}
}
