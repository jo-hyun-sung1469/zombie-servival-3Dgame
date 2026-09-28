using System.Text.RegularExpressions;

public static class AuthInputRules
{
    public const int UserNameMinLength = 3;
    public const int UserNameMaxLength = 30;
    public const string UserNamePattern = @"\A[a-z0-9가-힣ㄱ-ㅎㅏ-ㅣ]+\z";
    public const string UserNameError = "닉네임은 영문 소문자, 숫자, 한글만 사용할 수 있습니다.";

    public const int PasswordMinLength = 6;
    public const int PasswordMaxLength = 100;
    public const string PasswordPattern = @"\A[A-Za-z0-9!@#$%^*?/]+\z";
    public const string PasswordError = "비밀번호는 영문, 숫자, !@#$%^*?/만 사용할 수 있습니다.";

    public static bool TryValidateUserName(string userName, out string error)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            error = "닉네임을 입력해주세요.";
            return false;
        }

        if (userName.Length < UserNameMinLength || userName.Length > UserNameMaxLength)
        {
            error = $"닉네임은 {UserNameMinLength}자 이상 {UserNameMaxLength}자 이하로 입력해주세요.";
            return false;
        }

        if (!Regex.IsMatch(userName, UserNamePattern))
        {
            error = UserNameError;
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool TryValidatePassword(string password, out string error)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            error = "비밀번호를 입력해주세요.";
            return false;
        }

        if (password.Length < PasswordMinLength || password.Length > PasswordMaxLength)
        {
            error = $"비밀번호는 {PasswordMinLength}자 이상 {PasswordMaxLength}자 이하로 입력해주세요.";
            return false;
        }

        if (!Regex.IsMatch(password, PasswordPattern))
        {
            error = PasswordError;
            return false;
        }

        error = string.Empty;
        return true;
    }
}
