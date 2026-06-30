using System;
using Newtonsoft.Json;

[Serializable]
public sealed class LoginResponse
{
    [JsonProperty("accessToken")]
    public string AccessToken;  

    [JsonProperty("tokenType")]
    public string TokenType;

    [JsonProperty("expiresAtUtc")]
    public DateTime ExpiresAtUtc;

    [JsonProperty("userName")]
    public string UserName;

    [JsonProperty("role")]
    public string Role;
}
