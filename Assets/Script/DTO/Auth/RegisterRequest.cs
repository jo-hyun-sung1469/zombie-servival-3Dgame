using System;
using Newtonsoft.Json;

[Serializable]
public sealed class RegisterRequest
{
    [JsonProperty("userName")]
    public string UserName { get; set; } = string.Empty;//최대 30글자, 최소 3글자

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;//최대 254글자;

    [JsonProperty("emailVerificationId")]
    public string EmailVerificationId { get; set; } = string.Empty;

    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;//최대 100글자, 최소 6글자

}
