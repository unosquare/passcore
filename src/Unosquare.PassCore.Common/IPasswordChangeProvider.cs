namespace Unosquare.PassCore.Common
{
    /// <summary>
    /// Represents a interface for a password change provider
    /// </summary>
    public interface IPasswordChangeProvider
    {
        /// <summary>
        /// Performs the password change.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns></returns>
        ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword);
    }
}