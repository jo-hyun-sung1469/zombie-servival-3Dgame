using System;
using Newtonsoft.Json;

[Serializable]
public sealed class EmailCodeResponse
{
    [JsonProperty("emailVerificationId")]
    public string EmailVerificationId;

    [JsonProperty("expiresAtUtc")]
    public DateTime ExpiresAtUtc;
}
