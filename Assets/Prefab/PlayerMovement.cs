using UnityEngine;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 9f;
    public KeyCode runningKey = KeyCode.LeftShift;
    public bool canRun = true;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    [Header("Network Sync")]
    [SyncVar(hook = nameof(OnRunningChanged))] private bool _isRunning;
    [SyncVar(hook = nameof(OnWalkingChanged))] private bool _isWalking;
    [SyncVar(hook = nameof(OnVelocityChanged))] private Vector3 _syncVelocity;
    [SyncVar(hook = nameof(OnRotationChanged))] private Quaternion _syncRotation;

    private Rigidbody _rigidbody;
    private bool _isGrounded;
    private Animator animator;
    private bool movementEnabled = true;
    public List<System.Func<float>> speedOverrides = new();

    public bool IsRunning => _isRunning;

    void Awake()
    {
        InitializeRigidbody();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void InitializeRigidbody()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (!isLocalPlayer || !movementEnabled) return;

        CheckGrounded();
        HandleRunningInput();
        HandleJumpInput();
        HandleWalkingInput();
    }

    private void HandleWalkingInput()
    {
        float vertical = Input.GetAxis("Vertical");
        bool walking = vertical > 0.1f;
        if (_isWalking != walking)
        {
            CmdSetWalking(walking);
        }
    }

    private void HandleRunningInput()
    {
        bool runningState = canRun && Input.GetKey(runningKey);
        if (_isRunning != runningState)
        {
            CmdSetRunning(runningState);
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            PerformJump();
            CmdNotifyJump();
        }
    }

    private void PerformJump()
    {
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (animator)
        {
            animator.SetTrigger("JumpUp");
            animator.SetBool("IsGrounded", false);
            animator.SetBool("IsJumping", true);
        }
        _isGrounded = false;
    }

    [Command]
    private void CmdSetWalking(bool walking) => _isWalking = walking;

    [Command]
    private void CmdSetRunning(bool running) => _isRunning = running;

    private void CmdNotifyJump() => RpcJump();

    [ClientRpc]
    private void RpcJump()
    {
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (animator != null)
        {
            animator.SetTrigger("JumpUp");
            animator.SetBool("IsGrounded", false);
            animator.SetBool("IsJumping", true);
        }
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.3f;
        bool wasGrounded = _isGrounded;

        _isGrounded = Physics.SphereCast(
            origin,
            0.2f,
            Vector3.down,
            out RaycastHit hit,
            0.5f,
            groundLayer
        );

        if (animator != null)
        {
            animator.SetBool("IsGrounded", _isGrounded);

            if (!wasGrounded && _isGrounded)
            {
                animator.SetTrigger("JumpDown");
                animator.SetBool("IsJumping", false);
            }
        }

        Debug.DrawRay(transform.position, Vector3.down * 1f, _isGrounded ? Color.green : Color.red, 1f);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        float sphereRadius = 0.4f;
        float checkDistance = 0.3f;
        Vector3 sphereBottom = transform.position + Vector3.down * (checkDistance - sphereRadius);

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(sphereBottom, sphereRadius);
    }
#endif

    void FixedUpdate()
    {
        CheckGrounded();

        if (isLocalPlayer)
        {
            HandleMovement();
        }
        else
        {
            ApplyRemoteMovement();
        }
    }

    private void HandleMovement()
    {
        if (_rigidbody == null) return;

        float speed = GetCurrentSpeed();
        Vector2 input = GetMovementInput();

        Vector3 moveDir = transform.TransformDirection(new Vector3(input.x, 0, input.y));
        Vector3 velocity = moveDir * speed;
        velocity.y = _rigidbody.linearVelocity.y;

        _rigidbody.linearVelocity = velocity;
        CmdUpdateMovement(new Vector2(velocity.x, velocity.z), transform.rotation);
    }

    private float GetCurrentSpeed() =>
        speedOverrides.Count > 0 ? speedOverrides[^1]() : (_isRunning ? runSpeed : moveSpeed);

    private Vector2 GetMovementInput() =>
        new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

    [Command]
    private void CmdUpdateMovement(Vector2 horizontalVelocity, Quaternion rotation)
    {
        _syncVelocity = new Vector3(horizontalVelocity.x, _rigidbody.linearVelocity.y, horizontalVelocity.y);
        _syncRotation = rotation;
        RpcUpdateMovement(horizontalVelocity, rotation);
    }

    [ClientRpc]
    private void RpcUpdateMovement(Vector2 horizontalVelocity, Quaternion rotation)
    {
        if (!isLocalPlayer && _rigidbody != null)
        {
            _syncVelocity = new Vector3(horizontalVelocity.x, _rigidbody.linearVelocity.y, horizontalVelocity.y);
            _syncRotation = rotation;
        }
    }

    private void ApplyRemoteMovement()
    {
        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = _syncVelocity;
            transform.rotation = _syncRotation;
        }
    }

    public void SetMovementEnabled(bool isEnabled)
    {
        movementEnabled = isEnabled;
        if (!isEnabled && _rigidbody != null)
        {
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        }
    }

    private void OnRunningChanged(bool oldValue, bool newValue)
    {
        _isRunning = newValue;
        if (animator) animator.SetBool("isRunning", newValue);
    }

    private void OnWalkingChanged(bool oldValue, bool newValue)
    {
        _isWalking = newValue;
        if (animator) animator.SetBool("isWalking", newValue);
    }

    private void OnVelocityChanged(Vector3 oldValue, Vector3 newValue) => _syncVelocity = newValue;
    private void OnRotationChanged(Quaternion oldValue, Quaternion newValue) => _syncRotation = newValue;
}