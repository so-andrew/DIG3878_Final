using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private bool useDragPan = true;
    [SerializeField] private float fieldOfViewMin = 30f;
    [SerializeField] private float fieldOfViewMax = 80f;

    public float moveSpeed = 50f;
    public float rotateSpeed = 100f;
    public float zoomSpeed = 10f;
    public float dragPanSpeed = 2f;
    public float minY = -7f;

    private bool dragPanMoveActive = false;
    private Vector2 lastMousePosition;
    private float targetFOV = 50f;


    void Update()
    {
        HandleCameraMovement();
        if (useDragPan)
        {
            DragPanMovement();
        }
        HandleCameraRotation();
        if (GameManager.Instance.CurrentMouseMode != MouseMode.UI)
        {
            HandleCameraZoom_MoveForward();
        }

    }
    private void HandleCameraMovement()
    {
        // Camera movement (arrow keys)
        Vector3 inputDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) inputDirection.z = 1f;
        if (Input.GetKey(KeyCode.S)) inputDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDirection.x = 1f;

        // Move the camera
        Vector3 moveDirection = (transform.forward * inputDirection.z) + (transform.right * inputDirection.x);
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void DragPanMovement()
    {
        Vector3 inputDirection = new Vector3(0, 0, 0);

        // Camera movement (click to drag)
        if (Input.GetMouseButtonDown(1))
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mousePosition;
            Debug.Log(lastMousePosition);
        }
        if (Input.GetMouseButtonUp(1))
        {
            dragPanMoveActive = false;
        }

        if (dragPanMoveActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;
            inputDirection.x = -1 * mouseMovementDelta.x * dragPanSpeed;
            inputDirection.z = -1 * mouseMovementDelta.y * dragPanSpeed;
            lastMousePosition = Input.mousePosition;
        }

        // Move the camera
        Vector3 moveDirection = (transform.forward * inputDirection.z) + (transform.right * inputDirection.x);
        transform.position += moveSpeed * Time.deltaTime * moveDirection;
    }

    private void HandleCameraRotation()
    {
        // Camera rotation
        float rotateDirection = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDirection += 1f;
        if (Input.GetKey(KeyCode.E)) rotateDirection -= 1f;

        transform.eulerAngles += new Vector3(0, rotateDirection * rotateSpeed * Time.deltaTime, 0);

    }

    private void HandleCameraZoom_FOV()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFOV -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFOV += 5;
        }
        targetFOV = Mathf.Clamp(targetFOV, fieldOfViewMin, fieldOfViewMax);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    private void HandleCameraZoom_MoveForward()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            cam.transform.position += Time.deltaTime * zoomSpeed * cam.transform.forward;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            cam.transform.position -= Time.deltaTime * zoomSpeed * cam.transform.forward;
        }
        if (cam.transform.position.y < minY) cam.transform.position = new Vector3(cam.transform.position.x, minY, cam.transform.position.z);
    }
}

