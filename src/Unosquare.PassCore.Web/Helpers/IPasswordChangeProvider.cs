namespace Unosquare.PassCore.Web.Helpers
{
    using Microsoft.Extensions.Options;
    using System.DirectoryServices.AccountManagement;
    using Unosquare.PassCore.Web.Models;

    public interface IPasswordChangeProvider
    {
        ApiErrorItem PerformPasswordChange(ChangePasswordModel model, AppSettings options);
        PrincipalContext AcquirePrincipalContext();
        UserPrincipal AcquireUserPricipal(PrincipalContext context, string username);
    }
}
