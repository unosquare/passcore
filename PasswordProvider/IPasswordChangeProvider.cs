namespace PasswordProvider
{
    public interface IPasswordChangeProvider
    {
        ApiErrorItem PerformPasswordChange(ChangePasswordModel model);
    }
}