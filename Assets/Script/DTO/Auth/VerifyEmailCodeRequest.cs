using System;
using Newtonsoft.Json;

[Serializable]
public sealed class VerifyEmailCodeRequest
{
    [JsonProperty("emailVerificationId")]
    public string EmailVerificationId = string.Empty;//4~12자리인데 6 또는 8자리로 바꿀 예정

    [JsonProperty("code")]
    public string Code = string.Empty;
}
