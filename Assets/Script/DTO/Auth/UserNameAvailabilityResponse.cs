using Newtonsoft.Json;
using UnityEngine;

public class UserNameAvailabilityResponse : MonoBehaviour
{
    [JsonProperty("username")]
    public string UserName { get; set; }

    [JsonProperty("isavailable")]
    public bool IsAvailable { get; set; }
}
