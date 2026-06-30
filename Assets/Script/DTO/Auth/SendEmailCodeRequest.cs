using System;
using Newtonsoft.Json;

[Serializable]
public sealed class SendEmailCodeRequest
{
    [JsonProperty("email")]
    public string Email = string.Empty;//최대 254글자
}
