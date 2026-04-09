using UnityEngine;
using UnityEngine.UI;

public class SteminaAndHp : MonoBehaviour
{
    public Slider stemina_bar;
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float hp = 100;
    [SerializeField] private float maxStemina = 100;
    [SerializeField] private float stemina = 100;
    public bool IsSprinting { get; set; } = false;
    void Start()
    {
        stemina_bar.minValue = 0f;
        stemina_bar.maxValue = 1f;
    }
    void Update()
    {
        RegenStemina();
    }
    private void RegenStemina()
    {
        if (IsSprinting && stemina > 0)
        {
            stemina -= 5 * Time.deltaTime;
        }
        else if (!IsSprinting && stemina < maxStemina) // ← 조건 수정
        {
            stemina += 3 * Time.deltaTime;
        }

        stemina = Mathf.Clamp(stemina, 0, maxStemina); // 0~100 범위 제한
        stemina_bar.value = stemina / maxStemina;
    }
}