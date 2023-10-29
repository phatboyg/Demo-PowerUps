using MassTransit;
using Retry.Components.Exceptions;
using Retry.Contracts;

namespace Retry.Components.Consumers;

public class AccountInformationConsumer :
    IConsumer<RequestAccountBalance>,
    IConsumer<RequestAccountStatus>
{
    public Task Consume(ConsumeContext<RequestAccountBalance> context)
    {
        if (context.Message.AccountNumber == "404")
            throw new BusinessRuleException($"Account not found: {context.Message.AccountNumber}");

        if (context.Message.AccountNumber == "500" && context.GetRetryCount() < 2)
            throw new TransientException("Something wicked this way came.");

        return context.RespondAsync(new AccountBalance
        {
            AccountNumber = context.Message.AccountNumber,
            Balance = 1_000_000.00m
        }, x =>
        {
            if (context.GetRetryCount() > 0)
                x.Headers.Set("Retried", context.GetRetryCount());
        });
    }

    public Task Consume(ConsumeContext<RequestAccountStatus> context)
    {
        if (context.Message.AccountNumber == "404")
        {
            if (context.IsResponseAccepted<AccountNotAvailable>())
                return context.RespondAsync(new AccountNotAvailable
                {
                    AccountNumber = context.Message.AccountNumber,
                    Reason = "Account not found"
                });

            throw new BusinessRuleException($"Account not found: {context.Message.AccountNumber}");
        }

        return context.RespondAsync(new AccountStatus
        {
            AccountNumber = context.Message.AccountNumber,
            Status = "Good"
        });
    }
}

public class AccountInformationConsumerDefinition :
    ConsumerDefinition<AccountInformationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<AccountInformationConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 100).Handle<TransientException>());

        endpointConfigurator.UseMessageRetry(r => r.None().Handle<BusinessRuleException>());
    }
}