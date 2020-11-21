namespace Template_NET_5_WORKER.CoreService.Helper
{
    using System;
    using System.Text.RegularExpressions;

    using MassTransit;
    using MassTransit.Definition;

    public class CustomEndpointNameFormatter : DefaultEndpointNameFormatter
    {
        private static readonly Regex _pattern = new Regex("(?<=[a-z0-9])[A-Z]", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
        private readonly string _separator;
        private readonly string _prefix;

        public CustomEndpointNameFormatter()
        {
            this._prefix = "MyApp";
            this._separator = "-";
        }

        public CustomEndpointNameFormatter(string prefix, string separator)
        {
            this._prefix = prefix;
            this._separator = separator;
        }

        public static new IEndpointNameFormatter Instance { get; } = new CustomEndpointNameFormatter();

        public override string SanitizeName(string name)
        {
            return $"{this._prefix}{this._separator}{_pattern.Replace(name, m => this._separator + m.Value).ToLowerInvariant()}";
        }
    }
}