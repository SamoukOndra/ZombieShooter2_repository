using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform camTarget = null;
    [SerializeField] Transform atScreenCentre;
    [SerializeField] Transform playerModel;

    Vector2 mouseInput;

    float mouseX;
    float mouseY;

    float multiplier = 0.001f;

    float xRotation;
    float yRotation;

    Vector3 screenCentreTarget;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnLook(InputValue value)
    {
        mouseInput = value.Get<Vector2>();
    }
    private void Update()
    {

        //mouseX = Input.GetAxisRaw("Mouse X");
        //mouseY = Input.GetAxisRaw("Mouse Y");

        //yRotation += mouseX * sensX * multiplier;
        //xRotation -= mouseY * sensY * multiplier;

        yRotation += mouseInput.x * sensX * multiplier;
        xRotation -= mouseInput.y * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camTarget.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        playerModel.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        //transform.rotation = Quaternion.Euler(0, yRotation, 0);

        atScreenCentre.position = Vector3.Lerp(atScreenCentre.position, screenCentreTarget, Time.deltaTime * 20f);
        
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
        /*Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else return Vector3.zero;*/
    }

    public void RotatePlayer()
    {

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        playerModel.rotation = transform.rotation;
    }
}