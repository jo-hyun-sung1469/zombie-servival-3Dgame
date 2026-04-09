using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float spritSpeed = 2f;//스프린트시 배울로 적용

    private Rigidbody _rigidbody;
    [SerializeField] private SteminaAndHp _steminaAndHp;
    [SerializeField] private Transform cameraTransform; // 카메라 연결
    Vector3 dir;
    private bool isJumped { get; set; } = false;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _rigidbody.linearDamping = 1f;
    }

    private void FixedUpdate()  
    {
        dir = Vector3.zero;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; // ← 이게 하늘로 올라가는 문제 해결
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (Input.GetKey(KeyCode.W)) dir += forward;
        if (Input.GetKey(KeyCode.S)) dir += -forward;
        if (Input.GetKey(KeyCode.D)) dir += right;
        if (Input.GetKey(KeyCode.A)) dir += -right;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _rigidbody.AddForce(dir * moveSpeed * spritSpeed, ForceMode.Force);
            _steminaAndHp.IsSprinting = true;
        }
        else
        {
            _rigidbody.AddForce(dir * moveSpeed, ForceMode.Force);
            _steminaAndHp.IsSprinting = false;
        }

        if (dir != Vector3.zero)
        {
            _rigidbody.AddForce(dir * moveSpeed, ForceMode.Acceleration);
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, new Vector3(0, _rigidbody.linearVelocity.y, 0), Time.fixedDeltaTime * 75f);
        }

        Vector3 flat = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

        if (flat.magnitude > maxSpeed)
        {
            Vector3 limited = flat.normalized * maxSpeed;
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
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
