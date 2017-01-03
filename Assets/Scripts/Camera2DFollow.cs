using UnityEngine;
using UnityEngine.UI;


public class Camera2DFollow : MonoBehaviour {
    //for RetryScreen
    public RawImage retryBG;
    private Color retryColor;

    public Vector3 target;
    public float damping;
    public float lookAheadFactor;
    public float lookAheadReturnSpeed;
    public float lookAheadMoveThreshold;
    public float downFactor;
    public float speedOfCollapsingUniverse;

    private float m_OffsetZ;
    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;
    private Vector3 currentCelestial;
        
    // Use this for initialization
    private void Start() {
        retryColor = new Color(0, 0, 0, 0.2f);
        SetTarget(GameObject.Find("StartCelestial").transform.position);
        m_LastTargetPosition = target;
        m_OffsetZ = (transform.position - target).z;
        transform.parent = null;
    }

    public void SetTarget(Vector3 target) {
        currentCelestial = target;
        this.target = target + Vector3.up * downFactor;
    }
        
    // Update is called once per frame
    private void FixedUpdate(){
        if (PlayerWithGravity.startCamMovment && !overlordScript.fuckedUp) {
            target += Vector3.up / speedOfCollapsingUniverse;
        } else if (overlordScript.fuckedUp) {
            retryBG.color = new Color(1,1,1,Mathf.MoveTowards(retryBG.color.a, retryColor.a, 0.01f));
        }
        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        Vector3 aheadTargetPos = target + m_LookAheadPos + Vector3.forward * m_OffsetZ;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
        transform.position = newPos;
        m_LastTargetPosition = target;    
    }
}

