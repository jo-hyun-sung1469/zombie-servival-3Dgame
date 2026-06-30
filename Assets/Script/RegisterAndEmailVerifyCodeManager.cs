using Newtonsoft.Json;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterAndEmailVerifyCodeManager : MonoBehaviour
{
    [Header("URL 모음")]
    private string baseURL = "https://localhost:7037/api/auth";
    private string sendEmailVerificationCodeURL = "/register/email-code";
    private string verifyEmailCodeURL = "/register/email-code/verify";
    private string registerURL = "/register";

    [Header("private 필드")]
    private string email = string.Empty;
    private string password = string.Empty;
    private string nickName = string.Empty;
    private string insertCode = string.Empty;

    [Header("Text 필드")]
    [SerializeField] private TMP_InputField emailText;
    [SerializeField] private TMP_InputField passwordText;
    [SerializeField] private TMP_InputField nicNameText;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private TextMeshProUGUI sendOrNot;//코드가 보내졌는지 확인하는 text
    public void RequestEmailVerificationCode()
    {
        if(!string.IsNullOrWhiteSpace(email))
        {
            StartCoroutine(SendVerificationCode(email));
        }
        else
        {
            Debug.Log("이메일란이 비어있습니다");
            emailText.text = "이메일을 입력해주세요";
        }
    }

    private IEnumerator SendVerificationCode(string userEmail)
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
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");//json타입의 값을 보낸다고 해더에 공지

            yield return request.SendWebRequest();//서버로 요청 전송

            if (request.result == UnityWebRequest.Result.Success)
            {
                sendOrNot.text = "코드 전송 성공!";
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
}
