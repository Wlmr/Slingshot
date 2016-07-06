using UnityEngine;
using System.Collections;

public class PlayerWithGravity : MonoBehaviour {


    public float radius;

    private LineRenderer trajectoryLine;
    private GameObject bodyBeingOrbited;
    private Rigidbody2D playerRigidbody;
    private Rigidbody2D bodyBeingOrbitedRigidbody;


    private bool firstBurn = true;
    private bool firstAfterBurn = false;
    private bool burning = false;
    private bool firstCalc = true;

    private Vector2 speed;
    private float grvtyPull;

   public int nrmlTime = 6;

    private readonly float grvtyCnst = 0.000001f;

    private Vector2 directionVectorTowardsCelestial;


    //for DotifyTrajectory()
    private Vector3[] trajectoryPoints;
    public int simplify = 10;
    public int maxCount = 1000;
    private float playerWeight;
    private int nbrOfTrajectoryPoints;
    private int privateMaxCount;
    private float angle;
    private float dt;
    private Vector3 s;
    private Vector3 lastS;
    private Vector3 v;
    private Vector3 a;
    private float tempAngleSum;
    private int step;
    private bool newChange = true;

    //for CelestialInsideTrajectory()/TryEstablishNewOrbit()

    GameObject successfulCelestial;
    Rigidbody2D successfulCelestialRigidBody;
    bool celestialInside;
    private Vector3 pointOfBurn;

    // int successfulCelestialIndex;
    GameObject[] celestials;

    //TryEstablishNewOrbit

    float requiredVelocity;
    Vector3 lastPos;
    bool awatingTransition = false;

    private int counter;

    static public bool startCamMovment;

    /*
    TODO:
        1.        Figure out why the spacecraft usually needs to rounds to establish new orbit
        2.        Divide the force over several frames so that it looks as if it is burning opposite?
        3.        Fixa så att den närmsta celestial testas först i CelestialInsideTrajectory();
    */


    void Start() {
        startCamMovment = false;
        privateMaxCount = maxCount;                                                             //for DotifyTrajectory() 
        trajectoryPoints = new Vector3[privateMaxCount];                                        //for DotifyTrajectory()
        dt = Time.fixedDeltaTime * 10;                                                          //for DotifyTrajectory()
        bodyBeingOrbited = GameObject.FindGameObjectWithTag("startOrbit");                      //initial orbit around celestial with tag "startOrbit" now called bodyBeingOrbited
        trajectoryLine = GetComponent<LineRenderer>();                                          
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerWeight = playerRigidbody.mass;
        bodyBeingOrbitedRigidbody = bodyBeingOrbited.GetComponent<Rigidbody2D>();
        transform.localPosition = new Vector2(bodyBeingOrbited.transform.position.x, bodyBeingOrbited.transform.position.y - (radius)); //putting the spaceship in place
        speed = Vector2.right * Mathf.Sqrt(grvtyCnst * bodyBeingOrbitedRigidbody.mass / radius);                                        //calculating speed for circular orbit
        playerRigidbody.velocity = speed;                                                                                               //adding the speed
        Time.timeScale = nrmlTime;                                                                                                      //speeding up time so that updating the trajectory path run smoothly
        DotifyTrjctry();
        

    }



