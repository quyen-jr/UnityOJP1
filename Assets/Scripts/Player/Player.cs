
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera mainCamera;
    public CinemachineFreeLook freeLookCamera;
    private Animator animator;
    private Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    #region animationParameter
    private float velocityX;
    private float velocityZ;
    private float acclecration = 2f;
    private float decleration = 2f;
    private float maximumWalkVelocity = 0.5f;
    private float maximumRunVelocity = 2f;

    private CharacterController controller;
    public bool isThirdCamera = true;
    #endregion

    [Header("First Person camera")]
    public Transform firstCamera;
    private float mouseSensivity = 2f;
    private float cameraVerticalRotation = 0f;

    void Start()
    {

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            isThirdCamera = true;
        }
        // switching cameara
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isThirdCamera)
            {
                mainCamera.gameObject.SetActive(false);
                freeLookCamera.gameObject.SetActive(false);
                firstCamera.gameObject.SetActive(true);
                isThirdCamera = false;

            }
            else
            {
                mainCamera.gameObject.SetActive(true);
                isThirdCamera = true;
                freeLookCamera.gameObject.SetActive(true);
                firstCamera.gameObject.SetActive(false);
            }
        }
        Animation();
        MoveCharacter();


    }

    private void Animation()
    {
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool runPressed = Input.GetKey("left shift");
        float currentVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        // accleration
        if (forwardPressed && velocityZ < currentVelocity)
        {
            velocityZ += Time.deltaTime * acclecration;
        }
        if (leftPressed && velocityX > -currentVelocity)
        {
            velocityX -= Time.deltaTime * acclecration;
        }
        if (rightPressed && velocityX < currentVelocity)
        {
            velocityX += Time.deltaTime * acclecration;
        }

        // decleration
        if (!forwardPressed && velocityZ > 0)
        {
            velocityZ -= Time.deltaTime * decleration;
        }
        if (!forwardPressed && velocityZ < 0)
        {
            velocityZ = 0;
        }
        if (!runPressed && velocityZ > 0.5)
        {
            velocityZ -= Time.deltaTime * decleration;
        }

        if (!leftPressed && velocityX < 0)
        {
            velocityX += Time.deltaTime * decleration;
        }
        if (!rightPressed && velocityX > 0)
        {
            velocityX -= Time.deltaTime * decleration;
        }
        if (!rightPressed && !leftPressed && velocityX != 0 && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0;
        }
        animator.SetFloat("velocityX", velocityX);
        animator.SetFloat("velocityZ", velocityZ);
    }

    private void MoveCharacter()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * velocityZ + transform.right * velocityX;
        if (controller.isGrounded)
        {
            moveDirection.y = -0.5f;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = 10;
            }
        }
        else
        {
            moveDirection.y += Physics.gravity.y * Time.deltaTime * 30f;
        }

        Vector3 velocity = moveDirection * moveSpeed;
        controller.Move(velocity * Time.deltaTime);
        Vector3 distanceToMove = transform.forward * velocityZ + transform.right * velocityX;

        if (distanceToMove != Vector3.zero && isThirdCamera)
        {
            Quaternion rotation = Quaternion.LookRotation(distanceToMove, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
        }
    }
}
