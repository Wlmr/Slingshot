using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {

    private Vector2 lastPos;
    private float angleOfBurnPoint;

    private Vector2 triggerPoint;
    public Transform player;
    public Transform startCelestial;
    private int currentTextIndex = 0;

    public Text[] tutorialTexts = new Text[3];
    public PlayerWithGravity playerWithGravitySC;
    public IPlantCelestials iPlantCelestialsSC;
    public overlordScript overlordSC;
    float normalTimeScale;

    private bool readyForFirstText, transitioning, c;
    

    // Use this for initialization
    void Start () {
        readyForFirstText = transitioning = c = false;
        normalTimeScale = Time.timeScale;
        if (isFirstTimePlaying()) {
            readyForFirstText = true;
            playerWithGravitySC.tutorialActive = true;
            PlayerPrefs.SetInt("firstTime", 0);
        }
    }

    void getAngleOfBurn() {
        Vector2 radius = player.position - startCelestial.position;
        Vector2 startCelestialPos = startCelestial.position;
        Vector2 secondCelestialPos = iPlantCelestialsSC.celestialsQueue.Peek().transform.position;
        Vector2 triggerPos = startCelestialPos - secondCelestialPos;
        triggerPos += (triggerPos.normalized * radius.magnitude);
        //triggerPoint = triggerPos;                              //probably wont be neccessary
        Vector2.Angle(triggerPos, Vector2.down);
    }


    void firstTutorialText() {
        activateTutorialText(true);
        normalTimeScale = playerWithGravitySC.nrmlTime;
        playerWithGravitySC.nrmlTime = 1.0F;
        Time.timeScale = 1.0F;
        //activateTutorialText(false);
        //Time.timeScale = normalTimeScale;
        getAngleOfBurn();
    }

    void secondTutorialText() {
        transitioning = true;
        lastPos = player.transform.position;
    }

    bool closestToTriggerPoint() {
        return true;
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
        }
        overlordSC.Restarter();  
    }

    public void tutorialButtonClicked() {
        switch (currentTextIndex) {
            case 0:
                activateTutorialText(false);
                currentTextIndex++;
                break;
            case 1:
                secondTutorialText();
                break;
            case 2:
                break;
            default:
                break;


        }
        
    }
	
	// Update is called once per frame
	void Update() {
         if (readyForFirstText) {
            firstTutorialText();
            readyForFirstText = false;    
        }else if (transitioning) {

        }  
    }
}
