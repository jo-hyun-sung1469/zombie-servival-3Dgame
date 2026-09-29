using System;
using Newtonsoft.Json;

[Serializable]
public sealed class LoginRequest
{
    [JsonProperty("userName")]
    public string UserName = string.Empty;

    [JsonProperty("password")]
    public string Password = string.Empty;
}
