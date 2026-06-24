using System;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    
    [Header("Initial Factors")]
    public OrbitBody body;
    [SerializeField] private Vector2 velocity;
    /*
     * 1 - orbiting a planet
     * 2 - transferring orbits
     * 3 - hyperbolic orbit
     * 4 - flinging
     */
    public int state;

    [SerializeField] private float flingingAcceleration = 2.0f;

    [SerializeField] private bool pointTowardsVelocity = false;

    private float capturePercent = 0;

    public float setDistance;

    public int flinging = 0;
    
    // Time between two simulated frames (calibrated to be ~= to real time for 60fps)
    private readonly float DT = 0.015f;

    // The X-Z worldspace position of the controller
    public Vector2 position {
        get {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    public float distance {
        get {
            return Mathf.Abs((this.position - body.position).magnitude);
        }
    }

    // Returns true while the controller is enduring re-entry or in atmosphere
    public bool isReentering {
        get {
            return setDistance < body.orbitMinRadius;
        }
    }

    public void AddPerpVelocity(float deltaV) {
        velocity += deltaV * new Vector2(-velocity.y, velocity.x).normalized;
    }
    public void AddTangVelocity(float deltaV) {
        velocity += deltaV * velocity.normalized;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (body == null)
            state = 2;
        
        if (velocity == null)
            velocity = new Vector2(0,0);
        
        if (state == 1 && !CheckOrbitViability())
            setDistance = .5f * (body.orbitMinRadius + body.orbitMaxRadius);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 deltaPos = velocity * DT, pos = new Vector2(transform.position.x, transform.position.z);
        switch (state) {
        case 1 : // steady state orbit/capture
            pos -= body.position;
            deltaPos += .01f * (setDistance * pos.normalized - pos);
            velocity += .5f * capturePercent * (Mathf.Sqrt(body.keplerConstant/pos.magnitude) * (velocity - pos*Vector2.Dot(pos,velocity)/pos.sqrMagnitude).normalized - velocity);
            if (capturePercent < 1) capturePercent += .0005f;
            if (setDistance < .95f*body.orbitMinRadius || setDistance > body.orbitMaxRadius) state = 3;
            if (flinging != 0) state = 4;
            break;
        case 2 : // outside of any SOI
            if (null == (body = GetNearestGravityWell()) || SqDistanceToSOI(body) > 0)
                break;
            setDistance = distance;
            if (CheckOrbitViability()) {
                state = 1; // orbital parameters within acceptable bounds => capture into orbit
                capturePercent = 0;
            } else 
                state = 3; // orbital parameters out of tolerance => initiate hyperbolic trajectory or re-entry
            break;
        case 3 : // hyperbolic or suborbital trajectory
            float a = body.keplerConstant/(body.position-pos).sqrMagnitude;
            velocity += DT*(body.position - pos).normalized * a;
            setDistance = distance;
            if (SqDistanceToSOI(body) > 0)
                state = 2; // exited soi => enter freefloat
            else if (CheckOrbitViability()) {
                state = 1; // orbital parameters became acceptable => capture into orbit
                capturePercent = 0;
            } else if (setDistance < body.orbitMinRadius) {
                velocity *= .95f; //re-entering => obliterate velocity
                pointTowardsVelocity = false; //lose direction control
                // TODO: kill the player here
            }
            break;
        case 4 : // flinging
            pos -= body.position;
            deltaPos += .1f * (setDistance * pos.normalized - pos);
            velocity -= pos*Vector2.Dot(pos,velocity)/pos.sqrMagnitude;
            if (setDistance < body.orbitMinRadius) velocity *= .95f;
            switch (flinging) {
            case -1 :
                velocity += flingingAcceleration*DT * new Vector2(-pos.y,pos.x).normalized;
                break;
            case 1 :
                velocity -= flingingAcceleration*DT * new Vector2(-pos.y,pos.x).normalized;
                break;
            default :
                state = 3;
                break;
            }
            break;
        default:
            state = 2;
            break;
        }
        transform.position += new Vector3(deltaPos.x, 0, deltaPos.y);
        if (pointTowardsVelocity)
            transform.eulerAngles = new Vector3(0,57.295f * Mathf.Atan2(velocity.x, velocity.y),0);
    }

    // returns true when the point is in a viable orbit configuration
    private bool CheckOrbitViability() {
        //check position is in acceptable range and velocity is in acceptable range
        return setDistance > body.orbitMinRadius && setDistance < body.orbitMaxRadius && Mathf.Abs(velocity.magnitude - Mathf.Sqrt(body.keplerConstant/setDistance)) < body.orbitSpeedTolerance;
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
