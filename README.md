# Introduction #
Mip.ShellArgs.Parser is a parser for command line / shell arguments which are usually passed to the `static void Main(string[] args)` method in shell programs.

While being very [easy to use](https://github.com/praschl/shellargs/wiki/GettingStarted), it is also very flexible via its fluent API. Simple programs may require only a single line.

Name and aliases of options can be customized as well as if they are required or not.

It supports many of the [standard .NET types](https://github.com/praschl/shellargs/wiki/SupportedTypes) as ` string, bool, int, float, DateTime, TimeSpan ` but also enums, collections and you can even add converters for your custom types.

Arguments can be parsed to instances of custom classes or call event handlers.

Even a help string is automatically generated which can be used to display the usage of your program.
