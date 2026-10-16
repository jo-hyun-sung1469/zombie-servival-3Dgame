using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AuthSceneController : MonoBehaviour
{
    [FormerlySerializedAs("loginPanel")]
    [SerializeField] private GameObject _loginPanel;
    [FormerlySerializedAs("registerPanel")]
    [SerializeField] private GameObject _registerPanel;
    [FormerlySerializedAs("basePanel")]
    [SerializeField] private GameObject _basePanel;

    public void OnShowLoginUI()
    {
        _loginPanel.SetActive(true);
        _registerPanel.SetActive(false);
        _basePanel.SetActive(false);
    }

    public void OnShowRegisterUI()
    {
        _loginPanel.SetActive(false);
        _registerPanel.SetActive(true);
        _basePanel.SetActive(false);
    }

    public void OnShowStartUI()
    {
        _loginPanel.SetActive(false);
        _registerPanel.SetActive(false);
        _basePanel.SetActive(true);
    }
}
