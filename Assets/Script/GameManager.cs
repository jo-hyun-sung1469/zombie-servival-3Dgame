using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private LoginResponse _loginResponse;//LoginResponse에 있는 AssessToken을 저장&사용하기 위한 변수
    public LoginResponse LoginResponse
    {
        get { return _loginResponse; }
        set { _loginResponse = value; }
    }

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
}
