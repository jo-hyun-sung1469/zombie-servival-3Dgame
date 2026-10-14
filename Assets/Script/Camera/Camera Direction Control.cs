using UnityEngine;
using UnityEngine.Serialization;

public class MouseLook : MonoBehaviour
{
    [FormerlySerializedAs("mouseSensitivity")]
    [SerializeField] private float _mouseSensitivity = 100f; // 마우스 민감도
    [FormerlySerializedAs("playerBody")]
    [SerializeField] private Transform _playerBodyTransform;        // 플레이어 몸통(부모 오브젝트)

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    void Start()
    {
        // 마우스 커서를 화면 중앙에 고정하고 숨김
        Cursor.lockState = CursorLockMode.Locked;
    }               

    void Update()
    {
        // 1. 마우스 입력 값 가져오기
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        // 2. 상하 회전 (Pitch): 위아래 90도 제한(Clamp)을 걸어 뒤집히지 않게 함
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _yRotation += mouseX; // 좌우 누적
        // 카메라의 로컬 X축 회전 적용

        _playerBodyTransform.eulerAngles = new Vector3(_xRotation, _yRotation, 0f);
    }
}
