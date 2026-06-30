using System;
using Newtonsoft.Json;

public sealed class VerifyEmailCodeResponse
{
    [JsonProperty("emailVerificationId")]
    public string EmailVerificationId;

    [JsonProperty("verifiedAtUtc")]
    public DateTime VerifiedAtUtc;
}
