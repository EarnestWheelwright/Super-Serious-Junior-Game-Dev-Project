using System;
using System.Runtime.Serialization;
using UnityEditor.VersionControl;
using UnityEngine;

public class PlayerOrbit : MonoBehaviour {

    private Rigidbody rb;
    private Vector3 force, distance;

    [Header("Control factors")]
    [SerializeField] private float universalAttractionFactor = 15f;
    [SerializeField] private float spaceResistance = .00025f;
    [SerializeField] private float forceClamp = 500f;
    [SerializeField] private Vector2 initialVelocity = new Vector2(0,0);

    [Header("Attractors")]
    [SerializeField] private GameObject[] attractors = {
        
    };
    [SerializeField] private float[] gravityMultipliers = {
        
    };

    // Runs once
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Runs once
    void Start() {
        if (rb != null)
            rb.linearVelocity = new Vector3(initialVelocity.x,0,initialVelocity.y);
        else
            Debug.Log("failed to initialize velocity");
        for (int i = 0; i < gravityMultipliers.Length; i++) {
            gravityMultipliers[i] *= universalAttractionFactor;
        }
        universalAttractionFactor = 1;
    }

    // Update is called once per frame
    void Update() {
        for (int i = 0; i < attractors.Length; i++) {
            distance = attractors[i].transform.position - transform.position;
            force += gravityMultipliers[i] * distance.normalized / Vector3.SqrMagnitude(distance); // sum of inv squares
        }
        force -= spaceResistance * rb.linearVelocity;
        force *= Math.Min(1, forceClamp/force.magnitude);
        rb.AddForce(force); // add force to rigidbody
        force = new Vector3();
    }
}
