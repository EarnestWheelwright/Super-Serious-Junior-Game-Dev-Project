using System;
using UnityEngine;

[RequireComponent(typeof(OrbitController))]
public class RocketController : MonoBehaviour
{
    private OrbitController orbit;
    public float wrapAngle {
        get;
        private set;
    }
    public Vector2 wrapPoint {
        get {
            Vector2 p = new Vector2(transform.position.x,transform.position.z) - orbit.body.position;
            float r1 = orbit.body.grappleRadius;
            float r2 = Mathf.Sign(wrapAngle) * Mathf.Sqrt(p.sqrMagnitude - r1*r1);
            return new Vector2(p.x*r1 - p.y*r2, p.x*r2 + p.y*r1) * r1/p.sqrMagnitude;
        }
    }


    public float fuel;
    public Vector2 throttle;
    public float usedGrappleLength;
    public float maxGrappleLength;
    public int grappleStatus;

    // Time passed between simulated frames
    private readonly float DT = .015f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orbit = GetComponent<OrbitController>();
        throttle = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        // actions on x throttle
        if (throttle.x != 0 && fuel > 0) switch (orbit.state) {
        case 1 :
            orbit.setDistance += throttle.x * DT;
            fuel -= throttle.x * DT;
            break;
        case 2 :
        case 3 :
            orbit.AddPerpVelocity(throttle.x * DT);
            fuel -= throttle.x * DT;
            break;
        case 4:
            // TODO : create whatever this should do
            // (should be like case 1 but no fuel use + new distance constraint)
            break;
        default :
            break;
        }

        // actions on y throttle
        if (throttle.y != 0 && fuel > 0) switch (orbit.state) {
        case 2 :
        case 3 :
            orbit.AddTangVelocity(throttle.y * DT);
            fuel -= throttle.x * DT;
            break;
        default:
            break;
        }
    }
}
