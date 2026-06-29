using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera loginCamera;
    [SerializeField] private GameObject ingameUI;
    [SerializeField] private GameObject loginUI;

    private GameManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        ShowLogin();
    }

    public void ShowLogin()
    {
        firstPersonCamera.gameObject.SetActive(false);
        loginCamera.gameObject.SetActive(true);

        ingameUI.SetActive(false);
        loginUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        loginCamera.gameObject.SetActive(false);
        firstPersonCamera.gameObject.SetActive(true);

        loginUI.SetActive(false);
        ingameUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
