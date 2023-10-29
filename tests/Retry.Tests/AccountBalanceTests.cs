using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Retry.Components.Consumers;
using Retry.Contracts;
using Xunit.Abstractions;

namespace Retry.Tests;

public class Requesting_an_account_balance
{
    readonly TestOutputTextWriter _output;

    public Requesting_an_account_balance(ITestOutputHelper output)
    {
        _output = new TestOutputTextWriter(output);
    }

    [Fact]
    public async Task Should_return_if_found_successfully()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(_output, x => x.AddConsumer<AccountInformationConsumer, AccountInformationConsumerDefinition>())
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<RequestAccountBalance>();

        var response = await client.GetResponse<AccountBalance>(new RequestAccountBalance
        {
            AccountNumber = "123456"
        });

        Assert.Equal(1_000_000m, response.Message.Balance);
    }

    [Fact]
    public async Task Should_eventually_succeed()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(_output, x => x.AddConsumer<AccountInformationConsumer, AccountInformationConsumerDefinition>())
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<RequestAccountBalance>();

        var response = await client.GetResponse<AccountBalance>(new RequestAccountBalance
        {
            AccountNumber = "500"
        });

        Assert.Equal(1_000_000m, response.Message.Balance);

        Assert.Equal(2, response.Headers.Get<int>("Retried"));
    }

    [Fact]
    public async Task Should_not_retry_business_rule_exceptions()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(_output, x => x.AddConsumer<AccountInformationConsumer, AccountInformationConsumerDefinition>())
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<RequestAccountBalance>();

        await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<AccountBalance>(new RequestAccountBalance
        {
            AccountNumber = "404"
        }));
    }
}