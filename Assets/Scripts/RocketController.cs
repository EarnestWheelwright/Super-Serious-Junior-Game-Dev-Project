using System;
using UnityEngine;

[RequireComponent(typeof(OrbitController))]
public class RocketController : MonoBehaviour
{
    private OrbitController orbit;
    public float wrapAngle {
        get {
            return wrapLength / orbit.body.grappleRadius;
        }
    }
    public Vector2 wrapPoint {
        get {
            Vector2 p = new Vector2(transform.position.x,transform.position.z) - orbit.body.position;
            float r1 = orbit.body.grappleRadius;
            float r2 = Mathf.Sign(wrapAngle) * Mathf.Sqrt(p.sqrMagnitude - r1*r1);
            return new Vector2(p.x*r1 - p.y*r2, p.x*r2 + p.y*r1) * r1/p.sqrMagnitude;
        }
    }


    public float fuel = 100;
    public Vector2 throttle = new Vector2();
    public float wrapLength = 0;
    public float maxWrapLength = 100;
    public bool retractGrapple = true;

    // TODO: get rid of this test thing
    public bool unleashTheBeast = false;

    // Time passed between simulated frames
    private readonly float DT = .015f;

    public void StartGrapple() {
        retractGrapple = false;
    }
    public void PullGrapple() {
        retractGrapple = true;
    }

    public void DetachGrapple() {
        retractGrapple = true;
        wrapLength = 0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orbit = GetComponent<OrbitController>();
        throttle = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: get rid of this
        if (unleashTheBeast) DetachGrapple();

        // actions on x throttle
        if (throttle.x != 0 && fuel > 0) switch (orbit.state) {
        case 1 :
            orbit.setDistance += throttle.x * DT;
            fuel -= Mathf.Abs(throttle.x * DT);
            break;
        case 2 :
        case 3 :
            orbit.AddPerpVelocity(throttle.x * DT);
            fuel -= Mathf.Abs(throttle.x * DT);
            break;
        case 4 :
            // TODO : create whatever this should do
            // (should be like case 1 but no fuel use + new distance constraint)
            break;
        default:
            break;
        }

        // actions on y throttle
        if (throttle.y != 0 && fuel > 0) switch (orbit.state) {
        case 2 :
        case 3 :
            orbit.AddTangVelocity(throttle.y * DT);
            fuel -= Mathf.Abs(throttle.y * DT);
            break;
        default:
            break;
        }

        // actions on grapple
        if (orbit.state != 2) {
            float wrapChange = DT*orbit.tangVelocity*orbit.body.grappleRadius/orbit.distance;
            if (retractGrapple) {
                if (wrapLength != 0) orbit.flinging = (int) Mathf.Sign(wrapLength);
                else orbit.flinging = 0;
                float temp;
                if (Mathf.Sign(temp = wrapLength + wrapChange) != Mathf.Sign(wrapLength))
                    wrapLength = 0;
                else
                    wrapLength = temp;
            } else {
                wrapLength += wrapChange;
            }

            // TODO: make better max wrap functionality
            // if (Mathf.Abs(wrapLength) > maxWrapLength)
            //     orbit.AddTangVelocity(-orbit.tangVelocity);
        } else {
            wrapLength = 0;
        }
    }
}
