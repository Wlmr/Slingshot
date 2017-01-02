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
    public static int adInterval = 5;
    public GameObject[] menuButtons = new GameObject[3];
    public AudioSource musiken;
  


    void Start() {
        fuckedUp = false;
    }

    public void Crash() {
        fuckedUp = true;
        crashCounter++;
        showAd();
        ActivateMenu();
    }

    void ActivateMenu() {
        foreach(GameObject g in menuButtons) {
            g.SetActive(true);
        }
    }

    public static void showAd() {
        if (crashCounter % adInterval == 0 && Advertisement.IsReady()) {
            Advertisement.Show();
        }
    }
    
    void FixedUpdate() {
        //background.Rotate(Vector3.forward / 30);   
        screenLowerBoundary = cam.ScreenToWorldPoint(new Vector2(0, 0)).y;
    }
    public void Restarter (){
        PlayerPrefs.SetInt("Musiken", musiken.mute == true ? 0 : 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

