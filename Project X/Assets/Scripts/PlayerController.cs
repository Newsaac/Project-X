using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    private bool isWallSliding;
    private bool isWallJumping;
    private Vector3 wallJumpingDirection;
    private Vector3 wallNormalSum;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;


    [Header("General Movement")]
    [SerializeField] Camera playerCamera;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed = 12f;
    [SerializeField] float jumpPower = 7f;
    [SerializeField] float gravity = 10f;

    [Header("Camera Movement")]
    [SerializeField] float lookSpeed = 2f;
    [SerializeField] float lookXLimit = 45f;

    [Header("Wall Jumping")]
    [SerializeField] float wallSlidingSpeed = 0.5f;
    [SerializeField] Vector3 wallJumpingPower = new Vector3(8f, 8f, 8f);
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Transform wallCheck;

    CharacterController characterController;
    GameManager gameManager;
    ControlsSerializable controls;
    void Start() {
        characterController = GetComponent<CharacterController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        controls = gameManager.controls;
    }

    void Update() {
        if (gameManager.gameOver)
            return;
   
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        // Press Left Shift to run
        if (!isWallJumping) {
            #region Handles Movement

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(controls.sprint);
            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * vertical : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * horizontal : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            #endregion

            #region Handles Jumping
            if (!isWallJumping) {
                if (Input.GetKey(controls.jump) && canMove && characterController.isGrounded) {
                    moveDirection.y = jumpPower;
                }
                else {
                    moveDirection.y = movementDirectionY;
                }
            }
        }

        if (!characterController.isGrounded) {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation

        if (canMove) {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        #endregion

        WallSlide();
        WallJump();

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private bool IsWalled() {
        Collider[] colliders = Physics.OverlapSphere(wallCheck.position, characterController.radius + characterController.skinWidth, wallLayer);
        int wallCnt = 0;
        wallNormalSum = Vector3.zero;

        foreach (Collider collider in colliders) {
            if(collider.gameObject.tag == "Wall") {
                wallCnt++;
                Vector3 closestPoint = Physics.ClosestPoint(wallCheck.position, collider, collider.transform.position, collider.transform.rotation);
                Vector3 wallNormal = (wallCheck.position - closestPoint).normalized;
                wallNormalSum += wallNormal;
            }
        }
        wallNormalSum.Normalize();
        return wallCnt > 0;
    }

    private void WallSlide() {
        if (IsWalled() && !characterController.isGrounded && (horizontal != 0f || vertical != 0f)) {
            isWallSliding = true;
            moveDirection = new Vector3(moveDirection.x, Mathf.Clamp(moveDirection.y, -wallSlidingSpeed, float.MaxValue), moveDirection.z);
        }
        else {
            isWallSliding = false;
        }
    }

    private void WallJump() {
        if(isWallSliding) {
            isWallJumping = false;
            wallJumpingCounter = wallJumpingTime;

            wallJumpingDirection = (transform.TransformDirection(Vector3.forward).normalized + wallNormalSum).normalized;

            CancelInvoke(nameof(StopWallJumping));
        }
        else {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButton("Jump") && wallJumpingCounter > 0f) {
            isWallJumping = true;
            moveDirection = new Vector3(wallJumpingDirection.x * wallJumpingPower.x, wallJumpingPower.y, wallJumpingDirection.z * wallJumpingPower.z);
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() {
        isWallJumping = false;
    }
}
