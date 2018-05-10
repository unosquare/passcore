namespace Unosquare.PassCore.Web.Helpers
{
    using Models;

    public interface IPasswordChangeProvider
    {
        ApiErrorItem PerformPasswordChange(ChangePasswordModel model);
    }
}