using System.ComponentModel.DataAnnotations;

namespace Unosquare.PassCore.Web.Models;

public class ChangePasswordModel
{
    private string? _username;
    private string? _currentPassword;
    private string? _newPassword;
    private string? _newPasswordVerify;
    private string? _recaptcha;

    [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
    public string Username
    {
        get => _username ?? string.Empty;
        set => _username = value;
    }

    [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
    public string CurrentPassword
    {
        get => _currentPassword ?? string.Empty;
        set => _currentPassword = value;
    }

    [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
    public string NewPassword
    {
        get => _newPassword ?? string.Empty;
        set => _newPassword = value;
    }

    [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
    [Compare(nameof(NewPassword), ErrorMessage = nameof(ApiErrorCode.FieldMismatch))]
    public string NewPasswordVerify
    {
        get => _newPasswordVerify ?? string.Empty;
        set => _newPasswordVerify = value;
    }

    public string Recaptcha
    {
        get => _recaptcha ?? string.Empty;
        set => _recaptcha = value;
    }
}