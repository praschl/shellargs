# Customizing the parser #
There are some settings with which the parsers behavior can be changed. Here is a list of customizable settings:
  * Prefix for options
  * Character for assignments
  * Boolean switches
  * Custom StringParsers

To customizer the parser either use an instance of ParserSettings which you pass to the static `Parse()` methods or call the `Customize(Action<ParserSettings> customizer)` instance method of the fluent api.

## Prefixes ##
The prefixes are characters which start an option. To set the characters which may start an option use
```
  settings.PrefixWith(params char[] prefixes);
```
At least one character must be passed, and you should make sure its some kind of special character.

The default value is `char[] {'/', '-'} `.

## Assignments ##
The assignment character is used to separate a passed argument when the user entered something like /name=michael. Change the default with
```
  settings.AssignWith(params char[] prefixes);
```
Unlike `PrefixWith` it is allowed to pass an empty array, which effectively forbids using assignments like this.

The default value is `char[] {'=', ':'} `

## Short boolean assignment ##
Boolean options can be parsed in many ways.
Pass the value as in `/bool=true` or `/bool=false`, or even omit the value completely (which negates the default value).
Or you can use the short boolean syntax to pass the values `/bool+` or `/bool-`.
You can disable the short boolean syntax with
```
  settings.EnableShortBooleans(false);
```
By default, the behavior is turned on.

## Registering custom string parsers ##
The `ParserSettings` instance can be used to add custom string parsers which the parser can use to parse values to custom classes.
To add a new string parser you wrote use
```
  settings.ParseTo<User>().With<UserParser>();
  // or
  settings.ParseTo<User>().With(new UserParser());
```
By default the `StringToObjectParser`, the `StringToEnumParser` and the `StringToBoolParser` are already added to the instance; see SupportedTypes for their description.