using Microsoft.Extensions.Logging;

namespace Telegram.Reactive.Tests.ConsoleApp
{
    /// <summary>
    /// Main program class for the Telegram.Reactive.Tests.ConsoleApp project.
    /// Demonstrates basic usage of the ReactiveClient for console applications.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry point for the console test application.
        /// Creates a reactive client, collects handlers, and starts receiving updates.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            ReactiveClient client = new ReactiveClient("7625502724:AAGK9mnV6k-1jlAdNCzoaebeWWcUIQY-ejI");
            client.Handlers.CollectHandlersDomainWide();

            ILoggerFactory logFactory = LoggerFactory.Create(builder => builder.AddConsole());
            client.StartReceiving();
            Thread.Sleep(-1);
        }
    }
}
