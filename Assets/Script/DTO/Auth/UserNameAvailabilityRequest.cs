using Newtonsoft.Json;
using UnityEngine;

public sealed class UserNameAvailabilityRequest
{
    [JsonProperty("username")]
    public string UserName = string.Empty;
}
