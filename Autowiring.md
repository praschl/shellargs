# Autowiring container classes to create options #
## Introduction ##
Its not necessary to create each option manually with the fluent api, instead set up a class with properties, add some attributes and your options can be created automatically.

## Creating a container class ##
### Example ###
Here is a simple example of a container:
```
public class MyArgs
{
  [Option("in")]
  [Aliases("i")]
  [Position(1)]
  [Required]
  [ValueDescription("input file")]
  public string Input {get; private set; }
  
  [Option("out")]
  [Aliases("o")]
  [Position(2)]
  [ValueDescription("output file")]
  public string Output {get; private set; }
  
  [Aliases("n")]
  public List<int> Numbers {get; set;}
}
```

This generates the two options
`[-in] <input file> [[-out ]<outputfile>]`

### Requirements for the properties ###
For each property in the class an option will be generated with the name and type of the property. Options can not be generated for property which
  * do not have a getter
  * do not have a setter and do not implement `ICollection<T>`
  * have the `[IgnoreOptionAttribute]` set

Getters and setters may be private, protected or internal. A setter is not required at all, but if the property does not have a setter at all (not even a private one) the propertys type must implement `ICollection<T>` and be initialized in the constructor. Properties which are of a type which implements `ICollection<T>` do not need to be initialized if a setter is present, the parser will try to create an instance of the collection itself.

The property may be of any .NET type. The Parser supports many of the [standard .NET types](SupportedTypes.md), enums and `Nullable<T>`. Collections which implement `ICollection<T>` are also supported and [support for custom types](SupportedTypes#Custom_types.md) is easy to add.

### Attributes ###
The generated options can be customized by attaching the following .NET attributes to the properties
| `[OptionAttribute]` | change the name of the option |
|:--------------------|:------------------------------|
| `[AliasesAttribute]` | add aliases to the option |
| `[PositionAttribute]` | make the option positional |
| `[RequiredAttribute]` | make the option required |
| `[ValueDescriptionAttribute]` | add a short description for the options value |
| `[IgnoreOptionAttribute]` | do not generate an option from this property |

By default, an option has the same name as the property. To rename the option use the `[Option("newName")]`.
Aliases can be added to the option by adding `[Aliases("a1", "a2")]`

The options name and aliases are all case insensitive.

Make the option positional with `[Position(1)]`. The option can then be used as first argument without specifying the name of the option. Positional options have a few [restrictions](PositionalOptions.md), too.

`[Required]` makes the option required. The used must specify a value for it.

The description of an option is the type of the property or - if the option is an enum - the possible enum values. This may not be descriptive enough for the user to guess what he should pass. Override the description text with `[ValueDescription("filename")]` makes it much easier for the user to guess than just "string".

## Adding the container class to the parser ##
### static Parse() ###
The first method of parsing arguments to the class is to use one of the static `Parse()` methods.
```
  MyArgs myArgs = Parser.Parse<MyArgs>(args);
```
This creates an instance of `MyArgs` parses the arguments into the properties and returns it.

#### Overloads ####
The following overloads are available:
```
  var settings = new ParserSettings();
  
  MyArgs myArgs = Parser.Parse<MyArgs>(settings, args);
  // or
  myArgs = new MyArgs();
  Parser.Parse(myArgs, args);
  // or
  Parser.Parse(myArgs, settings, args);
```

You can either let the parser create an instance of your container class or create it yourself (if you want to set some defaults).
The other overloads let you specify an instance of ParserSettings which is used to change the parsers behavior.

## Fluent api ##
### Adding the container ###
To add a container to the parser you can also use the fluent api:
```
  Parser parser = new Parser()
         .AutoWire<MyArgs>();
```

Note that you may add more than one container to the parser, which will all be used to create options. This way you can separate complex arguments into multiple classes. Restrictions apply. Option names and aliases must be unique and positions must be unique and sequential.

### Call a handler when an option is parsed ###
If you want to know when an option was parsed, subscribe a handler with this code:
```
  Parser parser = new Parser()
         .AutoWire<MyArgs>(

               // after parsing a value for the "in" option
         b => b.WhenParsed(x => x.Input)  

               // call DoSomething with the ParsingContext
               .Do(pc => DoSomething(pc)) 

               // after parsing a value for the "out" option
               .WhenParsed("out")
               .Do(pc => { Console.WriteLine(pc.Container.Value); })
               );
```

This will call `DoSomething(ParsingContext pc` when a value for the option was parsed and set to the property. If the property is not accessible from where you create the parser because it is `private/internal/protected`, you can also pass the options name as a string, but I recommend using the `Expression<T>` overload, because it makes refactor/rename easier and safer.

### Use a handler when an option for a collection is parsed ###
In case your property is a collection, you need to use the extension method `ICollection<T>.CurrentValue()` to define the correct callback:
```
  Parser parser = new Parser()
         .AutoWire<MyArgs>(

                // after parsing a value for the Numbers property and adding it to the collection
         b => b.WhenParsed(x => x.Numbers.CurrentValue()) 
               
               // call DoSomething with the ParsingContext
               .Do(pc => DoSomething(pc))
               );
```

### ParsingContext ###
The `class ParsingContext<TContainer, TArgument>` has the following properties
| `TContainer Container` | Access to the container for which the handler is called |
|:-----------------------|:--------------------------------------------------------|
| `string Option` | Name of the parsed option |
| `TValue Value` | Value which was parsed from the args |