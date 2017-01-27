using UnityEngine;
using UnityEngine.UI;


public class Camera2DFollow : MonoBehaviour {
    //for RetryScreen
    public RawImage retryBG;
    private Color retryColor;
    private Color antiRetryColor;


    public PlayerWithGravity playerWithGravitySC;

   
   

    public Vector3 target;
    public float damping;
    public float lookAheadFactor;
    public float lookAheadReturnSpeed;
    public float lookAheadMoveThreshold;
    public float downFactor;
    public float speedOfCollapsingUniverse;

    private float m_OffsetZ;
   // private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;
    public float maxSpeed;
    private float deltaSpeed;
    public int celestialNbrMaxSpeed;

    //  private Vector3 currentCelestial;

    // Use this for initialization
    private void Start() {
        
    
        retryColor = new Color(0, 0, 0, 0.2f);
        antiRetryColor = new Color(0, 0, 0, 0);
        SetTarget(GameObject.Find("StartCelestial").transform.position);
        m_OffsetZ = (transform.position - target).z;
        transform.parent = null;
        deltaSpeed = Mathf.Abs( maxSpeed - speedOfCollapsingUniverse);
    }

    public void SetTarget(Vector3 target) {
        this.target = target + Vector3.up * downFactor;
    }
        
    public void SpeedIncrease() {
        if (speedOfCollapsingUniverse > maxSpeed) {
            speedOfCollapsingUniverse -= (deltaSpeed / celestialNbrMaxSpeed);

        }
    }

private void FixedUpdate(){
        // cam.orthographicSize = Mathf.Lerp(maxZoom, minZoom, (fuel.value - fuel.minValue)/(fuel.maxValue-fuel.minValue));
       // ZoomCheck();
        if (PlayerWithGravity.startCamMovment && !overlordScript.fuckedUp) {
            target += Vector3.up / speedOfCollapsingUniverse;
        } else if (overlordScript.fuckedUp) {
            retryBG.color = new Color(1,1,1,Mathf.MoveTowards(retryBG.color.a, retryColor.a, 0.01f));
        } else if (!overlordScript.fuckedUp) {
            retryBG.color = new Color(1, 1, 1, Mathf.MoveTowards(retryBG.color.a, antiRetryColor.a , 0.01f));
        }
        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        Vector3 aheadTargetPos = target + m_LookAheadPos + Vector3.forward * m_OffsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
        transform.position = newPos;
   //     m_LastTargetPosition = target;    
    }


    //public void ZoomCheck() {
    //    float x0 = playerWithGravitySC.bodyBeingOrbited.transform.position.x;
    //    float xMax = float.MinValue;
    //    float xMin = float.MaxValue;
    //    float dx = 0;
    //    foreach (Vector3 point in playerWithGravitySC.trajectoryPoints) {
    //        xMin = point.x < xMin ? point.x : xMin;
    //        xMax = point.x > xMax ? point.x : xMax;
    //    }
    //    xMin -= x0;
    //    xMax -= x0;
    //    dx = Mathf.Abs(xMax) >= Mathf.Abs(xMin) ? xMax : xMin;
    //    if ((dx * 2) > 3) cam.orthographicSize = dx * 2;  


    
}

