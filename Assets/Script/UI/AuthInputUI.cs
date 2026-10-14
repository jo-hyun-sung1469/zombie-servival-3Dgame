using TMPro;
using UnityEngine;

[RequireComponent(typeof(LegalCheckScript))]
[RequireComponent(typeof(RegisterAndEmailVerifyCodeManager))]
[RequireComponent(typeof(LoginManager))]
public class InputUI : MonoBehaviour
{
    [Header("private 필드")]
    private string email = string.Empty;
    private string password = string.Empty;
    private string nickname = string.Empty;
    private string insertCode = string.Empty;
    private LegalCheckScript legalCheck;

    private RegisterAndEmailVerifyCodeManager registerManager;
    private LoginManager loginManager;

    private void Awake()
    {
        registerManager = GetComponent<RegisterAndEmailVerifyCodeManager>();
        loginManager = GetComponent<LoginManager>();
        legalCheck = GetComponent<LegalCheckScript>();
    }


    private void OnPasswordStart()
    {
        Input.imeCompositionMode = IMECompositionMode.Off;
    }

    private void OnPasswordEnd()
    {
        Input.imeCompositionMode = IMECompositionMode.On;
    }

    public void OnCheckNicknameAvailability()
    {
        nickname = registerManager.NicknameText.text;
        if (!AuthInputRules.TryValidateUserName(nickname, out string error))
        {
            ShowFeedback(registerManager.NicknameAvailabilityText, error);
            return;
        }

        if (registerManager.NicknameAvailabilityText != null)
        {
            registerManager.NicknameAvailabilityText.text = string.Empty;
        }

        StartCoroutine(registerManager.checkNicknameAvailability(nickname, (response) =>
        {
            ShowFeedback(registerManager.NicknameAvailabilityText, response.IsAvailable
                ? "사용 가능한 닉네임입니다."
                : "이미 사용 중인 닉네임입니다.");
        }));
    }

    public void OnRequestEmailVerificationCode()
    {
        if (legalCheck == null || !legalCheck.HasAcceptedAllTerms)
        {
            ShowFeedback(registerManager.SendOrNot, "모든 약관에 동의해주세요.");
            return;
        }

        email = registerManager.EmailText.text;
        email.Trim();
        if (!string.IsNullOrWhiteSpace(email))
        {
            StartCoroutine(registerManager.sendVerificationCode(email, (response) =>
            {
                registerManager.EmailCodeResponse = response;
            }));
        }
        else
        {
            Debug.Log("이메일란이 비어있습니다");
            emailText.text = "이메일을 입력해주세요";
        }
    }

    public void OnCheckVerificationCode()
    {
        insertCode = codeText.text;
        if (!string.IsNullOrEmpty(insertCode))
        {
            StartCoroutine(VerifyCode(insertCode, emailCodeResponse.EmailVerificationId, (response) =>
            {
                verifyEmailCodeResponse = response;
            }));
        }
        else
        {
            verifyCodeText.text = "인증 코드가 비어있습니다";
            Debug.Log("인증코드가 비어있습니다");
        }
    }

    public void OnRegister()
    {
        email = emailText.text;
        password = passwordText.text;
        nickname = nicknameText.text;
        if (!AuthInputRules.TryValidateUserName(nickname, out string nicknameError))
        {
            ShowFeedback(nicknameAvailabilityText, nicknameError);
            return;
        }

        if (!AuthInputRules.TryValidatePassword(password, out string passwordError))
        {
            ShowFeedback(passwordValidationText, passwordError);
            return;
        }

        if (passwordValidationText != null)
        {
            passwordValidationText.text = string.Empty;
        }

        if (legalCheck == null || !legalCheck.HasAcceptedAllTerms)
        {
            ShowFeedback(sendOrNot, "모든 약관에 동의해주세요.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            if (verifyEmailCodeResponse == null ||
                string.IsNullOrWhiteSpace(verifyEmailCodeResponse.EmailVerificationId))
            {
                ShowFeedback(sendOrNot, "이메일 인증을 먼저 완료해주세요.");
                return;
            }

            StartCoroutine(RegisterUser(email, password, nickname, verifyEmailCodeResponse.EmailVerificationId));
        }
        else
        {
            Debug.Log("모든 필드를 입력해주세요.");
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
