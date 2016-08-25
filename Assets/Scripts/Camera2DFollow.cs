using System;
using UnityEngine;


    public class Camera2DFollow : MonoBehaviour
    {
        public Vector3 target;
        public float damping = 10;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
        public float downFactor = 2f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        // Use this for initialization
        private void Start()
        {
            SetTarget(GameObject.Find("StartCelestial").transform.position);
            m_LastTargetPosition = target;
            m_OffsetZ = (transform.position - target).z;
            transform.parent = null;
        }

        public void SetTarget(Vector3 target) { this.target = target + Vector3.up * downFactor;}
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            // only update lookahead pos if accelerating or changed direction
            float yMoveDelta = (target - m_LastTargetPosition).y;

            bool updateLookAheadTarget = Mathf.Abs(yMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.up*Mathf.Sign(yMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target;
        }
    }

