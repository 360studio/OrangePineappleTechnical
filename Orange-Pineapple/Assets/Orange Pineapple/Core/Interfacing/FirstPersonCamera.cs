using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
    float XSensitivity = 2;
    [SerializeField]
    float YSensitivity = 2;
    [SerializeField]
    bool smooth = true;
    [SerializeField]
    float smoothTime = 1f;

    [SerializeField]
    float minVertical = -80f;
    [SerializeField]
    float maxVertical = 80f;
    // Use this for initialization
    void Start()
    {
        targetRot = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
    }

    Quaternion targetRot;
    // Update is called once per frame
    void Update()
    {
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

        Vector3 eulers = targetRot.eulerAngles;
        
        targetRot = Quaternion.Euler(ClampAngle(eulers.x + -xRot, minVertical, maxVertical), eulers.y + yRot, 0);

        if (smooth)
        {
            transform.rotation = Quaternion.Lerp(transform.localRotation, targetRot,
                smoothTime * Time.deltaTime);
        } else
        {
            transform.rotation = targetRot;

        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = Cursor.lockState != CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public float ClampAngle(float angle, float min, float max)
    {
        bool minMoreThan = min > max;
        if (min < 0)
        {
            min = WrapAngle(min);
            if (min > max)
                minMoreThan = true;
        } else
            min = WrapAngle(min);

        max = WrapAngle(max);
        angle = WrapAngle(angle);

        if (minMoreThan == false)
        {
            if (angle < min || angle > max)
                angle = ClosestAngle (angle,min, max);
            
        } else
        {

            if (angle < min && angle > max) {
                angle = ClosestAngle (angle, min, max);
            }

        }
        return angle;
    }

    float ClosestAngle (float angle, float val1, float val2) {
        float dist1 = Mathf.Min (Mathf.Abs(angle - val1), Mathf.Abs(360 - val1 + angle));
        float dist2 = Mathf.Min (Mathf.Abs(angle - val2), Mathf.Abs(360 - val2 + angle));
        return (dist1 < dist2 ? val1 : val2);
    }

    //Make sure angle is within 0,360 range
    float WrapAngle(float angle)
    {
        //If its negative rotate until its positive
        if (angle < 0)
        {
            angle += 360;
            while (angle < 0)
                angle += 360;
        }
        else if (angle > 360)
        {
            angle %= 360;
        }
        return angle;
    }
}
