using System;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    
    [Header("Initial Factors")]
    [SerializeField] private OrbitBody body;
    [SerializeField] private Vector2 velocity;
    /*
     * 1 - orbiting a planet
     * 2 - transferring orbits
     * 3 - hyperbolic orbit
     * 4 - flinging
     */
    [SerializeField] private int orbitState;

    [SerializeField] private float flingingAcceleration = 2.0f;

    [SerializeField] private bool pointTowardsVelocity = false;

    private float orbitCapturePercent = 0;

    public float orbitDistance;

    public int flinging = 0;
    
    // Time between two simulated frames (calibrated to be ~= to real time for 60fps)
    private readonly float DT = 0.015f;

    // Return 2d position of the controlled object
    public Vector2 position {
        get {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    public float distance {
        get {
            return Math.Abs((this.position - body.position).magnitude);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (body == null)
            orbitState = 2;
        
        if (velocity == null)
            velocity = new Vector2(0,0);
        
        if (orbitState == 1 && !CheckOrbitViability())
            orbitDistance = .5f * (body.orbitMinRadius + body.orbitMaxRadius);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 deltaPos = velocity * DT, pos = new Vector2(transform.position.x, transform.position.z);
        switch (orbitState) {
        case 1 : // steady state orbit/capture
            pos -= body.position;
            deltaPos += .01f * (orbitDistance * pos.normalized - pos);
            velocity += .5f * orbitCapturePercent * (((float)Math.Sqrt(body.keplerConstant/pos.magnitude)) * (velocity - pos*Vector2.Dot(pos,velocity)/pos.sqrMagnitude).normalized - velocity);
            if (orbitCapturePercent < 1) orbitCapturePercent += .0005f;
            if (orbitDistance < body.orbitMinRadius || orbitDistance > body.orbitMaxRadius) orbitState = 3;
            if (flinging != 0) orbitState = 4;
            break;
        case 2 : // outside of any SOI
            if (null == (body = GetNearestGravityWell()) || SqDistanceToSOI(body) > 0)
                break;
            orbitDistance = distance;
            if (CheckOrbitViability()) {
                orbitState = 1; // orbital parameters within acceptable bounds => capture into orbit
                orbitCapturePercent = 0;
            } else 
                orbitState = 3; // orbital parameters out of tolerance => initiate hyperbolic trajectory or re-entry
            break;
        case 3 : // hyperbolic or suborbital trajectory
            float a = body.keplerConstant/(body.position-pos).sqrMagnitude;
            velocity += DT*(body.position - pos).normalized * a;
            orbitDistance = distance;
            if (SqDistanceToSOI(body) > 0)
                orbitState = 2; // exited soi => enter freefloat
            else if (CheckOrbitViability()) {
                orbitState = 1; // orbital parameters became acceptable => capture into orbit
                orbitCapturePercent = 0;
            } else if (orbitDistance < body.orbitMinRadius) {
                velocity *= .95f; //re-entering => obliterate velocity
                pointTowardsVelocity = false; //lose direction control
                // TODO: kill the player here
            }
            break;
        case 4 : // flinging
            pos -= body.position;
            deltaPos += .1f * (orbitDistance * pos.normalized - pos);
            velocity -= pos*Vector2.Dot(pos,velocity)/pos.sqrMagnitude;
            switch (flinging) {
            case -1 :
                velocity += flingingAcceleration*DT * new Vector2(-pos.y,pos.x).normalized;
                break;
            case 1 :
                velocity -= flingingAcceleration*DT * new Vector2(-pos.y,pos.x).normalized;
                break;
            case 0 :
                orbitState = 3;
                break;
            }
            break;
        default:
            orbitState = 2;
            break;
        }
        transform.position += new Vector3(deltaPos.x, 0, deltaPos.y);
        if (pointTowardsVelocity)
            transform.eulerAngles = new Vector3(0,57.295f * (float) Math.Atan2(velocity.x, velocity.y),0);
    }

    // returns true when the point is in a viable orbit configuration
    private bool CheckOrbitViability() {
        //check position is in acceptable range and velocity is in acceptable range
        return orbitDistance > body.orbitMinRadius && orbitDistance < body.orbitMaxRadius && Math.Abs(velocity.magnitude - Math.Sqrt(body.keplerConstant/orbitDistance)) < body.orbitSpeedTolerance;
    }

    // returns the OrbitBody of the closest attractive object
    private OrbitBody GetNearestGravityWell() {
        OrbitBody closest = null;
        float minDist = float.MaxValue, temp;
        // iterate through list of all instantiated OrbitBodies, if sqdist is smaller then use it as new minimum
        foreach(OrbitBody body in OrbitBody.orbitBodies) if ((temp = SqDistanceToSOI(body)) < minDist) {
            minDist = temp;
            closest = body;
        }
        return closest;
    }

    private float SqDistanceToSOI(OrbitBody body) {
        return (body.position3D - this.transform.position).sqrMagnitude - body.SOIRadius*body.SOIRadius;
    }
}
