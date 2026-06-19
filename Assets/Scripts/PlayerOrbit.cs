using UnityEngine;

public class PlayerOrbit : MonoBehaviour {

    private Rigidbody rb;
    private Vector2 force;
    private Vector2[] attractors = {
        new Vector2()
    };

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        rb.AddForce(0,0,0);
    }
}
