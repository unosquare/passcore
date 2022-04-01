using System;

namespace Unosquare.PassCore.Common;

/// <summary>
/// Represents a interface for a password change provider.
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
    ApiErrorItem? PerformPasswordChange(string username, string currentPassword, string newPassword);

    /// <summary>
    /// Compute the distance between two strings.
    /// Take it from https://www.csharpstar.com/csharp-string-distance-algorithm/.
    /// </summary>
    /// <param name="currentPassword">The current password.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>
    /// The distance between strings.
    /// </returns>
    int MeasureNewPasswordDistance(string currentPassword, string newPassword)
    {
        var n = currentPassword.Length;
        var m = newPassword.Length;
        var d = new int[n + 1, m + 1];

        // Step 1
        if (n == 0)
            return m;

        if (m == 0)
            return n;

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++) { }

        for (int j = 0; j <= m; d[0, j] = j++) { }

        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
                int cost = (newPassword[j - 1] == currentPassword[i - 1]) ? 0 : 1;
                // Step 6
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        // Step 7
        return d[n, m];

    }
}