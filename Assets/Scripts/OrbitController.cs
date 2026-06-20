using System;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    
    [Header("Initial Factors")]
    [SerializeField] private OrbitBody orbitingBody;
    [SerializeField] private Vector2 velocity;
    /*
     * 1 - orbiting a planet
     * 2 - transferring orbits
     * 3 - hyperbolic orbit
     * 4 - flinging
     */
    [SerializeField] private int orbitState;
    

    private readonly float DT = 0.015f; // artificial delta-t between updates

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (orbitingBody == null) orbitState = 2; 
        if (velocity == null) velocity = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 deltaPos = velocity * DT, pos = new Vector2(transform.position.x, transform.position.z);
        switch (orbitState) {
        case 1 : // steady state orbit/capture
            pos -= orbitingBody.position;
            deltaPos += .025f * (orbitingBody.orbitRadius * pos.normalized - pos);
            velocity += .025f * (orbitingBody.orbitSpeed * (velocity - pos*Vector2.Dot(pos,velocity)/pos.sqrMagnitude).normalized - velocity);
            break;
        case 2 : // outside of any SOI
            if (null == (orbitingBody = GetNearestGravityWell()) || SqDistanceToSOI(orbitingBody) > 0)
                break;
            if (CheckOrbitViability())
                orbitState = 1; // orbital parameters within acceptable bounds => capture into orbit
            else 
                orbitState = 3; // orbital parameters out of tolerance => initiate hyperbolic trajectory / re-entry
            break;
        case 3 : // hyperbolic or suborbital trajectory
            float a = orbitingBody.orbitSpeed*orbitingBody.orbitSpeed*orbitingBody.orbitRadius/(orbitingBody.position-pos).sqrMagnitude;
            velocity += DT*(orbitingBody.position - pos).normalized * a;
            if (SqDistanceToSOI(orbitingBody) > 0)
                orbitState = 2; // exited soi => enter freefloat
            else if (CheckOrbitViability())
                orbitState = 1; // orbital parameters became acceptable => capture into orbit
            break;
        case 4 : // flinging
            // TODO: implement flinging
            break;
        default:
            orbitState = 2;
            break;
        }
        transform.position += new Vector3(deltaPos.x, 0, deltaPos.y);
    }

    // returns true when the point is in a viable orbit configuration
    private bool CheckOrbitViability() {
        //check position is in acceptable range and velocity is in acceptable range
        return Math.Abs((new Vector2(transform.position.x, transform.position.z)-orbitingBody.position).magnitude - orbitingBody.orbitRadius) < orbitingBody.orbitRadiusTolerance && Math.Abs(velocity.magnitude - orbitingBody.orbitSpeed) < orbitingBody.orbitSpeedTolerance;
    }

    // returns the OrbitBody of the closest attractive object
    private OrbitBody GetNearestGravityWell() {
        OrbitBody closest = null;
        float minDist = float.MaxValue, temp;
        // iterate through list of all instantiated OrbitBodies, if sqdist is smaller then use it
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
