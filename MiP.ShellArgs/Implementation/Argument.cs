namespace MiP.ShellArgs.Implementation
{
    internal class Argument
    {
        public Argument()
        {
            Name = string.Empty;
            Value = string.Empty;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public bool HasName => !string.IsNullOrEmpty(Name);

        public bool HasValue => !string.IsNullOrEmpty(Value);
    }
}