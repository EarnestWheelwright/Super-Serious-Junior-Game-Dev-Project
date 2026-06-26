using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollower : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Vector2 mousePos;
    public float verticalScrollableBounds;
    [SerializeField] private int upperVerticalScrollableBound;
    [SerializeField] private int lowerVerticalScrollableBound;
    public float horizontalScrollableBounds;
    [SerializeField] private int rightHorizontalScrollableBound;
    [SerializeField] private int leftHorizontalScrollableBound;
    public float maxHorizontalPan;
    public float maxVerticalPan;
    public Transform parent;
    [SerializeField] private float hOffset;
    [SerializeField] private bool aiming;
    [SerializeField] private float snappingSpeed;
    private float previousFrames = 0;
    private float currentFrames = 0;
    private float frameConstant = 10;
    public float frameUpdateTime;
    void Start()
    {
        Application.targetFrameRate = 60;
        verticalScrollableBounds = Mathf.Clamp(verticalScrollableBounds, 0, 1);
        horizontalScrollableBounds = Mathf.Clamp(horizontalScrollableBounds, 0, 1);
        float midHeight = Screen.currentResolution.height / 2;
        float midWidth = Screen.currentResolution.width / 2;
        upperVerticalScrollableBound = (int)(midHeight * 2 - midHeight * verticalScrollableBounds);
        lowerVerticalScrollableBound = (int)(midHeight * verticalScrollableBounds);
        rightHorizontalScrollableBound = (int)(midWidth * 2 - midWidth * horizontalScrollableBounds);
        leftHorizontalScrollableBound = (int)(midWidth * verticalScrollableBounds);
        hOffset = transform.position.y;
        StartCoroutine(UpdateFramerate());
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        UpdateCameraPosition(mousePos.x, mousePos.y);
        transform.eulerAngles = new Vector3(90, 0, 0);
    }

    private void UpdateCameraPosition(float xPos, float yPos)
    {
        float y = parent.position.z;
        float x = parent.position.x;
        float h = parent.position.y + hOffset;
        if (aiming)
        {
            y += UpdateVerticalCameraPosition(yPos);
            x += UpdateHorizontalCameraPosition(xPos);
        }
        transform.position += (new Vector3(x, h, y) - transform.position) * frameConstant;
    }
    private float UpdateVerticalCameraPosition(float yPos)
    {
        float verticalPan;
        if (yPos < lowerVerticalScrollableBound)
        {
            verticalPan = -(maxVerticalPan * PercentScroll(lowerVerticalScrollableBound, 0, yPos));
        }
        else if (yPos > upperVerticalScrollableBound)
        {
            verticalPan = maxVerticalPan * PercentScroll(upperVerticalScrollableBound, Screen.height, yPos);
        }
        else
        {
            verticalPan = 0;
        }
        return verticalPan;
    }
    private float UpdateHorizontalCameraPosition(float xPos)
    {
        float horizontalPan;
        if (xPos < leftHorizontalScrollableBound)
        {
            horizontalPan = -(maxHorizontalPan * PercentScroll(leftHorizontalScrollableBound, 0, xPos));
        }
        else if (xPos > rightHorizontalScrollableBound)
        {
            horizontalPan = maxHorizontalPan * PercentScroll(rightHorizontalScrollableBound, Screen.width, xPos);
        }
        else
        {
            horizontalPan = 0;
        }
        return horizontalPan;
    }
    private float PercentScroll(float innerBoundary, float outerBoundary, float pos)
    {
        float percent = Mathf.Abs((pos - innerBoundary) / (outerBoundary - innerBoundary));
        return percent;
    }

    public void onAimCamera(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            aiming = true;
        }
        else if(context.canceled)
        {
            aiming = false;
        }

    }
    IEnumerator UpdateFramerate()
    {
        float framerate = 60;
        while(true)
        {
            previousFrames = (float) currentFrames;
            currentFrames = Time.frameCount;
            framerate = (currentFrames - previousFrames) / frameUpdateTime;
            frameConstant = Mathf.Pow(snappingSpeed, -snappingSpeed / framerate);
            Debug.Log("framerate: " + framerate);
            yield return new WaitForSeconds(frameUpdateTime);
        }
    }
}
