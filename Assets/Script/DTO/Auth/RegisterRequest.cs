using System;

[Serializable]
public sealed class RegisterRequest
{
    public string userName = string.Empty;//최대 30글자, 최소 3글자
    public string email = string.Empty;//최대 254글자;
    public string EmailVerificationId = string.Empty;
    public string Password = string.Empty;//최대 100글자, 최소 6글자

}