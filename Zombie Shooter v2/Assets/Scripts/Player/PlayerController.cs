using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    //really? proc vlastne?
    //predpoklada player transform.position.y = 0
    //float playerHeight = 2f;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    float movementMultiplier = 10f;
    [SerializeField] Vector3 stepRayLower = new Vector3(0, 0.1f);
    [SerializeField] Vector3 stepRayUpper = new Vector3(0, 0.4f);
    [SerializeField] Vector3 stepsForce = new Vector3(0f, 0.5f);
    bool isOnStairs = false;
    bool sprintPressed = false;

    [Header("Aiming and shooting")]
    [SerializeField] Rig RigRifleAim;
    [SerializeField] CinemachineVirtualCamera aimCam;
    [SerializeField] float blendToAimCamDuration = 0.1f;
    public static bool weaponDrawn { get; private set; }
    public Weapon.WeaponType activeWeaponType;
    bool isAiming = false;

    [Header("Speeds")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float slopeSlidingSpeed = 7f;

    [Header("Jumping and falling")]
    public float jumpForce = 5f;
    private float fallTimer = 0f;
    [SerializeField] float setFallAnimAtSec = 0.3f;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 0f;
    [SerializeField] float slideDrag = 0f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] LayerMask groundMask;
    float checkRadius;
    public bool isGrounded { get; private set; }

    [Header("Slope Control")]
    [SerializeField] float slopeLimitAngle = 45f;
    private enum slopeLevel { zero, mild, steep, notGrounded};
    private slopeLevel _slopeLevel;
    RaycastHit slopeHit;

    [Header("Animations")]
    [SerializeField] int indexEquipLayer;
    [SerializeField] float timeToEquipLayer;
    [SerializeField] float delayToWeaponLayer;
    [SerializeField] int indexWeaponLayer;
    [SerializeField] float timeToWeaponLayer;

    Vector2 moveInput;
    public Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;
    Animator playerAnimator;
    PlayerLook playerLook;
    RigBuilder rigBuilder;
    List<RigLayer> rigLayers;
    




    private void Start()
    {
        playerLook = GetComponent < PlayerLook>();
        playerAnimator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rigBuilder = GetComponentInChildren<RigBuilder>();
        rigLayers = rigBuilder.layers;
        rb.freezeRotation = true;
        checkRadius = playerCollider.radius * 0.9f;
        aimCam.enabled = false;
        weaponDrawn = false;
        // bo se  zmeni weight rigLayers[0].active = weaponDrawn;
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        MyInput();
        ControlDrag();
        ControlSpeed();

        

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        _slopeLevel = SlopeControl();
        //Debug.Log(_slopeLevel);
    }
    private void FixedUpdate()
    {
        playerLook.RotatePlayer();
        MovePlayer();
        AnimatePlayer();
    }
    void OnJump()
    {
        if (isGrounded)
        {
            Jump();
            //playerAnimator.SetBool("jump", true);
            playerAnimator.SetTrigger("jumped");
            playerAnimator.SetBool("inJump", true);

        }
    }
    void OnSprint(InputValue value)
    {
        sprintPressed = value.isPressed;
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnDrawWeapon()
    {
        weaponDrawn = !weaponDrawn;
        //tady spis rig weight z 0 na 1, ne pri aim tu udelam
        //rigLayers[0].active = weaponDrawn;
        AnimateWeaponDraw(weaponDrawn, activeWeaponType);
    }
    // !!!!!!!!!
    public void OnAltFire(InputValue value)
    {
        //disable for debug
        isAiming = value.isPressed;
        //enableis for debug
        //Aiming = true;
        if (weaponDrawn)
        {
            aimCam.enabled = isAiming;
            StartCoroutine(AimGunCoroutine(isAiming, RigRifleAim, blendToAimCamDuration));
            if (isAiming)
            {
                
            }
        }
    }



    bool IsGrounded()
    {
        Vector3 pos = transform.position + Vector3.up * (checkRadius * 0.9f);
        bool isGrounded = Physics.CheckSphere(pos, checkRadius, groundMask);
        return isGrounded;
    }

    void MyInput()
    {

        //horizontalMovement = Input.GetAxisRaw("Horizontal");
        //verticalMovement = Input.GetAxisRaw("Vertical");
        horizontalMovement = moveInput.x;
        verticalMovement = moveInput.y;

        moveDirection = gameObject.transform.forward * verticalMovement + gameObject.transform.right * horizontalMovement;
    }
    private slopeLevel SlopeControl()
    {
        //must be grounded
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out slopeHit, 2))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            //Debug.Log(slopeAngle);
            if (slopeHit.normal == Vector3.up) return slopeLevel.zero;
            else if (slopeLimitAngle < slopeAngle) return slopeLevel.steep;
            else return slopeLevel.mild;
        }
        else return slopeLevel.notGrounded;
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ControlSpeed()
    {
        if ((sprintPressed) && (isGrounded || isOnStairs) && _slopeLevel != slopeLevel.steep && verticalMovement > 0 && horizontalMovement < 0.1f)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
            playerAnimator.SetBool("sprintForward", true);
        }
        else if (_slopeLevel == slopeLevel.steep && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, slopeSlidingSpeed, acceleration * Time.deltaTime);
            playerAnimator.SetBool("sprintForward", false);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            playerAnimator.SetBool("sprintForward", false);
        }
    }

    void ControlDrag()
    {
        if (_slopeLevel == slopeLevel.steep) rb.drag = slideDrag;
        else if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    

    void MovePlayer()
    {
        if (isGrounded && (_slopeLevel == slopeLevel.zero || _slopeLevel == slopeLevel.notGrounded))
        {
            HandleStairs(moveDirection);
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

        }
        else if (isGrounded && _slopeLevel == slopeLevel.mild)
        {
            HandleStairs(slopeMoveDirection);
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (_slopeLevel == slopeLevel.steep)
        {
            SteepSlopeMovement();
        }

    }

    void SteepSlopeMovement()
    {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        Debug.Log(slopeDirection);
        float slideSpeed = slopeSlidingSpeed * Time.deltaTime;
        //!!!!
        Vector3 slideDirection = -slopeDirection + moveDirection;
        slideDirection.y = slideDirection.y - slopeHit.point.y;
        Debug.Log(slideDirection);

        rb.AddForce(slideDirection.normalized * slideSpeed * movementMultiplier, ForceMode.Acceleration);
    }

    void HandleStairs(Vector3 moveDirectionSteps)
    {
        Vector3 rayLower = transform.position + stepRayLower;
        Vector3 rayUpper = transform.position + stepRayUpper;
        Vector3 direction = moveDirectionSteps.normalized;
        Vector3 angle45 = new Vector3(0.5f, 0f);
        float hitDistance = 0.5f;

        if (isGrounded && (horizontalMovement != 0 || verticalMovement != 0))
        {
            RaycastHit hitLower;
            if (Physics.Raycast(rayLower, direction, out hitLower, hitDistance, groundMask))
            {
                RaycastHit hitUpper;
                if (!Physics.Raycast(rayUpper, direction, out hitUpper, hitDistance, groundMask))
                {
                    rb.AddForce(stepsForce, ForceMode.Impulse);
                    ResetIsOnStairs();
                    return;
                }
            }
            RaycastHit hitLowerPlus45;
            if (Physics.Raycast(rayLower, direction + angle45, out hitLowerPlus45, hitDistance, groundMask))
            {
                RaycastHit hitUpperPlus45;
                if (!Physics.Raycast(rayUpper, direction + angle45, out hitUpperPlus45, hitDistance, groundMask))
                {
                    rb.AddForce(stepsForce, ForceMode.Impulse);
                    ResetIsOnStairs();
                    return;
                }
            }
            RaycastHit hitLowerMinus45;
            if (Physics.Raycast(rayLower, direction - angle45, out hitLowerMinus45, hitDistance, groundMask))
            {
                RaycastHit hitUpperMinus45;
                if (!Physics.Raycast(rayUpper, direction - angle45, out hitUpperMinus45, hitDistance, groundMask))
                {
                    rb.AddForce(stepsForce, ForceMode.Impulse);
                    ResetIsOnStairs();
                    return;
                }
            }
        }
    }

    private void Landing()
    {
        playerAnimator.SetBool("fall", false);
    }

    void ResetIsOnStairs()
    {
        StopCoroutine(IsOnStairsCoroutine());
        StartCoroutine(IsOnStairsCoroutine());
    }

    IEnumerator IsOnStairsCoroutine()
    {
        isOnStairs = true;
        yield return new WaitForSeconds(setFallAnimAtSec);
        isOnStairs = false;
    }
    IEnumerator AimGunCoroutine(bool isAiming, Rig aimRig, float timeToAim)
    {
        float timer = 0f;
        if (isAiming)
        {
            aimRig.weight = 0;
            while (timer < timeToAim)
            {
                timer += Time.deltaTime;
                aimRig.weight += (Time.deltaTime / timeToAim);
                yield return null;
            }
            aimRig.weight = 1;
        }
        else
        {
            aimRig.weight = 1;
            while (timer < timeToAim)
            {
                timer += Time.deltaTime;
                aimRig.weight -= (Time.deltaTime / timeToAim);
                yield return null;
            }
            aimRig.weight = 0;
        }
       
    }

    // ANIMACE
    void AnimatePlayer()
    {
        playerAnimator.SetBool("grounded", isGrounded);
        if (isGrounded)
        {
            if (fallTimer > 0)
            {

                fallTimer = 0f;
                Invoke("Landing", 0.1f);
                playerAnimator.SetBool("inJump", false);

            }

            //playerAnimator.SetBool("jump", false);
            //playerAnimator.ResetTrigger("jumped");
        }
        else
        {
            fallTimer += Time.deltaTime;
            if (fallTimer > setFallAnimAtSec)
            {
                playerAnimator.SetBool("fall", true);
            }
        }
        //idle
        if (verticalMovement == 0 && horizontalMovement == 0) playerAnimator.SetBool("idle", true);
        else playerAnimator.SetBool("idle", false);

        //forward inc L+R
        if ((verticalMovement > 0.0f) && (horizontalMovement == 0.0f)) playerAnimator.SetBool("runForward", true);
        else playerAnimator.SetBool("runForward", false);

        if ((verticalMovement > 0.0f) && (horizontalMovement < 0.0f)) playerAnimator.SetBool("runLeft", true);
        else playerAnimator.SetBool("runLeft", false);

        if ((verticalMovement > 0.0f) && (horizontalMovement > 0.0f)) playerAnimator.SetBool("runRight", true);
        else playerAnimator.SetBool("runRight", false);

        //backward incl L+R
        if ((verticalMovement < 0.0f) && (horizontalMovement == 0.0f)) playerAnimator.SetBool("runBack", true);
        else playerAnimator.SetBool("runBack", false);

        if ((verticalMovement < 0.0f) && (horizontalMovement < 0.0f)) playerAnimator.SetBool("runBackLeft", true);
        else playerAnimator.SetBool("runBackLeft", false);

        if ((verticalMovement < 0.0f) && (horizontalMovement > 0.0f)) playerAnimator.SetBool("runBackRight", true);
        else playerAnimator.SetBool("runBackRight", false);

        //strafe L+R
        if ((verticalMovement == 0.0f) && (horizontalMovement < 0.0f)) playerAnimator.SetBool("strafeLeft", true);
        else playerAnimator.SetBool("strafeLeft", false);

        if ((verticalMovement == 0.0f) && (horizontalMovement > 0.0f)) playerAnimator.SetBool("strafeRight", true);
        else playerAnimator.SetBool("strafeRight", false);
    }
    void AnimateWeaponDraw(bool drawWeapon, Weapon.WeaponType activeWeaponType)
    {
        //jeste mixnout weights of layers
        if (drawWeapon)
        {
            switch (activeWeaponType)
            {
                case Weapon.WeaponType.pistol:
                    playerAnimator.SetTrigger("equipHip_R");
                    //StartCoroutine(WeaponDrawHandleAnimLayersCoroutine(indexEquipLayer, timeToEquipLayer, delayToWeaponLayer, indexWeaponLayer, timeToWeaponLayer));
                    break;

                case Weapon.WeaponType.rifle:
                    playerAnimator.SetTrigger("equipBack");
                    //StartCoroutine(WeaponDrawHandleAnimLayersCoroutine(indexEquipLayer, timeToEquipLayer, delayToWeaponLayer, indexWeaponLayer, timeToWeaponLayer));
                    break;
                default: break;
            }
        }
        else
        {
            switch (activeWeaponType)
            {
                case Weapon.WeaponType.pistol:
                    playerAnimator.SetTrigger("disarmHip_R");
                    break;

                case Weapon.WeaponType.rifle:
                    playerAnimator.SetTrigger("disarmBack");
                    break;
                default: break;
            }
        }
    }

    IEnumerator WeaponDrawHandleAnimLayersCoroutine(int indexEquipLayer, float timeToEquipLayer, float delayToWeaponLayer, int indexWeaponLayer, float timeToWeaponLayer)
    {
        float timer = 0f;
        float equipLayerWeight = 0;
        float weaponLayerWeight = 0;

        while (timer < (timeToEquipLayer + delayToWeaponLayer + timeToWeaponLayer))
        {
            timer += Time.deltaTime;
            if(timer < timeToEquipLayer)
            {
                equipLayerWeight += Time.deltaTime / timeToEquipLayer;
                playerAnimator.SetLayerWeight(indexEquipLayer, equipLayerWeight);
            }
            else if(timer < timeToEquipLayer + delayToWeaponLayer)
            {
                //nic
            }
            else
            {
                equipLayerWeight -= Time.deltaTime / timeToWeaponLayer;
                weaponLayerWeight += Time.deltaTime / timeToWeaponLayer;
                playerAnimator.SetLayerWeight(indexEquipLayer, equipLayerWeight);
                playerAnimator.SetLayerWeight(indexWeaponLayer, weaponLayerWeight);
            }
            yield return null;
        }
    }
}