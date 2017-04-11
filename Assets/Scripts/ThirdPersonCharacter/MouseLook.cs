using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float hSpeed, vSpeed, vLimit;
    float yaw, pitch;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void LateUpdate() {
        yaw += hSpeed * Input.GetAxis("Mouse X");
        pitch -= vSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -vLimit, vLimit);

        transform.localEulerAngles = Vector3.right * pitch;
        transform.parent.eulerAngles = Vector3.up * yaw;
    }

}
