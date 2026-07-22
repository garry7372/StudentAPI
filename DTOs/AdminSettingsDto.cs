namespace StudentAPI.DTOs
{
    public class AdminSettingsDto
    {
        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Role { get; set; } = "";

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }

        public string SiteName { get; set; } = "Student Admission System";

        public string ContactEmail { get; set; } = "";

        public int RecordsPerPage { get; set; } = 10;

        public string Language { get; set; } = "English";

        public string TimeZone { get; set; } = "Asia/Kolkata";

        public bool EnableDarkMode { get; set; }

        public bool EnableNotifications { get; set; }

        public bool CollapseSidebar { get; set; }
    }
}