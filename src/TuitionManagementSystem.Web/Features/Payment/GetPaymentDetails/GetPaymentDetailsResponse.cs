namespace TuitionManagementSystem.Web.Features.Payment.GetPaymentDetails;

public class GetPaymentDetailsResponse
{
    public required decimal Amount { get; init; }

    public required DateTime PaidAt { get; init; }

    public required PaidMethod Method { get; init; }

    public required ICollection<PaidInvoice> Invoices { get; init; }
}

public abstract class PaidMethod;

public class CardMethod : PaidMethod
{
    public required string Brand { get; init; }

    public required string Last4 { get; init; }
}

public class BankMethod : PaidMethod
{
    public required string Bank { get; init; }
}

public class GenericMethod : PaidMethod
{
    public required string Generic { get; init; }
}

public class PaidInvoice
{
    public required string Name { get; init; }

    public required decimal Amount { get; init; }

    public required InvoiceStudentDetails Student { get; init; }

    public required InvoiceEnrollmentDetails Enrollment { get; init; }
}

public class InvoiceStudentDetails
{
    public required int Id { get; init; }

    public required string Name { get; init; }
}

public class InvoiceEnrollmentDetails
{
    public required string CourseName { get; init; }
}
