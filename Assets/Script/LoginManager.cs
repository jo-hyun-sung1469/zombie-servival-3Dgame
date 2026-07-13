using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    private string loginURL= "https://localhost:7037/api/auth/login";
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private GameManager gameManager;

    public void onLogin()
    {
        string nickname = nicknameInputField.text.Trim();
        string password = passwordInputField.text.Trim();
        if (string.IsNullOrWhiteSpace(nicknameInputField.text) || string.IsNullOrWhiteSpace(password))
        {
            errorText.text = "필수 항목을 전부 입력해주세요.";
        }
        else
        {
            StartCoroutine(Login(nickname, password, response =>
            {
               gameManager.LoginResponse = response;
            }));
        }
    }

    private IEnumerator Login(string nickname, string password, Action<LoginResponse> response)
    {
        string url = loginURL;
        var requestBody = new LoginRequest
        {
            UserName = nickname,
            Password = password
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
                response?.Invoke(JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text));//받은 Json값을 EmailCodeResponse변환

                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
            else
            {
                errorText.text = "로그인 실패!";
                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
                Debug.Log("응답 내용: " + request.downloadHandler.text);
            }
        }
    }
}
