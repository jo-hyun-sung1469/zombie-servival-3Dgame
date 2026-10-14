using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Networking;


[RequireComponent(typeof(LegalCheckScript))]
public class RegisterAndEmailVerifyCodeManager : MonoBehaviour
{
    [Header("URL 모음")]//나중에 URL주소를 바꿔야함
    private string _baseUrl = "https://zombie-survival-3d-game.duckdns.org/api/auth";
    private string _nicknameAvailabilityPath = "/register/username-availability";
    private string _sendEmailVerificationCodePath = "/register/email-code";
    private string _verifyEmailCodePath = "/register/email-code/verify";
    private string _registerPath = "/register";
    private LegalCheckScript _legalCheck;

    [Header("Response 필드")]
    [FormerlySerializedAs("emailCodeResponse")]
    [SerializeField] private EmailCodeResponse _emailCodeResponse;
    [FormerlySerializedAs("verifyEmailCodeResponse")]
    [SerializeField] private VerifyEmailCodeResponse _verifyEmailCodeResponse;

    [Header("Text 필드")]
    [FormerlySerializedAs("nicknameText")]
    [SerializeField] private TMP_InputField _nicknameInputField;
    [FormerlySerializedAs("nicknameAvailabilityText")]
    [SerializeField] private TextMeshProUGUI _nicknameAvailabilityText;//닉네임 중복확인 결과를 보여주는 text
    [FormerlySerializedAs("passwordText")]
    [SerializeField] private TMP_InputField _passwordInputField;
    [FormerlySerializedAs("passwordValidationText")]
    [SerializeField] private TextMeshProUGUI _passwordValidationText;
    [FormerlySerializedAs("emailText")]
    [SerializeField] private TMP_InputField _emailInputField;
    [FormerlySerializedAs("codeText")]
    [SerializeField] private TMP_InputField _verificationCodeInputField;
    [FormerlySerializedAs("sendOrNot")]
    [SerializeField] private TextMeshProUGUI _emailCodeStatusText;//코드가 보내졌는지 확인하는 text
    [FormerlySerializedAs("verifyCodeText")]
    [SerializeField] private TextMeshProUGUI _verificationCodeStatusText;

    [Header("GetterSetter 필드")]
    public TMP_InputField NicknameText { get { return _nicknameInputField; } }
    public TextMeshProUGUI NicknameAvailabilityText { get { return _nicknameAvailabilityText; } }
    public TMP_InputField PasswordText { get { return _passwordInputField; } }
    public TextMeshProUGUI PasswordValidationText { get { return _passwordValidationText; } }
    public TMP_InputField EmailText { get { return _emailInputField; } }
    public TMP_InputField CodeText { get { return _verificationCodeInputField; } }
    public TextMeshProUGUI SendOrNot { get { return _emailCodeStatusText; } }
    public TextMeshProUGUI VerifyCodeText { get { return _verificationCodeStatusText; } }
    public EmailCodeResponse EmailCodeResponse { get; set; }
    public VerifyEmailCodeResponse VerifyEmailCodeResponse { get; set; }


    //delegate 필ㄷ
    public delegate IEnumerator CheckNicknameAvailability(string userName, Action<UserNameAvailabilityResponse> response);
    public CheckNicknameAvailability OnCheckNicknameAvailability;
    public delegate IEnumerator SendVerificationCode(string userEmail, Action<EmailCodeResponse> response);
    public SendVerificationCode OnSendVerificationCode;
    public delegate IEnumerator VerifyCode(string code, string emailVerificationId, Action<VerifyEmailCodeResponse> response);
    public VerifyCode OnVerifyCode;
    public delegate IEnumerator RegisterUser(string email, string password, string nickName, string emailVerificationId);
    public RegisterUser OnRegisterUser;

    [FormerlySerializedAs("LoginGuide")]
    [SerializeField] private GameObject _loginGuidePanel;

    private void Awake()
    {
        _legalCheck = GetComponent<LegalCheckScript>();
        OnCheckNicknameAvailability = new CheckNicknameAvailability(CheckNicknameAvailabilityCoroutine);
        OnSendVerificationCode = new SendVerificationCode(SendVerificationCodeCoroutine);
        OnVerifyCode = new VerifyCode(VerifyCodeCoroutine);
        OnRegisterUser = new RegisterUser(RegisterUserCoroutine);
    }

