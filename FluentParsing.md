# Parsing args with the fluent api #
## Setting up a global handler for all options ##
Instead of setting up a handler for a specific options for autowired options or stand alone options, you can set up a handler which is called for every option:
```
   parser.OnOptionParsed(pc => Console.WriteLine(pc.Option, pc.Value));
```
Note that `ParsingContext.Value` for this handler is typed to object.

## Parsing ##
Once you have set up your parser with [Autowiring](Autowiring.md) and/or the fluent WithOption methods, you can pass the `string[] args` from your `static Main()` to the parsers `Parse(params string[] args)` method. While the arguments are parsed, your callbacks will be called for the options you configured.

## Getting a result ##
For all stand alone options, you have to remember the value somewhere, otherwise it is lost after `Parse()` returns. For autowired options, the values are stored an a container which is instantiated by the parser, if you didnt pass a default instance.

If you didnt pass a default instance for your auto wired options, you can get the containers from the result of the `Parse()` method.

### IParsingResult ###
`IParsingResult` contains two methods, `Result<TContainer>` returns the instance of the container which has the type `TContainer`. The other method, `ResultTo<TContainer>(out TContainer result)` writes the instance to the passed out variable. Note that the compiler can infer the type parameter for from the parameter.