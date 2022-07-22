using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using neo_task.rabbitmq;
using neo_task.taskprocessor.Models;
using neo_task.taskprocessor.Services;
using Microsoft.Extensions.Configuration;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration config = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json")
         .AddEnvironmentVariables()
         .Build();
        services.AddSingleton<IConnectionProvider, ConnectionProvider>();
      
        services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
            "neo-task", "neo-task.tasks", "neo-task.tasks", ExchangeType.Topic));

        services.AddHostedService<TaskProcessorService>();
        services.AddDbContext<NeoTaskModelContext>(opt =>opt.UseInMemoryDatabase("NeoTask"));


    })
    .Build();

await host.StartAsync();



await host.WaitForShutdownAsync();