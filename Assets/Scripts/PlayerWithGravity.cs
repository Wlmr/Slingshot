using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerWithGravity : MonoBehaviour {

    

    //OTHER GAMEOBJECTS REFERENCED
    GameObject overlord;
    public overlordScript overlordSC;
    public IPlantCelestials IPlantCelestialsSC;
    Camera2DFollow camera2DFollowSC;
    public SatifactorySounds satisfactorySoundsSC;
    public HighScore HighScoreSC;
    //Camera mainCam;
    public GameObject retryButton;


    public float radiusSetter;
    private float radius;
    public float grejtRadiusTreshold;

    private Transform crashCelestial;
    private LineRenderer trajectoryLine;
    public GameObject bodyBeingOrbited;
    private Rigidbody2D playerRigidbody;
    private int celestialMass;

  

    private int burnCounter = 0;
    private bool firstBurn = true;
    private bool burning = false;
    private Vector2 speed;
    public float orbitalPeriod;
    private float grvtyPull;
    public float nrmlTime;
    private readonly float grvtyCnst = 0.000002f;
    private Vector2 directionVectorTowardsCelestial;


    //for DotifyTrajectory()
    public Vector3[] trajectoryPoints;
    public int simplify;
    public int maxCount;
    private float playerWeight;
    private int nbrOfTrajectoryPoints;
    private int privateMaxCount;
    private float angle;
    private float dt;
    private Vector3 s;
    //private Vector3 lastS;
    private Vector3 v;
    private Vector3 a;
    private float tempAngleSum;
    private int step;
    private bool newChange = true;

    //for CelestialInsideTrajectory()/TryEstablishNewOrbit()
    public GameObject oldCelestial;
    private GameObject successfulCelestial;

    public float threshold;
    bool celestialInside;
    private Vector3 pointOfBurn;

    //TryEstablishNewOrbit

    float requiredVelocity;
    Vector2 lastPos;
    Vector2 tempPos;
    bool awatingTransition = false;
    Queue<Vector3> previousPositions;

    
    static public bool startCamMovment;
    public Text scoreText;
    private int score;
    public float thrust;


    public Slider fuel;

    private bool outOfFuel;

    public int firstTimePlaying;
    public bool tutorialActive;

    //CHANGE IF YOU CHANGE THE SIZE OF THE CELESTIALS
    public float minApoapsis;
    public float maxApoapsis;
    public int maxBurns;
    public int minBurns;

    public bool inputPresent;

    public ParticleSystem explosion;



    /*
    TODO:
        2.        PROPOSITION: Divide the force over several frames so that it looks as if it is burning opposite? - when deaccelerating
        3.          
        5.        möjligen en dynamisk dt variabel med inverst förhållande till velocity
   

    */

    void Awake() {
        camera2DFollowSC = GameObject.Find("Main Camera").GetComponent<Camera2DFollow>();
        overlord = GameObject.Find("Overlord");
        overlordSC = overlord.GetComponent<overlordScript>();
        bodyBeingOrbited = GameObject.FindGameObjectWithTag("orbitingCelestial");                      //initial orbit around celestial with tag "startOrbit" now called bodyBeingOrbited
        oldCelestial = bodyBeingOrbited;
        trajectoryLine = GetComponent<LineRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerWeight = playerRigidbody.mass;
        celestialMass = bodyBeingOrbited.GetComponent<celestialScript>().mass;
        score = 0;
    }

    public void Start() {
        //PlayerPrefs.DeleteAll();
        radius = radiusSetter;
        setValuesOnStart();
        GetItSpinning();
    }



    void setValuesOnStart() {
        awatingTransition = false;
        startCamMovment = false;
        speed = Vector2.left * Mathf.Sqrt(grvtyCnst * celestialMass / radius);                  //calculating speed for circular orbit
        previousPositions = new Queue<Vector3>();
        privateMaxCount = maxCount;                                                             //for DotifyTrajectory() 
        trajectoryPoints = new Vector3[privateMaxCount];                                        //for DotifyTrajectory()
        dt = Time.fixedDeltaTime * 10;                                                          //for DotifyTrajectory()
        orbitalPeriod = (2 * Mathf.PI * radius) / speed.magnitude;
    }

    public void GetItSpinning() {
        nrmlTime = 6;
        transform.localPosition = new Vector2(bodyBeingOrbited.transform.position.x, bodyBeingOrbited.transform.position.y + radius); //putting the spaceship in place
        playerRigidbody.velocity = speed;                                                                                               //adding the speed
        Time.timeScale = nrmlTime;                                                                                                      //speeding up time so that updating the trajectory path run smoothly
        ResetFuelSlider();
        DotifyTrjctry();
        tutorialActive = false;
    }
    
    void updateRadius() {
        radius = (bodyBeingOrbited.transform.position - gameObject.transform.position).magnitude;
    }
    //public bool RadRadBro() {
    //    float idealRadius = ((IPlantCelestialsSC.celestialsQueue.Peek().transform.position - bodyBeingOrbited.transform.position).magnitude) / 2;
        
    //    return Mathf.Abs(idealRadius - radius) < grejtRadiusTreshold;
    //}


    public void FixedUpdate() {        
        playerRigidbody.AddForce(GetGravity(transform.position));               
        transform.rotation = Rotate(playerRigidbody);
       if (newChange) {DotifyTrjctry(); newChange = false;}                             //if something happens — run DotifyTrajectory() 
       if (!awatingTransition) {                                                                   
            if (inputPresent && !tutorialActive && !overlordSC.menuButtons.activeSelf) {
                if (firstBurn) {
                    startCamMovment = true;
                    Time.timeScale = nrmlTime / 10f;                                    //lerp into slowmotion perhaps???
                    firstBurn = false;                                                  //för att veta när man slutat bränna
                    burning = true;
                    pointOfBurn = transform.position;
                }
                if (burning) { Burn(); }
            } else {
                if (burning) {                                                          //checks if last frame was burning
                    burning = false;
                    float maxDX, minDX, minDY, maxDY;
                    maxDX  = maxDY = 0f;
                    minDX = minDY = 100f;
                    for (int i = 0; i < nbrOfTrajectoryPoints; i++) {
                        float deltaX = Mathf.Abs(trajectoryPoints[i].x - trajectoryPoints[(i + 1) % (nbrOfTrajectoryPoints)].x);
                        float deltaY = Mathf.Abs(trajectoryPoints[i].y - trajectoryPoints[(i + 1) % (nbrOfTrajectoryPoints)].y);
                        maxDX = (deltaX > maxDX) ? deltaX : maxDX;
                        minDX = (deltaX < minDX) ? deltaX : minDX;
                        maxDY = (deltaY > maxDY) ? deltaY : maxDY;
                        minDY = (deltaY < minDY) ? deltaY : minDY;
                    }
                    //Debug.Log("maxDX: " + maxDX + ", maxDY: " + maxDY + ", minDX: " + minDX + ", minDY: " + minDY);
                    burnCounter = 0;
                    awatingTransition = true;
                    if (CelestialInsideTrajectory()) {
                        pointOfBurn = GetPointClosestTo(successfulCelestial.transform.position);
                        GetRequiredVelocity(Vector3.Distance(pointOfBurn, successfulCelestial.transform.position), celestialMass);
                    } else {
                        GetRequiredVelocity(Vector3.Distance(pointOfBurn, bodyBeingOrbited.transform.position), celestialMass);
                    }
                }
                firstBurn = true;
                Time.timeScale = nrmlTime;
            }
        } else if(!overlordScript.fuckedUp){
           
            EstablishedNewOrbit(pointOfBurn);
            fuel.value = Mathf.MoveTowards(fuel.value, fuel.minValue, 0.5f);
        }
    }


    void Burn() {
        if (!outOfFuel) { 
            burnCounter++;
            playerRigidbody.AddRelativeForce(Vector2.up * nrmlTime / 3000 * thrust);
            speed = playerRigidbody.velocity;
            newChange = true;
            ChangeFuelValue();
        }
    }
    //bminVelocity är startingpoint
    void ResetFuelSlider() {
        fuel.minValue = 0;
        fuel.maxValue =  Mathf.Lerp(maxBurns, minBurns, Mathf.Sqrt(radius - minApoapsis) / Mathf.Sqrt(maxApoapsis - minApoapsis));
        fuel.value = fuel.minValue;
        outOfFuel = false;
    }


    void ChangeFuelValue() {
        fuel.value = burnCounter;
        outOfFuel = burnCounter > fuel.maxValue;
    }

    
    void OnTriggerEnter2D(Collider2D col) {
        explosion.Play();
        bool highScore, revive;
        highScore = revive = false;

        if(score > PlayerPrefs.GetInt("highscore")) {
            PlayerPrefs.SetInt("highscore", score);
            HighScoreSC.HighScoreUpdater();
            highScore = true;
        }
        if(score > 0) {
            revive = true;
        }
        overlordSC.Crash(highScore,revive);
        gameObject.SetActive(false);

    }


    Quaternion Rotate(Rigidbody2D obj) {
        float zRotation = Mathf.Atan2(directionVectorTowardsCelestial.y, directionVectorTowardsCelestial.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, zRotation - 180);
    }

    Vector3 GetGravity(Vector3 objAffected) {
        directionVectorTowardsCelestial = bodyBeingOrbited.transform.position - objAffected;
        return directionVectorTowardsCelestial.normalized * grvtyCnst * playerWeight * celestialMass / directionVectorTowardsCelestial.sqrMagnitude;
    }

    //will be used for GetGravity too
    Vector3 GetDirectionVector(Vector3 celestial) {
        return directionVectorTowardsCelestial = (celestial - transform.position);
    }
  
    Vector3 GetPointClosestTo(Vector3 celestial) {
        float length = 100000000;
        Vector3 periapsisPoint = Vector3.zero;
        for (int i = 0; i < nbrOfTrajectoryPoints; i+=5) {
            float testDistance = Vector2.Distance(trajectoryPoints[i], celestial);
            if (testDistance < length) {
                length = testDistance;
                periapsisPoint = trajectoryPoints[i] ;
            }
        }
        return periapsisPoint;
    }

    void GetRequiredVelocity(float radius, float CelestialMass) {
        requiredVelocity = Mathf.Sqrt(grvtyCnst * CelestialMass / radius);
    }


    void EstablishedNewOrbit(Vector3 pointOfBurn) {
        if (previousPositions.Count == 15) {
            if (((transform.position.x > pointOfBurn.x && pointOfBurn.x > lastPos.x)
            || (transform.position.x < pointOfBurn.x && pointOfBurn.x < lastPos.x))
           && ((transform.position.y > pointOfBurn.y && pointOfBurn.y > lastPos.y)
            || (transform.position.y < pointOfBurn.y && pointOfBurn.y < lastPos.y))) {
                EstablishNewOrbitValues();
                previousPositions.Clear();
            } else {
                lastPos = previousPositions.Dequeue();
            }
        }
        previousPositions.Enqueue(transform.position);
    }


    private void EstablishNewOrbitValues() {
        //Debug.Log(score);
        Vector3 transitionVel = Vector2.zero;
        if (celestialInside) {
            bodyBeingOrbited.tag = "celestial";
            bodyBeingOrbited = successfulCelestial;
            IPlantCelestialsSC.getCelestialsQueue().Dequeue();
            updateRadius();
            satisfactorySoundsSC.PrepareRandom();
            celestialMass = bodyBeingOrbited.GetComponent<celestialScript>().mass;
            bodyBeingOrbited.tag = "orbitingCelestial";
            score++;
            scoreText.text = "" + score;
            IPlantCelestialsSC.CelestialFactory();
            celestialInside = false;
        }
        awatingTransition = false;
        newChange = true;
        Vector2 tempForward = GetDirectionVector(bodyBeingOrbited.transform.position);
        Vector2 Forward = new Vector2(tempForward.y, - tempForward.x);   //rotates "gravityVector": TempForward 90 degrees clockwise -- forward
        Forward.Normalize();
        transitionVel = Forward * requiredVelocity;
        playerRigidbody.velocity = transitionVel;
        ResetFuelSlider();
    }



    bool CelestialInsideTrajectory() {
        int i = 0;
            Vector3 celestialPos = IPlantCelestialsSC.getCelestialsQueue().Peek().transform.position;
           // Debug.Log(celestialPos);
            bool a, b, c, d;                                                            //switches so that each statement only can be true once
            a = b = c = d = true;
            for (int j = 0; j < nbrOfTrajectoryPoints; j++) {
            int behindJ = j+nbrOfTrajectoryPoints - 1;
            int infrontJ = j+nbrOfTrajectoryPoints + 1; 
            float deltaX1 = Mathf.Abs(trajectoryPoints[j].x - trajectoryPoints[infrontJ%(nbrOfTrajectoryPoints)].x);
            float deltaY1 = Mathf.Abs(trajectoryPoints[j].y - trajectoryPoints[infrontJ%(nbrOfTrajectoryPoints)].y);
            float deltaX2 = Mathf.Abs(trajectoryPoints[j].x - trajectoryPoints[behindJ%(nbrOfTrajectoryPoints)].x); 
            float deltaY2 = Mathf.Abs(trajectoryPoints[j].y - trajectoryPoints[behindJ%(nbrOfTrajectoryPoints)].y);
            float shorterDX = deltaX1 < deltaX2 ? deltaX1 : deltaX2;
            float longerDX = deltaX1 > deltaX2 ? deltaX1 : deltaX2;
            float shorterDY = deltaY1 < deltaY2 ? deltaY1 : deltaY2;
            float longerDY = deltaY1 > deltaY2 ? deltaY1 : deltaY2;
            if (Mathf.Abs(trajectoryPoints[j].y - celestialPos.y) <= shorterDY/2 || Mathf.Abs(trajectoryPoints[j].y - celestialPos.y) <  longerDY/2) {
                    if (trajectoryPoints[j].x > celestialPos.x && a) {
                        i++;
                        a = false;
                    } else if (trajectoryPoints[j].x < celestialPos.x && b) {
                        i++;
                        b = false;
                    }
                } else if (Mathf.Abs(trajectoryPoints[j].x - celestialPos.x) <= shorterDX/2 || Mathf.Abs(trajectoryPoints[j].x - celestialPos.x) < longerDX/ 2) {
                    if (trajectoryPoints[j].y > celestialPos.y && c) {
                        i++;
                        c = false;
                    } else if(trajectoryPoints[j].y < celestialPos.y && d) {
                        i++;
                        d = false;
                    }
                }
            }
        Debug.Log("i: " + i + ", a: " + a + ", b: " + b + ", c: " + c + ", d: " + d);
        if (i == 4) {
            successfulCelestial = IPlantCelestialsSC.getCelestialsQueue().Peek();
            camera2DFollowSC.SetTarget(successfulCelestial.transform.position);
            camera2DFollowSC.SpeedIncrease();
            celestialInside = true;
            return true;
        } else {
            i = 0;
        }
        return false;
    }
    
    void DotifyTrjctry() {
            speed = playerRigidbody.velocity;
            trajectoryPoints = new Vector3[privateMaxCount];
            s = transform.position;
            v = speed / 10;
            a = GetGravity(s);
            step = 0;
            while (step < (privateMaxCount * simplify)) {
                if (step % simplify == 0) {
                    trajectoryPoints[step / simplify] = s;
                }
                a = GetGravity(s);
                v += a * dt;
                s += v * dt;
                step++;
            }
            nbrOfTrajectoryPoints = step / simplify;
            trajectoryLine.numPositions = nbrOfTrajectoryPoints;
            trajectoryLine.SetPositions(trajectoryPoints);    
        }
    }
    
