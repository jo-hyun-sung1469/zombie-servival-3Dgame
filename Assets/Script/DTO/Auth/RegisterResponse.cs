using System;
using Newtonsoft.Json;

[Serializable]
public sealed class RegisterResponse
{
    [JsonProperty("userId")]
    public string UserId;

    [JsonProperty("userName")]
    public string UserName;

    [JsonProperty("email")]
    public string Email;

    [JsonProperty("role")]
    public string Role;

    [JsonProperty("createdAtUtc")]
    public DateTime CreatedAtUtc;
}
