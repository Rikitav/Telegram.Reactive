using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Immutable;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Reactive.FilterAttributes;
using Telegram.Reactive.Handlers;
using Telegram.Reactive.Hosting;

namespace Telegram.Reactive.Tests.ConsoleHost.Handlers.DebugHandlers
{
    [CommandHandler, CommandAllias("script")]
    public class ScriptCommandHandler : CommandHandler
    {
        private static readonly MetadataReference[] scriptReferences = Directory.GetFiles(Environment.CurrentDirectory, "*.dll").Select(dll => MetadataReference.CreateFromFile(dll)).ToArray();
        private static readonly string[] scriptImports = ["Telegram.Bot.Types", "Telegram.Bot.Types.Enums", "Telegram.Bot"];
        private static readonly ScriptOptions scriptOptions = ScriptOptions.Default.AddImports(scriptImports).AddReferences(scriptReferences);

        private static readonly string[] RestrictedKeywords =
        [
            "System.Diagnostics",
            "System.IO"
        ];

        private static readonly long[] RestrictedUsers =
        [

        ];

        private const string scriptInitCode = "System.Console.SetOut(OutputWriter);";

        public override async Task Execute(AbstractHandlerContainer<Message> container, CancellationToken cancellation)
        {
            if (Input.From is not { Id: > 0 })
                return;

            if (Input.Text is not { Length: > 0 })
                return;

            if (RestrictedUsers.Contains(Input.From.Id))
            {
                await Reply("I will not exec this script, " + Input.From.FirstName + ", you're on a naughty list.", cancellationToken: cancellation);
                return;
            }

            if (Input is not { Entities.Length: > 0 })
            {
                await Reply("Please, provide some script using monowidth block", cancellationToken: cancellation);
                return;
            }

            MessageEntity? codeEntity = Input.Entities.FirstOrDefault(entity => entity.Type == MessageEntityType.Pre);
            if (codeEntity == null)
            {
                await Reply("Please, provide some script using monowidth block", cancellationToken: cancellation);
                return;
            }

            if (codeEntity.Language != null && codeEntity.Language != "cs" && codeEntity.Language != "C#" && codeEntity.Language != "csharp")
            {
                await Reply("Script must be written in C# (current lang : " + codeEntity.Language + ")", cancellationToken: cancellation);
                return;
            }

            string codeSubstring = Input.Text.Substring(codeEntity.Offset, codeEntity.Length);
            string[] restrictedFound = RestrictedKeywords.Where(keyword => codeSubstring.Contains(keyword)).ToArray();

            if (restrictedFound.Length != 0 && Input.From?.Id != 1483578671)
            {
                await Reply("Found restricted keywords to use :\n" + string.Join("\n", restrictedFound), cancellationToken: cancellation);
                return;
            }

            Script<object> script = CSharpScript.Create<object>(scriptInitCode, scriptOptions, typeof(ScriptGlobals)).ContinueWith(codeSubstring);
            ImmutableArray<Diagnostic> diagnostics = script.Compile(cancellation);
            
            if (diagnostics.Any())
            {
                await Reply("Failed to compile script. Diagnostics :", cancellationToken: cancellation);
                StringBuilder stringBuilder = new StringBuilder();

                foreach (Diagnostic diagnostic in diagnostics)
                {
                    string diagnosticStr = diagnostic.ToString();
                    if (stringBuilder.Length + diagnosticStr.Length > 1000)
                    {
                        await Responce(stringBuilder.ToString(), cancellationToken: cancellation);
                        stringBuilder.Clear();
                    }

                    stringBuilder.AppendLine(diagnosticStr);
                }

                await Responce(stringBuilder.ToString(), cancellationToken: cancellation);
            }
            else
            {
                StringBuilder outputBuider = new StringBuilder();
                TextWriter outputWriter = new StringWriter(outputBuider);
                ScriptGlobals scriptGlobals = new ScriptGlobals(outputWriter, null, Client, Input.Chat);

                try
                {
                    ScriptState<object> state = await script.RunAsync(scriptGlobals, cancellation);
                    await Responce(outputBuider.ToString(), cancellationToken: cancellation);
                }
                catch (Exception exc)
                {
                    await Reply("Script faulted with exception", cancellationToken: cancellation);
                    await Responce(exc.Message, cancellationToken: cancellation);
                    return;
                }
            }
        }

        public class ScriptGlobals(TextWriter outputWriter, TelegramBotHost? botHost, ITelegramBotClient tgBotClient, Chat thisChat)
        {
            public TextWriter OutputWriter { get; } = outputWriter;
            public TelegramBotHost? Host { get; } = botHost;
            public ITelegramBotClient Client { get; } = tgBotClient;
            public Chat ThisChat { get; } = thisChat;
        }
    }
}
