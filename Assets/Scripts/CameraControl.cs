using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    public enum CameraState { FirstPerson, ThirdPerson };
    public CameraState cameraState = CameraState.FirstPerson;
    Camera playerCam;
    PlayerControl playerControl;
    float cameraDistance;
    public float zoomSpeed = 125f;
    public Vector3 cameraOffset = new Vector3(0.75f, 1.25f, 0f);


    public void Start() {
        playerCam = GetComponentInChildren<Camera>();
        playerControl = GetComponent<PlayerControl>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UpdateCameraPosition();
        cameraDistance = (cameraState == CameraState.ThirdPerson) ? 1 : 0;
    }


    public void LateUpdate() {

        cameraDistance = Mathf.Clamp(cameraDistance - Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed, 0f, 2f);
        if (cameraDistance <= 0.8) {
            cameraState = CameraState.FirstPerson;
        } else {
            cameraState = CameraState.ThirdPerson;
        }

        UpdateCameraPosition();
    }


    public void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }


    void UpdateCameraPosition() {

        switch (cameraState) {
            case CameraState.FirstPerson:
                FirstPersonCameraUpdate(playerCam);
                break;

            case CameraState.ThirdPerson:
                ThirdPersonCameraUpdate(playerCam);
                break;

            default:
                Debug.Log("Undefined Camera State!");
                break;
        }


    }


    void FirstPersonCameraUpdate(Camera playerCamera) {

        MeshRenderer playerMesh = GetComponent<MeshRenderer>();
        if (playerMesh.enabled) {
            playerMesh.enabled = false;
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
                mesh.enabled = false;
            }
        }

        playerCam.transform.localPosition = Vector3.zero;
        playerCam.transform.Rotate(Input.GetAxisRaw("Mouse Y") * playerControl.rotationSpeed * Vector3.left * Time.deltaTime);

        float xEulerAngle = AngleClamp(playerCam.transform.localEulerAngles.x, -75f, 75f);
        playerCam.transform.localEulerAngles = new Vector3(xEulerAngle, 0f, 0f);
    }


    void ThirdPersonCameraUpdate(Camera playerCamera) {

        MeshRenderer playerMesh = GetComponent<MeshRenderer>();
        if (!playerMesh.enabled) {
            playerMesh.enabled = true;
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) {
                mesh.enabled = true;
            }
        }

        // Vertical rotation
        playerCam.transform.Rotate(Input.GetAxisRaw("Mouse Y") * playerControl.rotationSpeed * Vector3.left * Time.deltaTime);

        // Clamping angles
        float xEulerAngle = AngleClamp(playerCam.transform.localEulerAngles.x, -75f, 75f);
        playerCam.transform.localEulerAngles = new Vector3(xEulerAngle, 0f, 0f);

        // Update Position
        Vector3 playerCameraPosition = new Vector3(0f, Mathf.Sin(xEulerAngle * Mathf.PI / 180f), -Mathf.Cos(xEulerAngle * Mathf.PI / 180f)) * cameraDistance * 3f + cameraOffset;
        playerCam.transform.localPosition = playerCameraPosition;

        //Check for colisions
        //Debug.DrawRay(transform.position, playerCam.transform.position - transform.position, Color.red);
        if (Physics.Linecast(transform.position, playerCam.transform.position, out RaycastHit hitInfo)) {
            //Debug.Log(hitInfo.distance);
            playerCam.transform.localPosition = playerCameraPosition.normalized * Mathf.Max(hitInfo.distance, 1.5f);
        }

    }


    float AngleClamp(float angle, float min, float max) {
        return Mathf.Clamp(angle > 180f ? angle - 360f : angle, min, max);
    }
}
