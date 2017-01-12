using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public Button tutorialButton;

    public GameObject textBackground; 

    private Vector2 lastPos;
    private float angleOfBurnPoint;

    private Vector2 triggerVector;
    public Transform player;
    public Transform startCelestial;
    private int currentTextIndex;

    public float threshold;
    public Text[] tutorialTexts = new Text[3];
    public PlayerWithGravity playerWithGravitySC;
    public IPlantCelestials iPlantCelestialsSC;
    public overlordScript overlordSC;

    private float dampSpeed = 6f;

    private bool transitioning, readyToMoveOn;
    

    // Use this for initialization
    void Start () {
        currentTextIndex = 0;
        transitioning = readyToMoveOn = false;
        if (isFirstTimePlaying()) {
            playerWithGravitySC.tutorialActive = true;
            PlayerPrefs.SetInt("firstTime", 0);
            updateTutorialText();
        }
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

    bool isFirstTimePlaying() {
        return !PlayerPrefs.HasKey("firstTime");
    }

    public void activateTutorial() {
        if (PlayerPrefs.HasKey("firstTime")) {
            PlayerPrefs.DeleteKey("firstTime");
        }
        overlordSC.PlayButton();  
    }

    public void tutorialButtonClicked() {
        updateTutorialText();
    }
	

	// Update is called once per frame
	void Update() {
        if (readyToMoveOn) {
            if(Input.touchCount > 0 || Input.anyKey) {
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
