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
    private string baseURL = "https://localhost:7037/api/auth";
    private string sendEmailVerificationCodeURL = "/register/email-code";
    private string verifyEmailCodeURL = "/register/email-code/verify";
    private string registerURL = "/register";

    [Header("private 필드")]
    private string email = string.Empty;
    private string password = string.Empty;
    private string nickname = string.Empty;
    private string insertCode = string.Empty;

    [Header("Text 필드")]
    [SerializeField] private TMP_InputField emailText;
    [SerializeField] private TMP_InputField passwordText;
    [SerializeField] private TMP_InputField nicknameText;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private TextMeshProUGUI sendOrNot;//코드가 보내졌는지 확인하는 text

    [Header("Response 필드")]
    [SerializeField] private EmailCodeResponse emailCodeResponse;
    [SerializeField] private VerifyEmailCodeResponse verifyEmailCodeResponse;
    public void OnRequestEmailVerificationCode()
    {
        email = emailText.text;
        email.Trim();
        if(!string.IsNullOrWhiteSpace(email))
        {
            StartCoroutine(SendVerificationCode(email, (response) =>
            {
                emailCodeResponse = response;
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
            codeText.text = "인증 코드가 비어있습니다";
            Debug.Log("인증코드가 비어있습니다");
        }
    }

    public void OnRegister()
    {
        email = emailText.text;
        password = passwordText.text;
        nickname = nicknameText.text;
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(nickname))
        {
            StartCoroutine(RegisterUser(email, password, nickname, verifyEmailCodeResponse.EmailVerificationId));
        }
        else
        {
            Debug.Log("모든 필드를 입력해주세요.");
        }
    }

    private IEnumerator SendVerificationCode(string userEmail, Action<EmailCodeResponse> response)
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

    private IEnumerator VerifyCode(string code, string emailVerificationId, Action<VerifyEmailCodeResponse> response)
    {
        string url = baseURL + verifyEmailCodeURL;
        var requestBody = new VerifyEmailCodeRequest
        {
            EmailVerificationId = emailVerificationId,
            Code = code,
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
                sendOrNot.text = "코드 전송 성공!";
                response?.Invoke(JsonConvert.DeserializeObject<VerifyEmailCodeResponse>(request.downloadHandler.text));//보낼 값을 Json으로 변환


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

    private IEnumerator RegisterUser(string email, string password, string nickName, string emailVerificationId)
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
