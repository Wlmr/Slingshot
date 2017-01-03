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
    public Text[] tutorialTexts = new Text[2];
    public PlayerWithGravity playerWithGravitySC;
    public IPlantCelestials iPlantCelestialsSC;
    public overlordScript overlordSC;

    private float dampSpeed = 6f;

    private bool readyForFirstText, transitioning, readyToMoveOn;
    

    // Use this for initialization
    void Start () {
        currentTextIndex = 0;
        readyForFirstText = transitioning = readyToMoveOn = false;
       
        if (isFirstTimePlaying()) {
            tutorialButton.gameObject.SetActive(true);
            readyForFirstText = true;
            playerWithGravitySC.tutorialActive = true;
            PlayerPrefs.SetInt("firstTime", 0);
        }

    }

    void getPointOfBurn() {
        Vector2 radius = player.position - startCelestial.position;
        Vector2 startCelestialPos = startCelestial.position;
        Vector2 secondCelestialPos = iPlantCelestialsSC.celestialsQueue.Peek().transform.position;
        Vector2 triggerPos = startCelestialPos - secondCelestialPos;
        triggerPos += (triggerPos.normalized * radius.magnitude);
        triggerVector = triggerPos;                              //probably wont be neccessary
    }


    void firstTutorialText() {
        textBackground.SetActive(true);
        activateTutorialText(true);
        getPointOfBurn();
        
    }

    void secondTutorialText() {
        
    }

    void activateTutorialText(bool boolean) {
        tutorialTexts[currentTextIndex].gameObject.SetActive(boolean);
    }

    bool isFirstTimePlaying() {
        return !PlayerPrefs.HasKey("firstTime");
    }

    public void activateTutorial() {
        if (PlayerPrefs.HasKey("firstTime")) {
            PlayerPrefs.DeleteKey("firstTime");
            //PlayerPrefs.Save();
        }
        overlordSC.Restarter();  
    }

    public void tutorialButtonClicked() {
        if (currentTextIndex == 0) {

            transitioning = true;
            activateTutorialText(false);
            currentTextIndex++;
            activateTutorialText(true);
        } else if (currentTextIndex == 1) {
            secondTutorialText();

            tutorialButton.gameObject.SetActive(false);
        }
    }
	

   

	// Update is called once per frame
	void Update() {
        if (readyToMoveOn) {
            if(Input.touchCount > 0 || Input.anyKey) {
                playerWithGravitySC.tutorialActive = false;
                Time.timeScale = 6f;
                activateTutorialText(false);
                textBackground.SetActive(false);
                //currentTextIndex++;
                //activateTutorialText(true);
                gameObject.SetActive(false);
            }
        } else if (readyForFirstText) {
            firstTutorialText();
            readyForFirstText = false;
        } else if (transitioning) {
            float angle = Vector2.Angle((player.position - startCelestial.position), triggerVector);
          
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1f,ref dampSpeed, 0.05f);
            Debug.Log(Time.timeScale);
            if (Mathf.Abs(angle) < threshold) {
                activateTutorialText(true);
                Time.timeScale = 0f;
                if(currentTextIndex == 1) {
                    readyToMoveOn = true;
                }
            }
        }
    }
}
