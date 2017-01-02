using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class toggleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey("Musiken")) {
            GetComponent<Toggle>().isOn = (PlayerPrefs.GetInt("Musiken") == 1) ? false : true;
        }
    }
}
