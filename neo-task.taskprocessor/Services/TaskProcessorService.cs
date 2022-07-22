using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using neo_task.rabbitmq;
using neo_task.taskprocessor.Models;


namespace neo_task.taskprocessor.Services
{
    public class TaskProcessorService : IHostedService
    {
        public readonly ISubscriber _subscriber;
        private readonly NeoTaskModelContext _context;

        public TaskProcessorService(ISubscriber subscriber, IServiceScopeFactory factory)
        {
            this._subscriber = subscriber;
            this._context = factory.CreateScope().ServiceProvider.GetRequiredService<NeoTaskModelContext>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.Subscribe(ProcessMessage);
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool ProcessMessage(string message, IDictionary<string, object> headers)
        {
            Console.WriteLine($"[x] Rabbit Message Received : {message}");

            //save to in memory database
            var neotask = JsonSerializer.Deserialize<NeoTaskModel>(message);
          
            _context.NeoTasks.Add(neotask);
            _context.SaveChanges();

            return true;
        }

    }
}
