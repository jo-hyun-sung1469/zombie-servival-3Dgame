using UnityEngine;
using UnityEngine.UIElements;

public class LoginAndRegisterUI : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject basePanel;

    public void OnShowLoginUI()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        basePanel.SetActive(false);
    }

    public void OnShowRegisterUI()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        basePanel.SetActive(false);
    }
}
