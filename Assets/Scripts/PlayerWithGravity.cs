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
    private int apoapsisPointIndex;
    private Vector3 apoapsisPoint;
    

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
        
    
    change GameObjects to Transform if only used with Transform
    */


    void Start() {
        privateMaxCount = maxCount;
        trajectoryPoints = new Vector3[privateMaxCount];
        dt = Time.fixedDeltaTime*10;
        bodyBeingOrbited = GameObject.FindGameObjectWithTag("startOrbit");                      //initial orbit around celestial with tag "startOrbit" now called bodyBeingOrbited
        trajectoryLine = GetComponent<LineRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerWeight = playerRigidbody.mass;
        //semiMjrAxisLength = rdius; 
        bodyBeingOrbitedRigidbody = bodyBeingOrbited.GetComponent<Rigidbody2D>();
        speed = Vector2.right * Mathf.Sqrt(grvtyCnst * bodyBeingOrbitedRigidbody.mass / radius);
        transform.localPosition = new Vector2(bodyBeingOrbited.transform.position.x, bodyBeingOrbited.transform.position.y - (radius));
        playerRigidbody.AddForce(speed/2);        
        Time.timeScale = nrmlTime;       
        GetApoapsis(bodyBeingOrbited.transform.position);
        //orbitalPeriod = 2 * Mathf.PI * (Mathf.Sqrt(Mathf.Pow(rdius, 3) / (grvtyCnst * bodyBeingOrbitedRigidbody.mass)));
        //orbitalPeriod /= 2;                                                                         //since speed is only half of what its supposed to 
        DotifyTrjctry();

    }



    void FixedUpdate() {

        playerRigidbody.AddForce(GetGravity(playerRigidbody.transform.position));
        transform.rotation = Rotate(playerRigidbody);
        if (!awatingTransition) {
            if ((Input.touchCount > 0 || Input.anyKey)) {
                if (firstBurn) {
                    Time.timeScale = nrmlTime / 10f;                                    //lerp into slowmotion???
                    firstBurn = false;                                                  //för att veta när man slutat bränna
                    burning = true;
                    burningPosIndex = GetSmallestDistanceToIndexOnTrajectoryPoints(transform.position);       //funkar ej pga att dotifyTrajectory() uppdaterar punkterna vid varje körning
                 }
                if (burning) { Burn(); }
            } else {
                if (burning) {                                                          //checks if last frame was burning
                    burning = false;
                    awatingTransition = true;
                    CelestialInsideTrajectory();
                }
                firstBurn = true;
                Time.timeScale = nrmlTime;
            }
        } else {
            
            if (celestialInside) {
                GetValuesForEstablishingNewOrbit();
                EstablishedNewOrbit();
        }else if(burningPosIndex == GetSmallestDistanceToIndexOnTrajectoryPoints(transform.position)) {
                //bodyBeingOrbited = successfulCelestial;
                //bodyBeingOrbitedRigidbody = successfulCelestialRigidBody;
                Debug.Log("awaitingTrans");
                playerRigidbody.AddRelativeForce(Vector3.up * (speed.magnitude - GetRequiredVelocity(Vector3.Distance(transform.position, bodyBeingOrbited.transform.position),
                    bodyBeingOrbitedRigidbody.mass)));
                DotifyTrjctry();
                awatingTransition = false;
            }
            //if (transform.position = burningPos //funkar inte eftersom vi inte anväder trajectoryPositions[]                   //BLABLABLAFIXASÅATTDENANVÄNDERESTABLISHNEWORBIT ETC MED HJÄLP AV LASTPOS     

        }
    }

    void Burn() {
        playerRigidbody.AddRelativeForce(Vector2.up * nrmlTime / 3000);
        speed = playerRigidbody.velocity;
        DotifyTrjctry();


    }
    
    int getClosestTrajectoryPositionsIndex() {
        for(int i = 0; i < nbrOfTrajectoryPoints; i++) {
            Vector3.Distance(transform.position, trajectoryPoints[i]);
        }


        return 1;
    }

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
    int GetSmallestDistanceToIndexOnTrajectoryPoints(Vector3 point) {
        float length = 100000000;
        int periapsisPointIndex = -1;
        for (int i = 0; i < nbrOfTrajectoryPoints; i++) {
            float testDistance = Vector2.Distance(trajectoryPoints[i], point);
            if (testDistance < length) {
                length = testDistance;
                periapsisPointIndex = i;
            }
        }
        return periapsisPointIndex;
    }

    float GetRequiredVelocity(float radius, float CelestialMass) {
        return Mathf.Sqrt(grvtyCnst * CelestialMass / radius);
    }

    void GetValuesForEstablishingNewOrbit() {                                              
        if (celestialInside) {
            transitionPointIndex = GetSmallestDistanceToIndexOnTrajectoryPoints(successfulCelestial.transform.position);
            requiredVelocity = GetRequiredVelocity(Vector3.Distance(trajectoryPoints[transitionPointIndex],successfulCelestial.transform.position),successfulCelestialRigidBody.mass);
        }
    }

    void EstablishedNewOrbit() {
        
        if (((transform.position.x > trajectoryPoints[transitionPointIndex].x && trajectoryPoints[transitionPointIndex].x > lastPos.x) 
                || (transform.position.x < trajectoryPoints[transitionPointIndex].x && trajectoryPoints[transitionPointIndex].x < lastPos.x))
           &&   ((transform.position.y > trajectoryPoints[transitionPointIndex].y && trajectoryPoints[transitionPointIndex].y > lastPos.y)
                || (transform.position.y < trajectoryPoints[transitionPointIndex].y && trajectoryPoints[transitionPointIndex].y < lastPos.y))) {
            Debug.Log("EstablishedNewOrbit");
            Vector3 transitionVel = Vector2.down * Mathf.Abs(speed.magnitude-requiredVelocity);
            playerRigidbody.AddRelativeForce(transitionVel);
            awatingTransition = false;
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
        
        v = speed / 10;
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