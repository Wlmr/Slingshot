using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour {
    private Toggle toggle;

    void Start() {
        toggle = GetComponent<Toggle>();
        if (PlayerPrefs.HasKey("Musiken")) {
            toggle.isOn = (PlayerPrefs.GetInt("Musiken") == 1) ? false : true;
        }
    }
}