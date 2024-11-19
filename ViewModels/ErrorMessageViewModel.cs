namespace JwtCookiesScheme.ViewModels
{
    public class ErrorMessageViewModel
    {
        public string Message { get; set; }
        public string AlertType { get; set; } // e.g., "success", "danger", "info", "warning"
        public bool Dismissible { get; set; } = true; // Option to close the alert
    }
}
