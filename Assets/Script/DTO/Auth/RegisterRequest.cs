using System;
using Newtonsoft.Json;

[Serializable]
public sealed class RegisterRequest
{
    [JsonProperty("userName")]
    public string UserName = string.Empty;//최대 30글자, 최소 3글자

    [JsonProperty("email")]
    public string Email = string.Empty;//최대 254글자;

    [JsonProperty("emailVerificationId")]
    public string EmailVerificationId = string.Empty;

    [JsonProperty("password")]
    public string Password = string.Empty;//최대 100글자, 최소 6글자

}
