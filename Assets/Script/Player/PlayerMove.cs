using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [FormerlySerializedAs("moveSpeed")]
    [SerializeField] private float _moveSpeed = 20f;
    [FormerlySerializedAs("jumpForce")]
    [SerializeField] private float _jumpForce = 15f;
    [FormerlySerializedAs("maxSpeed")]
    [SerializeField] private float _maxSpeed = 30f;
    [FormerlySerializedAs("spritSpeed")]
    [SerializeField] private float _sprintSpeed = 2f;//스프린트시 배울로 적용

    private Rigidbody _rigidbody;
    [SerializeField] private SteminaAndHp _steminaAndHp;
    [FormerlySerializedAs("cameraTransform")]
    [SerializeField] private Transform _cameraTransform; // 카메라 연결
    Vector3 _moveDirection;
    private bool isJumped { get; set; } = false;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _rigidbody.linearDamping = 1f;
    }

    private void FixedUpdate()  
    {
        _moveDirection = Vector3.zero;

        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f; // ← 이게 하늘로 올라가는 문제 해결
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (Input.GetKey(KeyCode.W)) _moveDirection += forward;
        if (Input.GetKey(KeyCode.S)) _moveDirection += -forward;
        if (Input.GetKey(KeyCode.D)) _moveDirection += right;
        if (Input.GetKey(KeyCode.A)) _moveDirection += -right;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _rigidbody.AddForce(_moveDirection * _moveSpeed * _sprintSpeed, ForceMode.Force);
            _steminaAndHp.IsSprinting = true;
        }
        else
        {
            _rigidbody.AddForce(_moveDirection * _moveSpeed, ForceMode.Force);
            _steminaAndHp.IsSprinting = false;
        }

        if (_moveDirection != Vector3.zero)
        {
            _rigidbody.AddForce(_moveDirection * _moveSpeed, ForceMode.Acceleration);
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, new Vector3(0, _rigidbody.linearVelocity.y, 0), Time.fixedDeltaTime * 75f);
        }

        Vector3 flat = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

        if (flat.magnitude > _maxSpeed)
        {
            Vector3 limited = flat.normalized * _maxSpeed;
            _rigidbody.linearVelocity = new Vector3(limited.x, _rigidbody.linearVelocity.y, limited.z);
        }

        if (Input.GetKey(KeyCode.Space) && !isJumped)
            IsJumping();
    }

    private void IsRunning()
    {

    }

    private void IsJumping()
    {
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        isJumped = true;
    }


    //public void OnMove(InputAction.CallbackContext context)
    //{
    //    move = context.ReadValue<Vector3>();
    //}

    //public void OnJump(InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Performed)
    //    {
    //        _rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    //        isJumped = true;
    //    }
    //}

    public void OnCollisionEnter(Collision collision)
    {
        isJumped = false;
    }
}