    void FixedUpdate() {
        counter++;
        playerRigidbody.AddForce(GetGravity(transform.position));               
        transform.rotation = Rotate(playerRigidbody);
        if (newChange) { DotifyTrjctry(); newChange = false; }                                      //if something happens — run DotifyTrajectory() 
        if (!awatingTransition) {                                                                   
            if ((Input.touchCount > 0 || Input.anyKey)) {
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
                    awatingTransition = true;
                    if (CelestialInsideTrajectory()) {
                        pointOfBurn = GetPointClosestTo(successfulCelestial.transform.position);
                        GetRequiredVelocity(Vector3.Distance(pointOfBurn, successfulCelestial.transform.position), successfulCelestialRigidBody.mass);
                    } else {
                        GetRequiredVelocity(Vector3.Distance(pointOfBurn, bodyBeingOrbited.transform.position), bodyBeingOrbitedRigidbody.mass);
                    }
                }
                firstBurn = true;
                Time.timeScale = nrmlTime;
            }
        } else {
            EstablishedNewOrbit(pointOfBurn);
        }
    }


    void Burn() {
        playerRigidbody.AddRelativeForce(Vector2.up * nrmlTime / 3000);
        speed = playerRigidbody.velocity;
        newChange = true;
    }
    


    Quaternion Rotate(Rigidbody2D obj) {
        float zRotation = Mathf.Atan2(directionVectorTowardsCelestial.y, directionVectorTowardsCelestial.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, zRotation - 180);
    }

    Vector3 GetGravity(Vector3 objAffected) {
        directionVectorTowardsCelestial = bodyBeingOrbited.transform.position - objAffected;
        return directionVectorTowardsCelestial.normalized * grvtyCnst * playerWeight * bodyBeingOrbitedRigidbody.mass / directionVectorTowardsCelestial.sqrMagnitude;
    }

    //will be used for GetGravity too
    Vector3 GetDirectionVector(Vector3 celestial) {
        return directionVectorTowardsCelestial = (celestial - transform.position);
    }

  
    Vector3 GetPointClosestTo(Vector3 celestial) {
        float length = 100000000;
        Vector3 periapsisPoint = Vector3.zero;
        for (int i = 0; i < nbrOfTrajectoryPoints; i++) {
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
        if (((transform.position.x > pointOfBurn.x && pointOfBurn.x > lastPos.x) 
                || (transform.position.x < pointOfBurn.x && pointOfBurn.x < lastPos.x))
           &&   ((transform.position.y > pointOfBurn.y && pointOfBurn.y > lastPos.y)
                || (transform.position.y < pointOfBurn.y && pointOfBurn.y < lastPos.y))) {
            Vector3 transitionVel = Vector2.zero;
            if (celestialInside) { bodyBeingOrbited.tag = "celestial"; bodyBeingOrbited = successfulCelestial; bodyBeingOrbitedRigidbody = successfulCelestialRigidBody; }
            awatingTransition = false;
            newChange = true;
            Vector2 tempForward = GetDirectionVector(bodyBeingOrbited.transform.position);
            Vector2 Forward = new Vector2(tempForward.y, -tempForward.x);   //rotates "gravityVector": TempForward 90 degrees clockwise -- forward
            Forward.Normalize();
            transitionVel = Forward * requiredVelocity;
            playerRigidbody.velocity = transitionVel;
        }
        lastPos = transform.position;
        
        
    }
    

    GameObject[] GetActiveCelestialsPos() {
        return GameObject.FindGameObjectsWithTag("celestial");
    }


    //fortfarande buggig om celestial är under vad man kommer från 
    bool CelestialInsideTrajectory() {
        GameObject[] celestials = GetActiveCelestialsPos();
        //GameObject[] celestias;
        //foreach(GameObject cel in celestialsTemp) {
        //    celestials =
        //}
        int i = 0;
        Vector3 last = trajectoryPoints[nbrOfTrajectoryPoints-1];
        foreach (GameObject celestial in celestials) {
            Vector3 celestialPos = celestial.transform.position;
            bool a, b, c, d;                                                            //switches so that each statement only can be true once
            a = b = c = d = true;
            for (int j = 0; j < nbrOfTrajectoryPoints; j++) {
                if ((trajectoryPoints[j].y > celestialPos.y && last.y < celestialPos.y) || (trajectoryPoints[j].y < celestialPos.y && last.y > celestialPos.y)) {
                    if (trajectoryPoints[j].x > celestialPos.x && a) {
                        i++;
                        a = false;
                    } else if (b) {
                        i++;
                        b = false;
                    }
                } else if ((trajectoryPoints[j].x > celestialPos.x && last.x < celestialPos.x) || (trajectoryPoints[j].x < celestialPos.x && last.x > celestialPos.x)) {
                    if (trajectoryPoints[j].y > celestialPos.y && c) {
                        i++;
                        c = false;
                    } else if(d) {
                        i++;
                        d = false;
                    }
                }

                int fivePointsBefore = (nbrOfTrajectoryPoints-5+j)%nbrOfTrajectoryPoints;
                Debug.Log(fivePointsBefore);
                last = trajectoryPoints[fivePointsBefore];
            }
            if (i == 4) {
                successfulCelestial = celestial;
                successfulCelestialRigidBody = celestial.GetComponent<Rigidbody2D>();
                celestialInside = true;
                return true;
            } else {
                i = 0;
            }
        }
        return false;
    }
    
    void DotifyTrjctry() {
        speed = playerRigidbody.velocity;
        trajectoryPoints = new Vector3[privateMaxCount];
        angle = 0;
        s = transform.position;
        lastS = s;
        v = speed/10;
        a = GetGravity(s);
        tempAngleSum = 0;
        step = 0;
        while(angle < 360 && step < (privateMaxCount*simplify)) {
            if (step % simplify == 0) {
                trajectoryPoints[step / simplify] = s;
                angle += tempAngleSum;
                tempAngleSum = 0;
            }
            a = GetGravity(s);
            v += a * dt;
            s += v * dt;
            tempAngleSum += Vector3.Angle(lastS, s);
            lastS = s;
            step++;
        }
        nbrOfTrajectoryPoints = step / simplify;
        trajectoryLine.SetVertexCount(nbrOfTrajectoryPoints);
        for(int i = 0; i < nbrOfTrajectoryPoints; i++) {
            trajectoryLine.SetPosition(i, trajectoryPoints[i]);
        }
    }
}