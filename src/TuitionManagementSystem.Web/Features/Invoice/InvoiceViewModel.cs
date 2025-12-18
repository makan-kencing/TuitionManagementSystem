namespace TuitionManagementSystem.Web.Models.ViewModels
{
    using TuitionManagementSystem.Web.Models.Payment;
    using System;

    public class InvoiceViewModel
    {
        public int InvoiceId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int EnrollmentId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime InvoicedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public InvoiceStatus Status { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsSelected { get; set; }
    }
}
