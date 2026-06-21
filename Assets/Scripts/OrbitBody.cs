using System;
using System.Collections.Generic;
using UnityEngine;

public class OrbitBody : MonoBehaviour {
    // ideal distance that objects should orbit
    public float orbitMaxRadius = 0;

    // amount that distance can vary to still capture into orbit
    public float orbitMinRadius = 0;

    // radius of sphere of influence (leave 0 to set automatically based on gravity strength)
    public float SOIRadius = 0;

    // minimum gravity to autoset soi distance
    public float SOIGravity = 0.1f;

    // maximum speed that objects should (stable-ly) orbit
    public float orbitMaxSpeed = 0;

    // amount that velocity can vary from ideal to still capture into orbit
    public float orbitSpeedTolerance = 0;

    // list of all active OrbitBodies in the scene
    public static LinkedList<OrbitBody> orbitBodies = new LinkedList<OrbitBody>();

    // runs once per OrbitBody object
    void Start() {
        orbitBodies.AddFirst(this);
        if (SOIRadius <= orbitMaxRadius)
            SOIRadius = (float) Math.Sqrt(keplerConstant/SOIGravity);
    }

    public float keplerConstant {
        get {
            return orbitMaxSpeed*orbitMaxSpeed*orbitMinRadius;
        }
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
}
