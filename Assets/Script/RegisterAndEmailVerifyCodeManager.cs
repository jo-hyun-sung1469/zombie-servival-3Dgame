using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class RegisterAndEmailVerifyCodeManager : MonoBehaviour
{
    [Header("URL 모음")]//나중에 URL주소를 바꿔야함
    private string baseURL = "https://zombie-survival-3d-game.duckdns.org/api/auth";
    private string nicknameCheck = "/register/username-availability";
    private string sendEmailVerificationCodeURL = "/register/email-code";
    private string verifyEmailCodeURL = "/register/email-code/verify";
    private string registerURL = "/register";

    [Header("Response 필드")]
    [SerializeField] private EmailCodeResponse emailCodeResponse;
    [SerializeField] private VerifyEmailCodeResponse verifyEmailCodeResponse;

    [Header("Text 필드")]
    [SerializeField] private TMP_InputField nicknameText;
    [SerializeField] private TextMeshProUGUI nicknameAvailabilityText;//닉네임 중복확인 결과를 보여주는 text
    [SerializeField] private TMP_InputField passwordText;
    [SerializeField] private TextMeshProUGUI passwordValidationText;
    [SerializeField] private TMP_InputField emailText;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private TextMeshProUGUI sendOrNot;//코드가 보내졌는지 확인하는 text
    [SerializeField] private TextMeshProUGUI verifyCodeText;

    [Header("GetterSetter 필드")]
    public TMP_InputField NicknameText { get { return nicknameText; } }
    public TextMeshProUGUI NicknameAvailabilityText { get { return nicknameAvailabilityText; } }
    public TMP_InputField PasswordText { get { return passwordText; } }
    public TextMeshProUGUI PasswordValidationText { get { return passwordValidationText; } }
    public TMP_InputField EmailText { get { return emailText; } }
    public TMP_InputField CodeText { get { return codeText; } }
    public TextMeshProUGUI SendOrNot { get { return sendOrNot; } }
    public TextMeshProUGUI VerifyCodeText { get { return verifyCodeText; } }
    public EmailCodeResponse EmailCodeResponse { get { return emailCodeResponse; } }
    public VerifyEmailCodeResponse VerifyEmailCodeResponse { get { return verifyEmailCodeResponse; } }


    //delegate 필ㄷ
    public delegate IEnumerator CheckNicknameAvailability(string userName, Action<UserNameAvailabilityResponse> response);
    public CheckNicknameAvailability checkNicknameAvailability;
    public delegate IEnumerator SendVerificationCode(string userEmail, Action<EmailCodeResponse> response);
    public SendVerificationCode sendVerificationCode;
    public delegate IEnumerator VerifyCode(string code, string emailVerificationId, Action<VerifyEmailCodeResponse> response);
    public VerifyCode verifyCode;
    public delegate IEnumerator RegisterUser(string email, string password, string nickName, string emailVerificationId);
    public RegisterUser registerUser;

    [SerializeField] private GameObject LoginGuide;

    private void Awake()
    {
        checkNicknameAvailability = new CheckNicknameAvailability(CheckNicknameAvailabilityCoroutine);
        sendVerificationCode = new SendVerificationCode(SendVerificationCodeCoroutine);
        verifyCode = new VerifyCode(VerifyCodeCoroutine);
        registerUser = new RegisterUser(RegisterUserCoroutine);
    }

    private IEnumerator CheckNicknameAvailabilityCoroutine(string userName, Action<UserNameAvailabilityResponse> response)
    {
        string url = $"{baseURL}{nicknameCheck}?username={userName}";

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
                nicknameAvailabilityText.text = "서버 오류(다시 요청해 주세요) " + request.error;
                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }

    }

    private IEnumerator SendVerificationCodeCoroutine(string userEmail, Action<EmailCodeResponse> response)
    {
        string url = baseURL + sendEmailVerificationCodeURL;
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
                sendOrNot.text = "코드 전송 성공!";
                response?.Invoke(JsonConvert.DeserializeObject<EmailCodeResponse>(request.downloadHandler.text));//받은 Json값을 EmailCodeResponse변환


                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
            else
            {
                sendOrNot.text = "코드 전송 실패!";
                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator VerifyCodeCoroutine(string code, string emailVerificationId, Action<VerifyEmailCodeResponse> response)
    {
        string url = baseURL + verifyEmailCodeURL;
        var requestBody = new VerifyEmailCodeRequest
        {
            EmailVerificationId = emailVerificationId,
            Code = code,
        };

        string json = JsonConvert.SerializeObject(requestBody);//보낼 값을 Json으로 변환

        verifyCodeText.text = "인증 중...";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))//HTTPMethod: Post로 url에 있는 URL경로로 요청을 보낸다
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);//body에 넣을 변수에 저장

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");//json타입의 값을 보낸다고 해더에 공지

            yield return request.SendWebRequest();//서버로 요청 전송

            if (request.result == UnityWebRequest.Result.Success)
            {
                verifyCodeText.text = "인증 성공!";
                response?.Invoke(JsonConvert.DeserializeObject<VerifyEmailCodeResponse>(request.downloadHandler.text));//보낼 값을 Json으로 변환


                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
            else
            {
                if (emailCodeResponse != null &&
                    emailCodeResponse.ExpiresAtUtc != default(DateTime) &&
                    DateTime.UtcNow >= emailCodeResponse.ExpiresAtUtc.ToUniversalTime())
                {
                    verifyCodeText.text = "인증 시간이 초과되었습니다. 인증 코드를 다시 요청해주세요.";
                }
                else
                {
                    verifyCodeText.text = "인증 실패!";
                }

                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }
    }

    private IEnumerator RegisterUserCoroutine(string email, string password, string nickName, string emailVerificationId)
    {
        string url = baseURL + registerURL;
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
                LoginGuide.SetActive(true);
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
