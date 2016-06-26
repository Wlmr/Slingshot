using UnityEngine;
using System.Collections;

public class PlayerWithGravity : MonoBehaviour {


    public float rdius;

    private LineRenderer trajectoryLine;
    private GameObject bodyBeingOrbited;
    private Rigidbody2D playerRigidbody;      
    private Rigidbody2D bodyBeingOrbitedRigidbody;
    private Rigidbody2D trajectoryHelper;

    private bool firstBurn = true;
    private bool firstAfterBurn = false;
    private bool burning = false;
  //  private bool firstCalc = true;

    private Vector2 speed;
    private float startingVelocity;
    private float semiMjrAxisLength;
    private float grvtyPull;
    private float orbitalPeriod;
    private float playerWeight;
    private int trajectoryPointsLength;
    public int maxCount = 1000;
    private int privateMaxCount;
    public int simplify = 10;
    public int nrmlTime = 6;

    private readonly float grvtyCnst = 0.000001f;

    private Vector2 directionvectorGrvty;
    private Vector3[] trajectoryPoints;

    //for GetSemiMjrAxsPoint()
    private int semiMjrAxsPointIndex;
    private Vector3 semiMjrAxsPoint;

    //for Dotify()
    float angle;
    float dt;
    Vector3 s;
    Vector3 lastS;
    Vector3 v;
    Vector3 a;
    float tempAngleSum;
    int step;

    //for CelestialInsideTrajectory()/TryEstablishNewOrbit()

    Vector3 successfulCelestial;
    /*
    TODO:
    
    trajectoryTransition — add angle
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
        //    semiMjrAxisLength = rdius; 
        bodyBeingOrbitedRigidbody = bodyBeingOrbited.GetComponent<Rigidbody2D>();
        speed = Vector2.right * Mathf.Sqrt(grvtyCnst * bodyBeingOrbitedRigidbody.mass / rdius);
        transform.localPosition = new Vector2(bodyBeingOrbited.transform.position.x, bodyBeingOrbited.transform.position.y - (rdius));
        playerRigidbody.AddForce(speed/2);        
        Time.timeScale = nrmlTime;       
        GetApoapsis();
        orbitalPeriod = 2 * Mathf.PI * (Mathf.Sqrt(Mathf.Pow(rdius, 3) / (grvtyCnst * bodyBeingOrbitedRigidbody.mass)));
        orbitalPeriod /= 2;                                                                         //since speed is only half of what its supposed to 
        DotifyTrjctry();

    }



    void FixedUpdate(){
        
        playerRigidbody.AddForce(GetGravity(playerRigidbody.transform.position));
        transform.rotation = Rotate(playerRigidbody);
        if (Input.touchCount > 0 || Input.anyKey){
            if (firstBurn){
                Time.timeScale = nrmlTime/10f;                                    //lerp into slowmotion???
                firstBurn = false;                                                //för att veta när man slutat bränna
                burning = true;
            }
            Burn();
        } else {
            if (burning) {                                                      //checks if last frame was burning
                burning = false;
                Debug.Log(CelestialInsideTrajectory());
                TryEstablishNewOrbit();
            }
            firstBurn = true;
            Time.timeScale = nrmlTime; 
        }     
    }

    void Burn() {
        playerRigidbody.AddRelativeForce(Vector2.up * nrmlTime / 3000);
        speed = playerRigidbody.velocity;
        DotifyTrjctry();


    }

    Quaternion Rotate(Rigidbody2D obj) {
        float zRotation = Mathf.Atan2(directionvectorGrvty.y, directionvectorGrvty.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, zRotation - 180);
    }

    Vector3 GetGravity(Vector3 objAffected) {
        directionvectorGrvty = (bodyBeingOrbited.transform.position - objAffected);
        return directionvectorGrvty.normalized * grvtyCnst * playerWeight * bodyBeingOrbitedRigidbody.mass / directionvectorGrvty.sqrMagnitude;
    }


    
    Vector3 GetApoapsis() {
        float length = -1;
        Vector3 apoapsisPoint = Vector3.zero;
            for(int i = 0; i < trajectoryPointsLength; i++) {
            float testDistance = Vector2.Distance(trajectoryPoints[i], bodyBeingOrbited.transform.position);
            if (testDistance > length){
                length = testDistance;
                apoapsisPoint = trajectoryPoints[i];
            }
        }
        return apoapsisPoint;
    }

    Vector3 GetPeriapsis() {
        float length = 100000000;
        Vector3 periapsisPoint = Vector3.zero;
        for (int i = 0; i < trajectoryPointsLength; i++) {
            float testDistance = Vector2.Distance(trajectoryPoints[i], bodyBeingOrbited.transform.position);
            if (testDistance < length) {
                length = testDistance;
                periapsisPoint = trajectoryPoints[i];
            }
        }
        return periapsisPoint;
    }


    void TryEstablishNewOrbit() {                                              
        if (CelestialInsideTrajectory()) {

        }
    }

    Vector3[] GetActiveCelestialsPos() {
        
        GameObject[] celestials = GameObject.FindGameObjectsWithTag("celestial");
        Vector3[] celestialsPos = new Vector3[celestials.Length];
        for (int i = 0; i < celestials.Length; i++) {
            celestialsPos[i] = celestials[i].transform.position;
            }
        return celestialsPos;
    }


    //fortfarande buggig om celestial är under vad man kommer från
    bool CelestialInsideTrajectory() {
        Vector3[] celestials = GetActiveCelestialsPos();
        int i = 0;
        Vector3 last = Vector3.zero;
        foreach (Vector3 celestial in celestials) {                 
            foreach (Vector3 trajectoryPoint in trajectoryPoints) {
                if((trajectoryPoint.y > celestial.y && last.y < celestial.y)||(trajectoryPoint.y < celestial.y && last.y > celestial.y)) {
                    if (trajectoryPoint.x > celestial.x) {
                        i++;
                    } else {
                        i++;
                    }
                } else if((trajectoryPoint.x > celestial.x && last.x < celestial.x)||(trajectoryPoint.x < celestial.x && last.x > celestial.x)) {
                    if (trajectoryPoint.y > celestial.y) {
                        i++;
                    } else {
                        i++;
                    }
                }
                last = trajectoryPoint;
                
            }
            if (i == 4) {
                successfulCelestial = celestial;
                return true;
            }
        }
        return i==4;
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
        trajectoryPointsLength = step / simplify;
        trajectoryLine.SetVertexCount(trajectoryPointsLength);
        
        for(int i = 0; i < trajectoryPointsLength; i++) {
            trajectoryLine.SetPosition(i, trajectoryPoints[i]);
        }
    }
}