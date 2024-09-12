using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Rigidbody rigidbody;
    private Animator animator;
    private float gameOverHeight = -5f;
    [SerializeField] private float outOfView = 3.4f;  
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHeight = 2f; 
    [SerializeField] private LayerMask floorLayer;
    public bool isSliding;
    private CapsuleCollider capsuleCollider;
    private BoxCollider boxCollider;
    private bool isGrounded = true;
    private bool finishedJump = false;  
    
    [SerializeField] private RotateCamera rotateCamera;


    private void Awake() 
    {
        if(!TryGetComponent<PlayerMovement>(out playerMovement))
        {
            Debug.LogError("PlayerMovement is NULL");
        }
        if(!TryGetComponent<Rigidbody>(out rigidbody))
        {
            Debug.LogError("Rigidbody is NULL");
        }
        if(!TryGetComponent<Animator>(out animator))
        {
            Debug.LogError("Animator is NULL");
        }
        if(!TryGetComponent<CapsuleCollider>(out capsuleCollider))
        {
            Debug.LogError("CapsuleCollider is NULL");
        }

        if(!TryGetComponent<BoxCollider>(out boxCollider))
        {
            Debug.LogError("BoxCollider is NULL");
        }
    }

    private void Update()
    {
        if(GameManager.Instance.isGameOver) return;

        if(PlayerOutsideFloor() || PlayerBelowFloor())
        {
            GameOver();
            return;
        }

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        // try to fix this later
        animator.SetFloat("forwardSpeed", playerMovement.GetSpeed());
      
        //if (!IsGrounded()) return;
        if(isSliding) return;

        if(!isGrounded && finishedJump)
        {
            IsGrounded();
            return;
        }

        Jump();
        Slide();
    }

    
    private bool PlayerBelowFloor()
    {
        return transform.position.y < gameOverHeight;
    }

    
    private bool PlayerOutsideFloor()
    {
        FaceDirection faceDirection = playerMovement.GetFaceDirection();
        float distanceToCamera = 0f;
        Vector3 cameraTargetPosition = rotateCamera.GetTargetPositionAfterTransition();
        
        switch (faceDirection)
        {
            case FaceDirection.faceForward:
            case FaceDirection.faceBackward:
                distanceToCamera = Mathf.Abs(cameraTargetPosition.z - transform.position.z);
                break;
            
            case FaceDirection.faceRight:
            case FaceDirection.faceLeft:
                distanceToCamera = Mathf.Abs(cameraTargetPosition.x - transform.position.x);
                break;
        }

        return distanceToCamera < outOfView;
    }

    private bool IsGrounded()
    {
        // Define offsets for the additional raycast positions
        Vector3 rayOriginCenter = transform.position; // Center ray
        Vector3 rayOriginLeft = transform.position + Vector3.left * 0.3f; // Left ray
        Vector3 rayOriginRight = transform.position + Vector3.right * 0.3f; // Right ray

        // Raycast downwards from each of the three positions
        bool isGroundedCenter = Physics.Raycast(rayOriginCenter, Vector3.down, jumpHeight, floorLayer);
        bool isGroundedLeft = Physics.Raycast(rayOriginLeft, Vector3.down, jumpHeight, floorLayer);
        bool isGroundedRight = Physics.Raycast(rayOriginRight, Vector3.down, jumpHeight, floorLayer);

        // Combine the results: return true only if at least one of the rays hit the ground
        isGrounded = isGroundedCenter || isGroundedLeft || isGroundedRight;

        // Update animator parameter
        bool isGroundedAndFinishedJump = isGrounded && finishedJump;
        animator.SetBool("isGrounded", isGroundedAndFinishedJump);
        //isGrounded = false;

        Debug.Log("Is Grounded: " + isGrounded);
        Debug.Log("Is Grounded: " + isGrounded);
        Debug.Log("Is Grounded: " + isGrounded);

        return isGrounded;
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        GameManager.Instance.GameOver();
        rigidbody.isKinematic = true;
        animator.enabled = false;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("isJumping");

            JumpResetParameters();
        }
    }

    private void JumpResetParameters()
    {
        animator.SetBool("isGrounded", false);
        isGrounded = finishedJump = false;
    }

    private void Slide()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded)
        {
            isSliding = true;
            animator.SetBool("isSliding", isSliding);
        }
    }

    // this event is getting called from the Running Slide animation
    void SlidingAnimationEvent()
    {
        isSliding = false;
        animator.SetBool("isSliding", false);
    }

    // this event is getting called from the Running Slide animation
    void StartSlidingAnimationSwitchCollidersEvent()
    {
        boxCollider.enabled = true;
        capsuleCollider.enabled = false;
    }

    // this event is getting called from the Running Slide animation
    void EndSlidingAnimationSwitchCollidersEvent()
    {
        boxCollider.enabled = false;
        capsuleCollider.enabled = true;
    }

    // this event is getting called from the Jump animation
    void FinishedJumpAnimationEvent()
    {
        finishedJump = true;
    }
}
