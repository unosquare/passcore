namespace Unosquare.PassCore.Web.Helpers
{
    using Unosquare.PassCore.Web.Models;

    public interface IPasswordChangeProvider
    {
        ApiErrorItem PerformPasswordChange(ChangePasswordModel model);       
    }
}
