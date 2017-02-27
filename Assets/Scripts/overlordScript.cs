using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class overlordScript : MonoBehaviour {

    public Camera cam;
    public float screenLowerBoundary;
    //public int universeSpeed;
    private int playerStartPosY;
    public static bool fuckedUp;
    public static int crashCounter = 1;
    public int adInterval;
    private static int adIntervalPrivate;
    public GameObject menuButtons;
    public GameObject resumeButton;
    public AudioSource musiken;

    public HighScore HighScoreSC;
    public PlayerWithGravity playerWithGravitySC;
    public Camera2DFollow Camera2DFollowSC;
    public Button revive;
    public GameObject reviveExp;

    private int restarted;
    

    void Start() {
        restarted = PlayerPrefs.GetInt("restarted");
        Debug.Log(PlayerPrefs.GetInt("restarted"));
        if (adIntervalPrivate == 0) adIntervalPrivate = Random.Range(adInterval - 2, adInterval + 2);
        CheckForRestart();
        fuckedUp = false;
        showAd();
        


    }


    

    private void HighScoreSetup() {

    }

    public void ActivateMenu(bool boolean) {
        menuButtons.SetActive(boolean);
    }


    public void Crash(bool highScore, bool reviveable) {
        fuckedUp = true;
        crashCounter++;
        // Debug.Log("crashes: " + crashCounter);
        ActivateMenu(true);
        if (highScore) {
            HighScoreSC.CongratulateNewHighScore(true);
        }
        if (reviveable && crashCounter % (adIntervalPrivate / 2) == 0) {
            revive.gameObject.SetActive(true);
            if (NoPlayerPrefsKey("notFirstRevive")) {
                reviveExp.SetActive(true);
                PlayerPrefs.SetInt("notFirstRevive", 1);
            }
        }


    }

    private void Revive() {
        playerWithGravitySC.gameObject.SetActive(true);
        fuckedUp = false;
        playerWithGravitySC.Start();
        Camera2DFollowSC.SetTarget(playerWithGravitySC.bodyBeingOrbited.transform.position);
        revive.gameObject.SetActive(false);
        reviveExp.SetActive(false);
        HighScoreSC.CongratulateNewHighScore(false);
        PlayerPrefs.SetInt("restarted", 1);
    }

    public void ShowRewardedAd() {
        if (Advertisement.IsReady("rewardedVideo")) {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
               // Debug.Log("The ad was successfully shown.");
                Revive();
                break;
            case ShowResult.Skipped:
              //  Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                //Debug.LogError("The ad failed to be shown.");
                break;
        }
    }


public void showAd() {
        if (crashCounter % (adIntervalPrivate) == 0 && Advertisement.IsReady()) {
            Advertisement.Show();
            adIntervalPrivate = Random.Range(adInterval - 2, adInterval + 2);
            crashCounter = 1;
        }
    }
    
    void FixedUpdate() {
        screenLowerBoundary = cam.ScreenToWorldPoint(new Vector2(0, 0)).y;
    }

    public void CheckForRestart() {
        if (NoPlayerPrefsKey("veryFirstTime") || PlayerPrefs.GetInt("restarted") == 1) { //if played for the very first time tutorial should be played
            ActivateMenu(false);
        } else {
            ActivateMenu(true);
        }
    }

    public bool NoPlayerPrefsKey(string key) {
        return (!PlayerPrefs.HasKey(key)||PlayerPrefs.GetInt(key) == 0 );
    }

    public void PlayButton (){
        ActivateMenu(false);
        HighScoreSC.Show(false);
        if (NoPlayerPrefsKey("veryFirstTime")) { //allra första gången efter tutorial
            PlayerPrefs.SetInt("veryFirstTime", 1);
            PlayerPrefs.SetInt("restarted", 1);
        }
        if (NoPlayerPrefsKey("restarted")) { //first round after launching app shouldnt reload scene
            PlayerPrefs.SetInt("restarted", 1);
        } else {
            PlayerPrefs.SetInt("Musiken", musiken.mute == false ? 1 : 0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Resume() {
        PlayerPrefs.SetInt("restarted", 1);
        Time.timeScale = playerWithGravitySC.nrmlTime;
        fuckedUp = false;
        resumeButton.SetActive(false);
    }

    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus && PlayerWithGravity.startCamMovment) {
            PlayerPrefs.SetInt("restarted", 0);
            PlayerPrefs.Save();
            if (!fuckedUp) {
                resumeButton.SetActive(true);
                Time.timeScale = 0;
                fuckedUp = true;
            }
        } else if (pauseStatus) {
            PlayerPrefs.SetInt("restarted", 1);
        }else if(!pauseStatus) {
            PlayerPrefs.SetInt("restarted", 1);
        }
    }

    void OnApplicationQuit() {
        PlayerPrefs.SetInt("restarted", 0);
    }
        //Debug.Log("quitted");
}


