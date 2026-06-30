using System;

[Serializable]
public sealed class VerifyEmailCodeRequest
{
    public string emailVerificationId = string.Empty;//4~12자리인데 6 또는 8자리로 바꿀 예정
    public string code = string.Empty;
}