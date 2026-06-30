using System;

[Serializable]
public sealed class LoginResponse
{
    public string AccessToken;  
    public string TokenType;
    public DateTime ExpiresAtUtc;
    public string UserName;
    public string Role;
}
