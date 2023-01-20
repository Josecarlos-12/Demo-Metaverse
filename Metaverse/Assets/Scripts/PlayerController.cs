using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;

public class PlayerController : NetworkBehaviour
{
    [Header("Base Setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    [Header("Animator setup")]
    public Animator anim;

    public override void OnStartClient( )
    {
        base.OnStartClient();
        if ( base.IsOwner )
        {
            playerCamera= Camera.main;
            playerCamera.transform.position=new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        //Lock cursor
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible= false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = false;

        //Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        //We are Grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right= transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? ( isRunning ? runningSpeed : walkingSpeed ) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? ( isRunning ? runningSpeed : walkingSpeed ) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = ( forward * curSpeedX ) + ( right * curSpeedY );

        if(Input.GetButton("Jump")&& canMove && characterController.isGrounded )
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if ( !characterController.isGrounded )
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        //Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        //Player and Camera Rotation
        if ( canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
