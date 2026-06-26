using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    public float scaleMultiplier = 1.2f;
    private RocketController rocket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rocket = transform.parent.gameObject.GetComponent<RocketController>();
        transform.localScale = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        if (rocket.wrapLength != 0) {
            Vector3 wp = rocket.wrapPointAbs;
            transform.position = (wp + rocket.transform.position)/2;
            wp -= rocket.transform.position;
            transform.eulerAngles = new Vector3(90, 90 - 57.296f * Mathf.Atan2(wp.z, wp.x),0);
            transform.localScale = new Vector3(.5f, scaleMultiplier * wp.magnitude, .5f);
        } else {
            transform.localPosition = new Vector3();
            transform.localScale = new Vector3();
        }
    }
}
