using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

    private LineRenderer trajectoryLine;
    private GameObject bodyBeingOrbited;

    public float speed;

 //   private float factor = 1f;

    private bool firstBurn = true;

    private float radius;
    private float radiusA;
    private float radiusB;
    private float angle = 1.5f*Mathf.PI;
    private float rotationalSpeed;
    private float gravitationalPull;
  //  private float riktningsvinkel;                                                                  //för att multiplicera ellipscenter med 

    private Vector2 ellipscenter;

    private Vector3[] trajectoryPositions;
    
    
    /*
    TODO:
    rotationalSpeed — requires both radius and gravitationalPull???
    trajectoryTransition — add angle
    */

    void Start ()
    {
        bodyBeingOrbited = GameObject.FindGameObjectWithTag("startOrbit");                      //initial orbit around celestial with tag "startOrbit" now called bodyBeingOrbited
        trajectoryLine = GetComponent<LineRenderer>();
        radius = 1.0f; radiusA = radius; radiusB = radius;                                      //set all different radii to 1 (perfect circle) 
        ellipscenter = bodyBeingOrbited.transform.position;                                     //center of trajectory ellipse initially set to bodyBeingOrbited
        TrajectoryRenderer(bodyBeingOrbited.transform.position, radius, radius);                //method drawing trajectory path
    }
	
	
/*	void FixedUpdate ()
    {
        ellipscenter = new Vector2(bodyBeingOrbited.transform.position.x, radiusB - radius + bodyBeingOrbited.transform.position.y);
        gravitationalPull = Vector2.Distance(transform.position, bodyBeingOrbited.transform.position);                 //makes ship go slower the further from center of it is (the longer for the bodyBeingOrbited)
        rotationalSpeed = (2 * Mathf.PI) * speed / 10000 * radius / gravitationalPull;          //rotational speed, affected by gravitationalPull, speed and radius
        if (Input.touchCount > 0)
        {
            if (firstBurn)
            {
                riktningsvinkel = Mathf.PI + (Mathf.Deg2Rad * Vector2.Angle(transform.position, bodyBeingOrbited.transform.position));  //for future use to rotate ellipse (so far it doesnt work)
                firstBurn = false;                                                                         //makes sure only first FixedUpdate() of burn check for angle 
            }
            Burn();                                                       //see trajectoryTransition()        
        }
        else
        {
            firstBurn = true;
            angle += rotationalSpeed;
            
            transform.localPosition = new Vector2(Mathf.Cos(angle) * radiusA + ellipscenter.x,  //farkostens koordinater baserat på ellipsens ekvation
                Mathf.Sin(angle) * radiusB + ellipscenter.y);
            transform.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * rotationalSpeed));               //farkostens rotation kring egen axel (z)
        }
    }
    */
    void EstablishNewOrbit ()                                                                   //made to establish a new orbit if celestialInsideTrajectory() == true
    {

    }

    bool CelestialInsideTrajectory()    
    {
        return true;
    }
    

    void Burn()                                                     //increases radiusB
    {
        radiusB *= 1.02f;
        radiusA *= 1.002f;
        TrajectoryRenderer(ellipscenter, radiusA, radiusB);    
    }

    void TrajectoryRenderer(Vector2 ellipscenter, float radiusA, float radiusB)                                      
    {
        trajectoryPositions = new Vector3[361];
        trajectoryLine.SetVertexCount(361);
        for (int i = 0; i < trajectoryPositions.Length; i++)
        {
            trajectoryPositions[i] = new Vector3(radiusA* Mathf.Cos((i + 1) * Mathf.Deg2Rad) + ellipscenter.x,
                radiusB * Mathf.Sin((i + 1) * Mathf.Deg2Rad)  + ellipscenter.y);
        }
        trajectoryPositions[360] = trajectoryPositions[0];
        trajectoryLine.SetPositions(trajectoryPositions);  
    }
}
