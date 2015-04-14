# Passing arguments to the parser #
Simply put, when the shell calls your program, it separates the argument string by space. If the user wants to pass a string that contains spaces, quotation marks need to be used. Your program gets an array of strings, containing an argument each.

When you pass this array to the `Parse(params string[] args)` method of the parser it tries to find options and values in the strings.

## Passing options ##
Options can be started with the prefix "/" or "-" and are not case sensitive. The following arguments mean the same:
```
*.exe /user michael
*.exe -User michael
```
Case sensitivity cannot be changed (yet), but you can override the default prefixes in the ParserSettings.

## Passing values ##
Values can be passed to the option as a separate argument or by assinging it. The following arguments mean the same:
```
*.exe /user michael
*.exe /user=michael
*.exe /user:michael
```

Boolean options do not require a value to be passed, which negates the default value. Assuming the default value is false, it can be set to true in the following ways:
```
*.exe /enable
*.exe /enable=true
*.exe /enable true
*.exe /enable+
```
The negative values are "false" or "-". Note that the short boolean values can not be passed as a separate argument, and they cannot be changed.

## Flags Enums ##
Enums and flags enums can be parsed. Flags enums have to be passed comma separated, and must be convertible with `Enum.Parse(Type enumType, string value, bool ignoreCase)`.
```
[Flags]
public enum MyFlags
{
  Off  = 0x00,
  One  = 0x01,
  Two  = 0x02,
  Four = 0x04
}
```
Values for this enum can be passed as `/flags:One,Four` or `/flags 5`

## Collections versus Single options ##
Single options mean that only one value can follow the option, and passing a second value will yield an error message telling that an option was expected after this first value. The option can still be used multiple times, but if it was autowired, the value in the container will be overwritten.

The parser supports option types which implement `ICollection<T>` with autowiring and with the fluent api. Options which are a collection can have multiple values after the option and all values will be added to the container and the handler will be called for each value added.
```
# single
*.exe /firstname michael

# this will produce an error
*.exe /fullname john koenig

# this is ok again
*.exe /fullname "john koenig"

# collection will contain 3 entries
*.exe /users tick trick track

# passing an option multiple times
*.exe /firstname dagobert /allow:true /firstname donald /allow:false
```
If the "firstname" and "allow" options are both single value options, the properties of the autowire container will be "donald" and "false". If both options were collections, they would yield the values `{"dagobert", "donald"} ` and `{true, false} ` after parsing.

## Positional options ##
Options which are defined as positional may be used by just passing the value if it occurs at the correct position. If you have defined "firstname", "lastname", "hat" and "nephews" with positions 1, 2, 3 and 4 you can omit the option and just pass the values to `Parse()`:
```
*.exe Scrooge McDuck stovepipe
```

Positional arguments are allowed without option as long as no named option is used. If you pass an option with its name, the following options must all be passed with named:
```
*.exe Donald -lastname Duck -hat "sailor's hat"
```

Positional arguments are not necessary required, but you cannot skip positional options. If you want to not specify one, you have to specify the name:
```
*.exe Mickey -hat none
```

The last positional option may be a collection:
```
*.exe Donald Duck "sailor's hat" Huey Dewey Louie
```

Options can be positional and required at the same time and you can also mix required and non-required positional options. But sorted by position, required options must be the first and all non-required options must follow.

## Converting values to the options type ##
Values for options are converted to the options actual type before a handler is called (the converted value is given to the handler in an instance of `ParsingContext<T>`). Here is a list of [supported types](SupportedTypes.md).