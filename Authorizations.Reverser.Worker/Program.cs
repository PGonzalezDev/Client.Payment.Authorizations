using Authorizations.Reverser.Worker;
using Authorizations.Reverser.Worker.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IClientPaymentAuthorizationService, ClientPaymentAuthorizationService>();
        services.AddSingleton(context.Configuration);
    })
    .Build();

host.Run();
