namespace StudentAPI.DTOs
{
    public class ReportsDto
    {
        public int TotalStudents { get; set; }

        public int TotalUsers { get; set; }

        public int TotalAdmins { get; set; }

        public int TotalNormalUsers { get; set; }

        public int MaleStudents { get; set; }

        public int FemaleStudents { get; set; }

        public int StudentsWithPhoto { get; set; }

        public int StudentsWithoutPhoto { get; set; }

        public List<CityReportDto> CityWise { get; set; } = [];

        public List<StateReportDto> StateWise { get; set; } = [];

        public List<MonthlyReportDto> MonthlyRegistrations { get; set; } = [];
    }
}