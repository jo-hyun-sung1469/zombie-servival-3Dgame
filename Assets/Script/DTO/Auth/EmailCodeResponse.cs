using System;

[Serializable]
public sealed class EmailCodeResponse
{
    public string EmailVerificationId;
    public DateTime ExpiresAtUtc;
}
