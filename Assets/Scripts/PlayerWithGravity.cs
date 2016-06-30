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
    private float startingVelocity;
    private float semiMjrAxisLength;
    private float grvtyPull;
    private float orbitalPeriod;

    public int nrmlTime = 6;

    private readonly float grvtyCnst = 0.000001f;

    private Vector2 directionvectorGrvty;
    private Vector3[] trajectoryPoints;

    //for Get(apoapsis/periapsis)Point()

    private Vector3 apoapsisPoint;
    private Vector3 periapsisPoint;


    //for Dotify()
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
    int burningPosIndex;
    int transitionPointIndex;
    /*
    TODO:
        GetSmallestDistanceToIndexOnTrajectoryPoints fix
    */


    void Start() {
        privateMaxCount = maxCount;
        trajectoryPoints = new Vector3[privateMaxCount];
        dt = Time.fixedDeltaTime * 10;
        bodyBeingOrbited = GameObject.FindGameObjectWithTag("startOrbit");                      //initial orbit around celestial with tag "startOrbit" now called bodyBeingOrbited
        trajectoryLine = GetComponent<LineRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerWeight = playerRigidbody.mass;
        //semiMjrAxisLength = rdius; 
        bodyBeingOrbitedRigidbody = bodyBeingOrbited.GetComponent<Rigidbody2D>();
        speed = Vector2.right * Mathf.Sqrt(grvtyCnst * bodyBeingOrbitedRigidbody.mass / radius);
        transform.localPosition = new Vector2(bodyBeingOrbited.transform.position.x, bodyBeingOrbited.transform.position.y - (radius));
        playerRigidbody.AddForce(speed / 2);
        Time.timeScale = nrmlTime;
        GetApoapsis(bodyBeingOrbited.transform.position);
        //orbitalPeriod = 2 * Mathf.PI * (Mathf.Sqrt(Mathf.Pow(rdius, 3) / (grvtyCnst * bodyBeingOrbitedRigidbody.mass)));
        //orbitalPeriod /= 2;                                                                         //since speed is only half of what its supposed to 
        DotifyTrjctry();

    }



    void FixedUpdate() {
        
        playerRigidbody.AddForce(GetGravity(playerRigidbody.transform.position));
        transform.rotation = Rotate(playerRigidbody);
        DotifyTrjctry();
        if (!awatingTransition) {
            if ((Input.touchCount > 0 || Input.anyKey)) {
                if (firstBurn) {
                    Time.timeScale = nrmlTime / 10f;                                    //lerp into slowmotion???
                    firstBurn = false;                                                  //för att veta när man slutat bränna
                    burning = true;
                    pointOfBurn = transform.position;
                    //funkar ej pga att dotifyTrajectory() uppdaterar punkterna vid varje körning
                }
                if (burning) { Burn(); }
            } else {
                if (burning) {                                                          //checks if last frame was burning
                    burning = false;
                    awatingTransition = true;
                    if (CelestialInsideTrajectory()) {
                        GetRequiredVelocity(Vector3.Distance(pointOfBurn, successfulCelestial.transform.position), successfulCelestialRigidBody.mass);
                        pointOfBurn = GetPointClosestTo(successfulCelestial.transform.position);
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
        DotifyTrjctry();
    }
    
    //int getClosestTrajectoryPositionsIndex() {
    //    for(int i = 0; i < nbrOfTrajectoryPoints; i++) {
    //        Vector3.Distance(transform.position, trajectoryPoints[i]);
    //    }
    //    return 1;
    //}

    Quaternion Rotate(Rigidbody2D obj) {
        float zRotation = Mathf.Atan2(directionvectorGrvty.y, directionvectorGrvty.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, zRotation - 180);
    }

    Vector3 GetGravity(Vector3 objAffected) {
        directionvectorGrvty = (bodyBeingOrbited.transform.position - objAffected);
        return directionvectorGrvty.normalized * grvtyCnst * playerWeight * bodyBeingOrbitedRigidbody.mass / directionvectorGrvty.sqrMagnitude;
    }

   
    Vector3 GetApoapsis(Vector3 celestial) {
        float length = -1;
        Vector3 apoapsisPoint = Vector3.zero;
            for(int i = 0; i < nbrOfTrajectoryPoints; i++) {
            float testDistance = Vector2.Distance(trajectoryPoints[i], celestial);
            if (testDistance > length){
                length = testDistance;
                this.apoapsisPoint = trajectoryPoints[i];
            }
        }
        return apoapsisPoint;
    }
    
    //metoden suger fixa den förhelvete
    //låt den kolla som EstablishedNewOrbit() gör istället som kollar varje FixedUpdate()
    Vector3 GetPointClosestTo(Vector3 celestial) {
        float length = 100000000;
        for (int i = 0; i < nbrOfTrajectoryPoints; i++) {
            float testDistance = Vector2.Distance(trajectoryPoints[i], celestial);
            if (testDistance < length) {
                length = testDistance;
                this.periapsisPoint = trajectoryPoints[i] ;
            }
        }
        return periapsisPoint;
    }

    void GetRequiredVelocity(float radius, float CelestialMass) {
        requiredVelocity = Mathf.Sqrt(grvtyCnst * CelestialMass / radius);
    }


    //NÖDVÄNDIG???
    //void GetValuesForEstablishingNewOrbit(GameObject celestial, Rigidbody2D celestialRigidbody) {
    //    GetRequiredVelocity(Vector3.Distance(pointOfBurn,celestial.transform.position),celestialRigidbody.mass);
    //}

    void EstablishedNewOrbit(Vector3 pointOfBurn) {
        if (((transform.position.x > pointOfBurn.x && pointOfBurn.x > lastPos.x) 
                || (transform.position.x < pointOfBurn.x && pointOfBurn.x < lastPos.x))
           &&   ((transform.position.y > pointOfBurn.y && pointOfBurn.y > lastPos.y)
                || (transform.position.y < pointOfBurn.y && pointOfBurn.y < lastPos.y))) {
            Debug.Log("EstablishedNewOrbit");
            Vector3 transitionVel = Vector2.down * (speed.magnitude-requiredVelocity);
            playerRigidbody.AddRelativeForce(transitionVel);
            if (celestialInside) { bodyBeingOrbited = successfulCelestial; bodyBeingOrbitedRigidbody = successfulCelestialRigidBody; }
            awatingTransition = false;
            DotifyTrjctry();
        }
        lastPos = transform.position;
        
        
    }
    

    GameObject[] GetActiveCelestialsPos() {
        return GameObject.FindGameObjectsWithTag("celestial");
    }


    //fortfarande buggig om celestial är under vad man kommer från
    bool CelestialInsideTrajectory() {
        GameObject[] celestials = GetActiveCelestialsPos();
        int i = 0;
        Vector3 last = Vector3.zero;
        foreach (GameObject celestial in celestials) {
            Vector3 celestialPos = celestial.transform.position;              
            foreach (Vector3 trajectoryPoint in trajectoryPoints) {
                if((trajectoryPoint.y > celestialPos.y && last.y < celestialPos.y)||(trajectoryPoint.y < celestialPos.y && last.y > celestialPos.y)) {
                    if (trajectoryPoint.x > celestialPos.x) {
                        i++;
                    } else {
                        i++;
                    }
                } else if((trajectoryPoint.x > celestialPos.x && last.x < celestialPos.x)||(trajectoryPoint.x < celestialPos.x && last.x > celestialPos.x)) {
                    if (trajectoryPoint.y > celestialPos.y) {
                        i++;
                    } else {
                        i++;
                    }
                }
                last = trajectoryPoint;   
            }
            if (i == 4) {
                successfulCelestial = celestial;
                successfulCelestialRigidBody = celestial.GetComponent<Rigidbody2D>();
                celestialInside = true;
                return true;
            }
        }
        return false;
    }
    
    void DotifyTrjctry() {
        trajectoryPoints = new Vector3[privateMaxCount];
        angle = 0;
        s = transform.position;
        lastS = s;
        v = playerRigidbody.velocity/10;
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
            tempAngleSum += Vector3.Angle(s, lastS);
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