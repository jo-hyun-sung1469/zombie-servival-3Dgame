using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private string _loginUrl= "https://zombie-survival-3d-game.duckdns.org/api/auth/login";
    [FormerlySerializedAs("nicknameInputField")]
    [SerializeField] private TMP_InputField _nicknameInputField;
    [FormerlySerializedAs("passwordInputField")]
    [SerializeField] private TMP_InputField _passwordInputField;
    [FormerlySerializedAs("errorText")]
    [SerializeField] private TextMeshProUGUI _errorText;

    public TMP_InputField NicknameInputField { get { return _nicknameInputField; } }
    public TMP_InputField PasswordInputField { get { return _passwordInputField; } }
    public TextMeshProUGUI ErrorText { get; private set; }

    public delegate IEnumerator LoginDelegate(string username, string password, Action<LoginResponse> response);
    public LoginDelegate OnLoginDelegate;


    private IEnumerator Login(string nickname, string password, Action<LoginResponse> response)
    {
        string url = _loginUrl;
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

                Debug.Log("로그인 성공!");
                SceneManager.LoadScene("MainScene");
            }
            else
            {   if(request.responseCode == 401)
                    _errorText.text = "닉네임 및 비밀번호가 틀렸습니다";
                else
                    _errorText.text = "서버 오류가 발생했습니다. 잠시 후 다시 시도해주세요.";

                Debug.Log("서버 오류: " + request.responseCode + " / " + request.error);
            }
        }
    }
}
