using System;

[Serializable]
public sealed class RegisterResponse
{
    public string userId;
    public string userName;
    public string email;
    public string role;
    public DateTime createdAtUtc;
}