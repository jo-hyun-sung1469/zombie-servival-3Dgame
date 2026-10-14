using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(LegalCheckScript))]
[RequireComponent(typeof(RegisterAndEmailVerifyCodeManager))]
[RequireComponent(typeof(LoginManager))]
[DefaultExecutionOrder(100)]
public class AuthInputUIHandler : MonoBehaviour
{
    [Header("private 필드")]
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _nickname = string.Empty;
    private string _verificationCode = string.Empty;
    private LegalCheckScript _legalCheck;

    private RegisterAndEmailVerifyCodeManager _registerManager;
    private LoginManager _loginManager;
    [SerializeField] private GameManager _gameManager;
    private bool _isPasswordInputFocused;

    private void Awake()
    {
        _registerManager = GetComponent<RegisterAndEmailVerifyCodeManager>();
        _loginManager = GetComponent<LoginManager>();
        _legalCheck = GetComponent<LegalCheckScript>();
    }


    private void LateUpdate()
    {
        TMP_InputField registerPasswordInputField = _registerManager.PasswordText;
        TMP_InputField loginPasswordInputField = _loginManager.PasswordInputField;
        bool isPasswordInputFocused =
            (registerPasswordInputField != null && registerPasswordInputField.isFocused) ||
            (loginPasswordInputField != null && loginPasswordInputField.isFocused);

        if (isPasswordInputFocused)
        {
            if (!_isPasswordInputFocused)
            {
                OnPasswordStart();
            }
            else if (Input.imeCompositionMode != IMECompositionMode.Off)
            {
                Input.imeCompositionMode = IMECompositionMode.Off;
            }
        }
        else if (_isPasswordInputFocused)
        {
            OnPasswordEnd();
        }
    }

    private void OnDisable()
    {
        if (_isPasswordInputFocused)
        {
            OnPasswordEnd();
        }
    }

    public void OnPasswordStart()
    {
        _isPasswordInputFocused = true;
        Input.imeCompositionMode = IMECompositionMode.Off;
    }

    public void OnPasswordEnd()
    {
        _isPasswordInputFocused = false;
        Input.imeCompositionMode = IMECompositionMode.On;
    }

    public void OnCheckNicknameAvailability()
    {
        _nickname = _registerManager.NicknameText.text;
        if (!AuthInputRules.TryValidateUserName(_nickname, out string error))
        {
            ShowFeedback(_registerManager.NicknameAvailabilityText, error);
            return;
        }

        if (_registerManager.NicknameAvailabilityText != null)
        {
            _registerManager.NicknameAvailabilityText.text = string.Empty;
        }

        StartCoroutine(_registerManager.OnCheckNicknameAvailability(_nickname, (response) =>
        {
            ShowFeedback(_registerManager.NicknameAvailabilityText, response.IsAvailable
                ? "사용 가능한 닉네임입니다."
                : "이미 사용 중인 닉네임입니다.");
        }));
    }

    public void OnRequestEmailVerificationCode()
    {
        if (_legalCheck == null || !_legalCheck.HasAcceptedAllTerms)
        {
            ShowFeedback(_registerManager.SendOrNot, "모든 약관에 동의해주세요.");
            return;
        }

        _email = _registerManager.EmailText.text;
        _email.Trim();
        if (!string.IsNullOrWhiteSpace(_email))
        {
            StartCoroutine(_registerManager.OnSendVerificationCode(_email, (response) =>
            {
                _registerManager.EmailCodeResponse = response;
            }));
        }
        else
        {
            Debug.Log("이메일란이 비어있습니다");
            _registerManager.EmailText.text = "이메일을 입력해주세요";
        }
    }

    public void OnCheckVerificationCode()
    {
        _verificationCode = _registerManager.CodeText.text;
        if (!string.IsNullOrEmpty(_verificationCode))
        {
            StartCoroutine(_registerManager.OnVerifyCode(_verificationCode, _registerManager.EmailCodeResponse.EmailVerificationId, (response) =>
            {
                _registerManager.VerifyEmailCodeResponse = response;
            }));
        }
        else
        {
            _registerManager.VerifyCodeText.text = "인증 코드가 비어있습니다";
            Debug.Log("인증코드가 비어있습니다");
        }
    }

    public void OnRegister()
    {
        _email = _registerManager.EmailText.text;
        _password = _registerManager.PasswordText.text;
        _nickname = _registerManager.NicknameText.text;
        if (!AuthInputRules.TryValidateUserName(_nickname, out string nicknameError))
        {
            ShowFeedback(_registerManager.NicknameAvailabilityText, nicknameError);
            return;
        }

        if (!AuthInputRules.TryValidatePassword(_password, out string passwordError))
        {
            ShowFeedback(_registerManager.PasswordValidationText, passwordError);
            return;
        }

        if (_registerManager.PasswordValidationText != null)
        {
            _registerManager.PasswordValidationText.text = string.Empty;
        }

        if (_legalCheck == null || !_legalCheck.HasAcceptedAllTerms)
        {
            ShowFeedback(_registerManager.SendOrNot, "모든 약관에 동의해주세요.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(_email))
        {
            if (_registerManager.VerifyEmailCodeResponse == null ||
                string.IsNullOrWhiteSpace(_registerManager.VerifyEmailCodeResponse.EmailVerificationId))
            {
                ShowFeedback(_registerManager.SendOrNot, "이메일 인증을 먼저 완료해주세요.");
                return;
            }

            StartCoroutine(_registerManager.OnRegisterUser(_email, _password, _nickname, _registerManager.VerifyEmailCodeResponse.EmailVerificationId));
        }
        else
        {
            Debug.Log("모든 필드를 입력해주세요.");
        }
    }

    public void OnLogin()
    {
        string nickname = _loginManager.NicknameInputField.text.Trim();
        string password = _loginManager.PasswordInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(_loginManager.NicknameInputField.text) || string.IsNullOrWhiteSpace(password))
        {
            _loginManager.ErrorText.text = "필수 항목을 전부 입력해주세요.";
        }
        else
        {
            StartCoroutine(_loginManager.OnLoginDelegate(nickname, password, response =>
            {
                _gameManager.LoginResponse = response;
            }));
        }
    }

    private static void ShowFeedback(TMP_Text feedbackText, string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }

        Debug.Log(message);
    }

}
