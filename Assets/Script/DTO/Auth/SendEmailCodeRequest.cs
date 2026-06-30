using System;

[Serializable]
public sealed class SendEmailCodeRequest
{
    public string Email = string.Empty;//최대 254글자
}