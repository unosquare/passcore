namespace Unosquare.PassCore.Web.Models
{
    using Common;
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordModel
    {
        private string? username;
        private string? currentPassword;
        private string? newPassword;
        private string? newPasswordVerify;
        private string? recaptcha;

        [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
        public string Username
        {
            get => username ?? string.Empty;
            set => username = value;
        }

        [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
        public string CurrentPassword
        {
            get => currentPassword ?? string.Empty;
            set => currentPassword = value;
        }

        [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
        public string NewPassword
        {
            get => newPassword ?? string.Empty;
            set => newPassword = value;
        }

        [Required(ErrorMessage = nameof(ApiErrorCode.FieldRequired))]
        [Compare(nameof(NewPassword), ErrorMessage = nameof(ApiErrorCode.FieldMismatch))]
        public string NewPasswordVerify
        {
            get => newPasswordVerify ?? string.Empty;
            set => newPasswordVerify = value;
        }

        public string Recaptcha
        {
            get => recaptcha ?? string.Empty;
            set => recaptcha = value;
        }
    }
}