    private IEnumerator CheckNicknameAvailabilityCoroutine(string userName, Action<UserNameAvailabilityResponse> response)
    {
        string url = $"{_baseUrl}{_nicknameAvailabilityPath}?username={userName}";

        using (UnityWebRequest request = new UnityWebRequest(url, "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();  

            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.Success)
            {
                response?.Invoke(JsonConvert.DeserializeObject<UserNameAvailabilityResponse>(request.downloadHandler.text));
            }
            else
            {
                _nicknameAvailabilityText.text = "서버 오류(다시 요청해 주세요) " + request.error;
                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }

    }

    private IEnumerator SendVerificationCodeCoroutine(string userEmail, Action<EmailCodeResponse> response)
    {
        string url = _baseUrl + _sendEmailVerificationCodePath;
        var requestBody = new SendEmailCodeRequest
        {
            Email = userEmail
        };
        
        string json = JsonConvert.SerializeObject(requestBody);//보낼 값을 Json으로 변환

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))//HTTPMethod: Post로 url에 있는 URL경로로 요청을 보낸다
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);//body에 넣을 변수에 저장

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();//서버에서 주는값을 메모리에 저장하는 역할
            request.SetRequestHeader("Content-Type", "application/json");//json타입의 값을 보낸다고 해더에 공지

            yield return request.SendWebRequest();//서버로 요청 전송

            if (request.result == UnityWebRequest.Result.Success)
            {
                _emailCodeStatusText.text = "코드 전송 성공!";
                response?.Invoke(JsonConvert.DeserializeObject<EmailCodeResponse>(request.downloadHandler.text));//받은 Json값을 EmailCodeResponse변환


                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
            else
            {
                _emailCodeStatusText.text = "코드 전송 실패!";
                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator VerifyCodeCoroutine(string code, string emailVerificationId, Action<VerifyEmailCodeResponse> response)
    {
        string url = _baseUrl + _verifyEmailCodePath;
        var requestBody = new VerifyEmailCodeRequest
        {
            EmailVerificationId = emailVerificationId,
            Code = code,
        };

        string json = JsonConvert.SerializeObject(requestBody);//보낼 값을 Json으로 변환

        _verificationCodeStatusText.text = "인증 중...";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))//HTTPMethod: Post로 url에 있는 URL경로로 요청을 보낸다
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);//body에 넣을 변수에 저장

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");//json타입의 값을 보낸다고 해더에 공지

            yield return request.SendWebRequest();//서버로 요청 전송

            if (request.result == UnityWebRequest.Result.Success)
            {
                _verificationCodeStatusText.text = "인증 성공!";
                response?.Invoke(JsonConvert.DeserializeObject<VerifyEmailCodeResponse>(request.downloadHandler.text));//보낼 값을 Json으로 변환


                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
            else
            {
                if (_emailCodeResponse != null &&
                    _emailCodeResponse.ExpiresAtUtc != default(DateTime) &&
                    DateTime.UtcNow >= _emailCodeResponse.ExpiresAtUtc.ToUniversalTime())
                {
                    _verificationCodeStatusText.text = "인증 시간이 초과되었습니다. 인증 코드를 다시 요청해주세요.";
                }
                else
                {
                    _verificationCodeStatusText.text = "인증 실패!";
                }

                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator RegisterUserCoroutine(string email, string password, string nickName, string emailVerificationId)
    {
        if (_legalCheck == null || !_legalCheck.HasAcceptedAllTerms)
        {
            if (_emailCodeStatusText != null)
            {
                _emailCodeStatusText.text = "모든 약관에 동의해주세요.";
            }

            yield break;
        }

        string url = _baseUrl + _registerPath;
        var requestBody = new RegisterRequest
        {
            Email = email,
            Password = password,
            UserName= nickName,
            EmailVerificationId = emailVerificationId
        };
        string json = JsonConvert.SerializeObject(requestBody);//보낼 값을 Json으로 변환
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))//HTTPMethod: Post로 url에 있는 URL경로로 요청을 보낸다
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);//body에 넣을 변수에 저장
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");//json타입의 값을 보낸다고 해더에 공지
            yield return request.SendWebRequest();//서버로 요청 전송
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("회원가입 성공!");
                Debug.Log("응답 내용: " + request.downloadHandler.text);
                _loginGuidePanel.SetActive(true);
            }
            else
            {
                Debug.Log("회원가입 실패!");
                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }
    }
}
