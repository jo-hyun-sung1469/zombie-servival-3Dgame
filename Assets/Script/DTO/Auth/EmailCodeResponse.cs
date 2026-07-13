using System;
using Newtonsoft.Json;

[Serializable]
public sealed class EmailCodeResponse
{
    [JsonProperty("emailVerificationId")]
    public string EmailVerificationId { get; set; } = string.Empty;

    [JsonProperty("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }
}
