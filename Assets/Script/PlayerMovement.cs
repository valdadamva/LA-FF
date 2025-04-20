using UnityEngine;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))] // Гарантирует наличие Rigidbody
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
    [SyncVar(hook = nameof(OnRunningChanged))] 
    private bool _isRunning;
    [SyncVar(hook = nameof(OnVelocityChanged))]
    private Vector3 _syncVelocity;
    [SyncVar(hook = nameof(OnRotationChanged))]
    private Quaternion _syncRotation;

    private Rigidbody _rigidbody;
    private bool _isGrounded;
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    public bool IsRunning => _isRunning;
    private bool movementEnabled = true;

    void Awake()
    {
        InitializeRigidbody();
    }

    private void InitializeRigidbody()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing!", this);
            enabled = false;
            return;
        }
        
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        if (!movementEnabled) return;
        HandleRunningInput();
        HandleJumpInput();
    }

    private void HandleRunningInput()
    {
        bool newRunningState = canRun && Input.GetKey(runningKey);
        if (_isRunning != newRunningState)
        {
            CmdSetRunning(newRunningState);
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isGrounded)
                CmdJump();
            else
            {
                Debug.LogError("Player not jumped!");
            }
        }
       
    }

    [Command]
    private void CmdSetRunning(bool runningState)
    {
        _isRunning = runningState;
    }

    [Command]
    private void CmdJump()
    {
        _isGrounded = false;
        RpcJump();
    }

    [ClientRpc]
    private void RpcJump()
    {
        if (_rigidbody != null)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void CheckGrounded()
    {
        float sphereRadius = 0.4f;
        float checkDistance = 0.3f;
    
        _isGrounded = Physics.SphereCast(
            transform.position + Vector3.up * 0.1f,
            sphereRadius,
            Vector3.down,
            out RaycastHit hit,
            checkDistance,
            groundLayer
        );

        // Визуализация луча
        Debug.DrawRay(transform.position, Vector3.down * (checkDistance + 0.1f), 
            _isGrounded ? Color.green : Color.red);
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
        CheckGrounded(); // Постоянная проверка земли
        
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
        
        Vector3 velocity = transform.TransformDirection(new Vector3(
            input.x * speed,
            _rigidbody.linearVelocity.y,
            input.y * speed
        ));

        _rigidbody.linearVelocity = velocity;
        CmdUpdateMovement(new Vector2(velocity.x, velocity.z), transform.rotation);
    }

    private float GetCurrentSpeed()
    {
        if (speedOverrides.Count > 0)
            return speedOverrides[speedOverrides.Count - 1]();
        return _isRunning ? runSpeed : moveSpeed;
    }

    private Vector2 GetMovementInput()
    {
        return new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

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
            // Обнуляем горизонтальную скорость при блокировке движения
            _rigidbody.linearVelocity = new Vector3(0, _rigidbody.linearVelocity.y, 0);
        }
    }

    // Network sync hooks
    private void OnRunningChanged(bool oldValue, bool newValue) => _isRunning = newValue;
    private void OnVelocityChanged(Vector3 oldValue, Vector3 newValue) => _syncVelocity = newValue;
    private void OnRotationChanged(Quaternion oldValue, Quaternion newValue) => _syncRotation = newValue;
}