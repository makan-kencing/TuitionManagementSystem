namespace TuitionManagementSystem.Web.Services.Payment;

using System.Configuration;
using Options;
using Stripe;

public class StripePaymentService : IPaymentService
{
    private readonly StripeOptions options;
    private readonly StripeClient client;

    public StripePaymentService(IConfiguration configuration)
    {
        this.options = configuration.GetSection("Stripe")
                           .Get<StripeOptions>()
                       ?? throw new ConfigurationErrorsException("Stripe configurations not found.");
        this.client = new StripeClient(this.options.SecretKey);
    }

    public async Task CreateSubscriptionAsync()
    {

    }

    public async Task UpdateSubscriptionAsync()
    {

    }

    public async Task CancelSubscriptionAsync()
    {

    }
}
