namespace Unosquare.PassCore.Common
{
    /// <summary>
    /// Represents a interface for a password change provider
    /// </summary>
    public interface IPasswordChangeProvider
    {
        /// <summary>
        /// Performs the password change using the credentials provided.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The API error item or null if the change password operation was successful.</returns>
        ApiErrorItem PerformPasswordChange(string username, string currentPassword, string newPassword);

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        IAppSettings Settings { get; }
    }
}