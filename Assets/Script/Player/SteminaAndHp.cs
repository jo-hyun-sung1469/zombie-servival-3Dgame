using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SteminaAndHp : MonoBehaviour
{
    [FormerlySerializedAs("stemina_bar")]
    public Slider StaminaSlider;
    [FormerlySerializedAs("maxHp")]
    [SerializeField] private float _maxHP = 100;
    [FormerlySerializedAs("hp")]
    [SerializeField] private float _currentHP = 100;
    [FormerlySerializedAs("maxStemina")]
    [SerializeField] private float _maxStamina = 100;
    [FormerlySerializedAs("stemina")]
    [SerializeField] private float _currentStamina = 100;
    public bool IsSprinting { get; set; } = false;
    void Start()
    {
        StaminaSlider.minValue = 0f;
        StaminaSlider.maxValue = 1f;
    }
    void Update()
    {
        RegenStemina();
    }
    private void RegenStemina()
    {
        if (IsSprinting && _currentStamina > 0)
        {
            _currentStamina -= 5 * Time.deltaTime;
        }
        else if (!IsSprinting && _currentStamina < _maxStamina) // ← 조건 수정
        {
            _currentStamina += 3 * Time.deltaTime;
        }

        _currentStamina = Mathf.Clamp(_currentStamina, 0, _maxStamina); // 0~100 범위 제한
        StaminaSlider.value = _currentStamina / _maxStamina;
    }
}