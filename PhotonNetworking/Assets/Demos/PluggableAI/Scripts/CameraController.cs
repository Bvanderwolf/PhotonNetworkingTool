using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Options")]
    [SerializeField]
    private bool enableArrowsPanning = true;

    [SerializeField]
    private bool enableWASDPanning = true;

    [SerializeField]
    private bool enableEdgePanning = false;

    [SerializeField]
    private bool enableQERotation = true;

    [SerializeField]
    private bool enableMouse3Rotation = true;

    [SerializeField]
    private bool invertQERotation = false;

    [SerializeField]
    private bool invertMouse3Rotation = false;

    [Header("Settings")]
    [SerializeField]
    private float panSpeed = 0.05f;

    [SerializeField]
    private float panSpeedMultiplier = 5;

    [SerializeField]
    private float camDistanceMin = 10f;

    [SerializeField]
    private float camDistanceMax = 50f;

    [SerializeField]
    private float camDistanceDefault = 30f;

    [SerializeField]
    private float camDistanceDelta = 3f;

    [SerializeField]
    private float camAngleMin = 30f;

    [SerializeField]
    private float camAngleMax = 60f;

    [SerializeField]
    private float camAngleDefault = 50f;

    [SerializeField]
    private float camAngleSensitivity = 1f;

    [SerializeField]
    private float camRotationDelta = 0.5f;

    [SerializeField]
    private float camRotationSensitivity = 2f;

    [Header("References")]
    [SerializeField]
    private Camera cam;

    private float camDistance = 0f;
    private float camAngle = 0f;
    private float camRotation = 0f;

    private void Start()
    {
        camDistance = camDistanceDefault;
        camAngle = camAngleDefault;
    }

    private void FixedUpdate()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            Scroll(-camDistanceDelta);
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            Scroll(camDistanceDelta);
        }
        
        if (enableQERotation)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                Rotate(camRotationDelta * (invertQERotation ? -1 : 1));
            }

            if (Input.GetKey(KeyCode.E))
            {
                Rotate(camRotationDelta * (invertQERotation ? 1 : -1));
            }
        }

        if (enableArrowsPanning)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Pan(Vector3.right);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Pan(Vector3.left);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Pan(Vector3.forward);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                Pan(Vector3.back);
            }
        }

        if (enableWASDPanning)
        {
            if (Input.GetKey(KeyCode.D))
            {
                Pan(Vector3.right);
            }

            if (Input.GetKey(KeyCode.A))
            {
                Pan(Vector3.left);
            }

            if (Input.GetKey(KeyCode.W))
            {
                Pan(Vector3.forward);
            }

            if (Input.GetKey(KeyCode.S))
            {
                Pan(Vector3.back);
            }
        }

        if (enableEdgePanning)
        {
            if (cam.ScreenToViewportPoint(Input.mousePosition).x > 1)
            {
                Pan(Vector3.right);
            }

            if (cam.ScreenToViewportPoint(Input.mousePosition).x < 0)
            {
                Pan(Vector3.left);
            }

            if (cam.ScreenToViewportPoint(Input.mousePosition).y > 1)
            {
                Pan(Vector3.forward);
            }

            if (cam.ScreenToViewportPoint(Input.mousePosition).y < 0)
            {
                Pan(Vector3.back);
            }
        }
    }

    private void LateUpdate()
    {
        if (enableMouse3Rotation)
        {
            if (Input.GetMouseButton(2))
            {
                Rotate(Input.GetAxis("Mouse X") * camRotationSensitivity * (invertMouse3Rotation ? -1 : 1));
                Tilt(Input.GetAxis("Mouse Y") * camAngleSensitivity * (invertMouse3Rotation ? 1 : -1));
            }
        }

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        Vector3 camPos = new Vector3(0f, Mathf.Sin(camAngle * Mathf.PI / 180) * camDistance, -(Mathf.Cos(camAngle * Mathf.PI / 180) * camDistance));
        cam.transform.localPosition = camPos;
        cam.transform.LookAt(transform);
    }

    private void Scroll(float distanceDelta)
    {
        camDistance += distanceDelta;
        camDistance = Mathf.Clamp(camDistance, camDistanceMin, camDistanceMax);
    }

    private void Rotate(float rotationDelta)
    {
        camRotation += rotationDelta;
        transform.rotation = Quaternion.Euler(0f, camRotation, 0f);
    }

    private void Tilt(float angleDelta)
    {
        camAngle += angleDelta;
        camAngle = Mathf.Clamp(camAngle, camAngleMin, camAngleMax);
    }

    private void Pan(Vector3 direction)
    {
        transform.Translate(direction * panSpeed * (Input.GetKey(KeyCode.LeftShift) ? panSpeedMultiplier : 1), Space.Self);
    }
}
