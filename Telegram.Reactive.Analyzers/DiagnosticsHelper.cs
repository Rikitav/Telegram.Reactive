using Microsoft.CodeAnalysis;

namespace Telegram.Reactive.Analyzers
{
    public static class DiagnosticsHelper
    {
        public const string Aspect = "Aspect";

        public static readonly DiagnosticDescriptor Test = new DiagnosticDescriptor("TR0001", "Test descriptor", string.Empty, Aspect, DiagnosticSeverity.Error, true, "Test diagnostic description.");

        public static Diagnostic Create(this DiagnosticDescriptor descriptor, Location? location, params object[] messageArgs)
            => Diagnostic.Create(descriptor, location, messageArgs);
    }
}
