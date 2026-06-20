using System.Collections.Generic;
using UnityEngine;

public class OrbitBody : MonoBehaviour {
    // ideal distance that objects should orbit
    public float orbitRadius;

    // amount that distance can vary to still capture into orbit
    public float orbitRadiusTolerance;

    // ideal speed that objects should orbit
    public float orbitSpeed;

    // amount that velocity can vary to still capture into orbit
    public float orbitSpeedTolerance;

    // list of all active OrbitBodies in the scene
    public static LinkedList<OrbitBody> orbitBodies = new LinkedList<OrbitBody>();

    // runs once per OrbitBody object
    void Start() {
        orbitBodies.AddFirst(this);
    }
    
    // get position in x-z plane
    public Vector2 position {
        get {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }

    // get position as vec3
    public Vector3 position3D {
        get {
            return transform.position;
        }
    }

    // returns maximum radius stable orbits are possible within
    public float SOIRadius {
        get {
            return orbitRadius + orbitRadiusTolerance;
        }
    }
}
