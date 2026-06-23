using UnityEngine;

public class IdleSpin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Transform thisObject;
    //degrees per second
    public Vector3 rotSpeed;
    public float bobsPerSecond;
    public float bobAmplitude;
    public Vector3 startingRotationModifier;
    //Value between 0 to 1 for percentage through bobbing
    public float startingBobPosition;
    private Vector3 originalPosition;
    void Start()
    {
        thisObject = this.gameObject.transform;
        originalPosition = thisObject.position;
        thisObject.Rotate(startingRotationModifier);
    }

    // Update is called once per frame
    void Update()
    {
        thisObject.Rotate(rotSpeed * Time.deltaTime);
        thisObject.SetPositionAndRotation(new Vector3(thisObject.position.x, originalPosition.y + Mathf.Sin(2 * bobsPerSecond * Time.time + startingBobPosition * 2 * Mathf.PI) * bobAmplitude, thisObject.position.z), thisObject.rotation);
    }
}
