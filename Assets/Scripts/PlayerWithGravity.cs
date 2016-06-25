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
  //  private bool firstCalc = true;

    private Vector2 speed;
    private float startingVelocity;
    private float semiMjrAxisLength;
    private float grvtyPull;
    private float orbitalPeriod;
    private float playerWeight;
    public int maxCount = 1000;
    private int privateMaxCount;
    public int simplify = 10;
    public int nrmlTime = 6;

    private readonly float grvtyCnst = 0.000001f;

    private Vector2 directionvectorGrvty;
    private Vector3[] trajectoryPoints;

    //for Dotify()
    float angle;
    float dt;
    Vector3 s;
    Vector3 lastS;
    Vector3 v;
    Vector3 a;
    float tempAngleSum;
    int step;
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
        GetSemiMjrAxsLngth();
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
                firstBurn = false;
            }
            playerRigidbody.AddRelativeForce(Vector2.up * nrmlTime / 3000);
            speed = playerRigidbody.velocity;
            DotifyTrjctry();
        } else {
            firstBurn = true;
            Time.timeScale = nrmlTime; 
        }     
    }

    void Burn() {

        
        
    }

    Quaternion Rotate(Rigidbody2D obj) {
        float zRotation = Mathf.Atan2(directionvectorGrvty.y, directionvectorGrvty.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, zRotation - 180);
    }

    Vector3 GetGravity(Vector3 objAffected) {
        directionvectorGrvty = (bodyBeingOrbited.transform.position - objAffected);
        return directionvectorGrvty.normalized * grvtyCnst * playerWeight * bodyBeingOrbitedRigidbody.mass / directionvectorGrvty.sqrMagnitude;
    }


    
    void GetSemiMjrAxsLngth() {
        float length = -1;
        foreach(Vector3 point in trajectoryPoints){
            float testDistance = Vector2.Distance(point, bodyBeingOrbited.transform.position);
            if (testDistance > length){
                length = testDistance;
            }
        }
        semiMjrAxisLength = length;
    }
    


    void EstablishNewOrbit() {                                              
        if (CelestialInsideTrajectory()) {

        }
    }



    bool CelestialInsideTrajectory() {
        return true;
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
        trajectoryLine.SetVertexCount(step/simplify);
        for(int i = 0; i < step/simplify; i++) {
            trajectoryLine.SetPosition(i, trajectoryPoints[i]);
        }
    }
}