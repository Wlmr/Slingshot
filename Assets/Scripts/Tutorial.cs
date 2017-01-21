using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour {

    public Button tutorialButton;

    public Camera cam;
    public RectTransform textComposite;

    private Vector2 lastPos;
    private float angleOfBurnPoint;

    private Vector2 triggerVector;
    public Transform player;
    public Transform startCelestial;
    private int currentTextIndex;

    public float threshold;
    public Text[] tutorialTexts;
    public GameObject textBackground;
    public PlayerWithGravity playerWithGravitySC;
    public IPlantCelestials iPlantCelestialsSC;
    public overlordScript overlordSC;

    private float dampSpeed = 6f;

    private bool transitioning, readyToMoveOn;
    

    // Use this for initialization
    void Start () {
        PositionTexts();
        currentTextIndex = 0;
        transitioning = readyToMoveOn = false;
        if (overlordSC.NoPlayerPrefsKey("notFirstTime")) { //if it is first time playing, double negation
            playerWithGravitySC.tutorialActive = true;
            PlayerPrefs.SetInt("notFirstTime", 1);
            PlayerPrefs.SetInt("notFirstRevive", 0);
            updateTutorialText();
            overlordSC.ActivateMenu(false);
        }
    }

    void PositionTexts() {
        textComposite.position = cam.WorldToScreenPoint(startCelestial.position);
    }

    void getPointOfBurn() {
        Vector2 radius = player.position - startCelestial.position;
        Vector2 startCelestialPos = startCelestial.position;
        Vector2 secondCelestialPos = iPlantCelestialsSC.celestialsQueue.Peek().transform.position;
        Vector2 triggerPos = startCelestialPos - secondCelestialPos;
        triggerPos += (triggerPos.normalized * radius.magnitude);
        triggerVector = triggerPos;
    }
    

    void updateTutorialText() {
        if (currentTextIndex == 0) { // First time, activated by tutorialbutton.
            tutorialButton.gameObject.SetActive(true);
            tutorialTexts[0].gameObject.SetActive(true);
            textBackground.SetActive(true);
            currentTextIndex++;
        } else if(currentTextIndex == tutorialTexts.Length) { // Last case, when tutorial finishes.
            tutorialTexts[currentTextIndex-1].gameObject.SetActive(false);
            textBackground.SetActive(false);
        } else if (currentTextIndex == tutorialTexts.Length - 1) { // Start breaking.
            ShowNextText();
            getPointOfBurn();
            transitioning = true;
            tutorialButton.gameObject.SetActive(false);
        } else {
            ShowNextText();
        }
    }

    private void ShowNextText() {
        tutorialTexts[currentTextIndex-1].gameObject.SetActive(false);
        tutorialTexts[currentTextIndex].gameObject.SetActive(true);
        currentTextIndex++;
    }

    

    public void activateTutorial() {
        overlordSC.showAd();
        PlayerPrefs.SetInt("notFirstTime", 0);
        PlayerPrefs.SetInt("notFirstRevive", 0);
        if (overlordSC.NoPlayerPrefsKey("restarted") && overlordScript.fuckedUp == false) { //first round after launching app shouldnt reload scene
            PlayerPrefs.SetInt("restarted", 1);
            overlordSC.ActivateMenu(false);
            Start();
        } else {
            PlayerPrefs.SetInt("Musiken", overlordSC.musiken.mute == false ? 1 : 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }




    public void tutorialButtonClicked() {
        updateTutorialText();
    }
	

	// Update is called once per frame
	void Update() {
        if (readyToMoveOn) {
            if(playerWithGravitySC.inputPresent) {
                textBackground.SetActive(false);
                playerWithGravitySC.tutorialActive = false;
                Time.timeScale = 6f;
                updateTutorialText();
                gameObject.SetActive(false);

            }
        } else if (transitioning) {
            float angle = Vector2.Angle((player.position - startCelestial.position), triggerVector);
            float dTBurnPoint = (playerWithGravitySC.orbitalPeriod * (angle / 360f))/6;
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1f,ref dampSpeed, dTBurnPoint);
            if (Mathf.Abs(angle) < threshold) {
                Time.timeScale = 0f;
                readyToMoveOn = true;            
            }
        }
    }
}
