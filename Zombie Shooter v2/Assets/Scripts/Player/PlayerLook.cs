using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform camTarget = null;
    [SerializeField] Transform atScreenCentre;
    [SerializeField] Transform playerModel;
    public float sensX { get; private set; }
    public float sensY { get; private set; }

    PlayerController playerController;

    public Vector2 mouseInput { get; private set; }

    float multiplier = 0.001f;

    float xRotation;
    float yRotation;

    Vector3 screenCentreTarget;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // provizorium !!!!!!!!!!!, musi ale byt setnuty nekde pri awake, bo z nej cerpa free look pri startu
        sensX = 250f;
        sensY = 250f;
    }

    void OnLook(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }
    private void Update()
    {
        atScreenCentre.position = Vector3.Lerp(atScreenCentre.position, screenCentreTarget, Time.deltaTime * 20f);

        yRotation += mouseInput.x * sensX * multiplier;
        xRotation -= mouseInput.y * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camTarget.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        if (playerController.isMoving || playerController.isAiming)
        {
            playerModel.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }   
    }
    private void FixedUpdate()
    {
        screenCentreTarget = GetScreenCentre();
    }

    private Vector3 GetScreenCentre()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Physics.Raycast(ray, out RaycastHit hit);
        return hit.point;
    }

    public void RotatePlayer()
    {
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        playerModel.rotation = transform.rotation;
    }
}