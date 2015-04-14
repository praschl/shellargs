# Defining stand alone options #
Options can easily be defined by autowiring a container, but they can also be created with the `WithOption()` method of the parser. The parameter is a delegate which can be used to customize the option, here is a complete example:
```
    int sum = 0;

    new Parser()
        .WithOption(b => b.Named("add")
                          .Alias("a", "ad")
                          .ValueDescription("a number")
                          .AtPosition(1)
                          .Required()
                          .As<int>()
                          .Do(pc => sum += pc.Value))
        .WithOption(b => b.Named("sub")
                          .Collection
                          .As<int>()
                          .Do(pc => sum -= pc.Value))
        .Parse("-add", "10", "-sub", "1", "2");

    Console.WriteLine("Sum: {0}", sum);   
```

As you can see, you can do anything which can be done with autowiring, it just looks a bit different. The only important difference are the calls to `.As<TOption>()` which defines the type of the option and the call to `.Do(Action<ParsingContext<TOption>)` where you can get the parsed value from and store it somewhere.

To define that the option should be presented to the user as a collection, use the `.Collection` property.