using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Retry.Components.Consumers;
using Retry.Contracts;
using Xunit.Abstractions;

namespace Retry.Tests;

public class Requesting_an_account_status
{
    readonly TestOutputTextWriter _output;

    public Requesting_an_account_status(ITestOutputHelper output)
    {
        _output = new TestOutputTextWriter(output);
    }

    [Fact]
    public async Task Should_return_if_found_successfully()
    {
        await using var provider = new ServiceCollection()
            .AddTelemetryListener(_output)
            .AddMassTransitTestHarness(_output, x => x.AddConsumer<AccountInformationConsumer, AccountInformationConsumerDefinition>())
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<RequestAccountStatus>();

        var response = await client.GetResponse<AccountStatus>(new RequestAccountStatus
        {
            AccountNumber = "123456"
        });

        Assert.Equal("Good", response.Message.Status);
    }

    [Fact]
    public async Task Should_return_not_available_if_not_found()
    {
        await using var provider = new ServiceCollection()
            .AddTelemetryListener(_output)
            .AddMassTransitTestHarness(_output, x => x.AddConsumer<AccountInformationConsumer, AccountInformationConsumerDefinition>())
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<RequestAccountStatus>();

        var response = await client.GetResponse<AccountStatus, AccountNotAvailable>(new RequestAccountStatus
        {
            AccountNumber = "404"
        });

        Assert.True(response.Is(out Response<AccountNotAvailable>? notAvailable));
        Assert.Equal("Account not found", notAvailable.Message.Reason);
    }

    [Fact]
    public async Task Should_throw_exception_if_not_found()
    {
        await using var provider = new ServiceCollection()
            .AddTelemetryListener(_output)
            .AddMassTransitTestHarness(_output, x => x.AddConsumer<AccountInformationConsumer, AccountInformationConsumerDefinition>())
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<RequestAccountStatus>();

        await Assert.ThrowsAsync<RequestFaultException>(async () => await client.GetResponse<AccountStatus>(new RequestAccountStatus
        {
            AccountNumber = "404"
        }));
    }
}