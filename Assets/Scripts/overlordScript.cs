using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class overlordScript : MonoBehaviour {

    public Transform background;
    public Camera cam;
    public float screenLowerBoundary;
    //public int universeSpeed;
    private int playerStartPosY;
    public static bool fuckedUp;
    public static int crashCounter = 0;
    public int adInterval;
    public GameObject menuButtons;
    public AudioSource musiken;
    public bool menuButtonsActive;
  

    void Start() {
        FirstPlay();
        fuckedUp = false;
        
    }
    

    public void Crash() {
        fuckedUp = true;
        crashCounter++;
        showAd();
        ActivateMenu(true);
    }

    bool ActivateMenu(bool boolean) {
        menuButtons.SetActive(boolean);
        return boolean;
    }

    public void showAd() {
        if (crashCounter % adInterval == 0 && Advertisement.IsReady()) {
            Advertisement.Show();
        }
    }
    
    void FixedUpdate() {
        //background.Rotate(Vector3.forward / 30);   
        screenLowerBoundary = cam.ScreenToWorldPoint(new Vector2(0, 0)).y;
    }

    public void FirstPlay() {
        if (!PlayerPrefs.HasKey("restarted")) {
        } else {
            PlayerPrefs.DeleteKey("restarted");
            menuButtonsActive = ActivateMenu(false);
        }
    }

    public void PlayButton (){
        if (!PlayerPrefs.HasKey("restarted")) {
            PlayerPrefs.SetInt("restarted", 1);
            Debug.Log("hore");
            menuButtonsActive= ActivateMenu(false);
        } else {
            PlayerPrefs.SetInt("Musiken", musiken.mute == false ? 1 : 0);
            PlayerPrefs.SetInt("restarted", 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

