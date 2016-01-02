using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class FirstPersonCamera : MonoBehaviour {
    [SerializeField]
    float XSensitivity = 2;
    [SerializeField]
    float YSensitivity = 2;
    [SerializeField]
    bool smooth = true;
    [SerializeField]
    float smoothTime = 1f;
	// Use this for initialization
	void Start () {
        targetRot = transform.rotation;
	}
    Quaternion targetRot;
	// Update is called once per frame
	void Update () {
        float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
        float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

        Vector3 eulers = targetRot.eulerAngles;
        targetRot = Quaternion.Euler(eulers.x + -xRot, eulers.y + yRot, 0);
        if(smooth)
        {
            transform.rotation = Quaternion.Lerp (transform.localRotation, targetRot,
                smoothTime * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRot;

        }
	}
}
