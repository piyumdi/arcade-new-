
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Camera mainCamera;
    public float cameraFollowSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 5, -7);
    public Animator animator;

    private Vector3 targetPosition;
    private Vector3 inputDirection;
    private float currentSpeed;

    // Wobble parameters (from previous code)
    public float wobbleAmount = 2f;
    public float wobbleSpeed = 5f;
    private float wobbleTimeOffset;

    private FoodStackManager foodStackManager;

    //
    public Joystick joystick;
    //

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        wobbleTimeOffset = Random.Range(0f, 2f * Mathf.PI); // Random offset for wobble
        foodStackManager = GetComponent<FoodStackManager>(); // Link the food stack manager
    }

    void Update()
    {
        //HandleMouseMovement();   // Existing mouse movement
        HandleKeyboardMovement(); // Existing keyboard movement
        HandleJoystickMovement(); // New joystick movement

        HandleCameraFollow(); // Existing camera follow logic
        UpdateAnimation();
        ApplyWobbleEffect();  // Wobble effect when moving with food stack (if applicable)
    }


    void HandleMouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            }

            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction != Vector3.zero)
            {
                transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * moveSpeed);
            }

            currentSpeed = direction.magnitude * moveSpeed;
        }
    }

    void HandleKeyboardMovement()
    {
        inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            inputDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            inputDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            inputDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            inputDirection += transform.right;
        }

        if (inputDirection != Vector3.zero)
        {
            transform.position += inputDirection.normalized * moveSpeed * Time.deltaTime;
            transform.forward = Vector3.Lerp(transform.forward, inputDirection.normalized, Time.deltaTime * moveSpeed);
            currentSpeed = inputDirection.magnitude * moveSpeed;
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    

    void HandleCameraFollow()
    {
        // Adjust the camera offset relative to the player's rotation
        Vector3 targetCameraPosition = transform.position + transform.rotation * new Vector3(0, 10, -7); // Offset is relative to player's rotation

        // Smoothly move the camera to the target position
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, cameraFollowSpeed * Time.deltaTime);

        // Make the camera look slightly ahead of the player
        Vector3 lookAtPosition = transform.position + transform.forward * 5f; // Look 5 units ahead of the player
        mainCamera.transform.LookAt(lookAtPosition);
    }

   

    void HandleJoystickMovement()
    {
        // Get the joystick's input
        Vector3 inputDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        // Check if the joystick is moved
        if (inputDirection.magnitude > 0.1f) // Check if joystick is being moved
        {
            // Move the player based on joystick input
            transform.position += inputDirection.normalized * moveSpeed * Time.deltaTime;
            transform.forward = inputDirection.normalized; // Face the direction of movement

            // Set speed for animation
            currentSpeed = inputDirection.magnitude * moveSpeed;
        }
        else
        {
            // If no movement, set the speed to 0
            currentSpeed = 0f;
        }

        // Update the animation with the current speed
        animator.SetFloat("Speed", currentSpeed);

        // Check if carrying a stack from the FoodStackManager
        bool isCarryingStack = foodStackManager.IsCarryingStack();
        animator.SetBool("IsCarryingStack", isCarryingStack);

        // If not moving, ensure the player goes to idle or carrying state
        if (currentSpeed == 0f)
        {
            // Player should be idle if not carrying anything
            if (!isCarryingStack)
            {
                animator.SetFloat("Speed", 0f); // Trigger idle
            }
            // If carrying stack and not moving, it should go to 'Carrying' state
        }
    }



    void UpdateAnimation()
    {
        animator.SetFloat("Speed", currentSpeed);
        // Check if carrying stack from the FoodStackManager
        animator.SetBool("IsCarryingStack", foodStackManager.IsCarryingStack());
    }

    // Trigger detection for food collection and delivery
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PizzaRestaurant"))
        {
            foodStackManager.CollectFood("PizzaRestaurant");
        }
        else if (other.CompareTag("FriedChickenRestaurant"))
        {
            foodStackManager.CollectFood("FriedChickenRestaurant");
        }
        else if (other.CompareTag("SushiRestaurant"))
        {
            foodStackManager.CollectFood("SushiRestaurant");
        }
        else if (other.CompareTag("Bakery"))
        {
            foodStackManager.CollectFood("Bakery");
        }
        else if (other.CompareTag("FastFood"))
        {
            foodStackManager.CollectFood("FastFood");
        }

        if (other.CompareTag("Building"))
        {
            foodStackManager.StartFoodDelivery();
        }
    }

    // Apply wobble effect to the food stack when moving with a stack
    void ApplyWobbleEffect()
    {
        if (foodStackManager.IsCarryingStack() && currentSpeed > 0)
        {
            foodStackManager.ApplyWobbleEffect(wobbleAmount, wobbleSpeed, wobbleTimeOffset);
        }
    }
}
