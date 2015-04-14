# Types supported by the parser #
By default, the parser supports all types which can be parsed with a `TypeConverter`.

For help with creating a `TypeConverter` see http://msdn.microsoft.com/en-us/library/system.componentmodel.typeconverter.aspx

## System types ##
This includes the following types
  * `System.String`
  * `System.Char`
  * `System.Int16`
  * `System.Int32`
  * `System.Int64`
  * `System.UInt16`
  * `System.UInt32`
  * `System.UInt64`
  * `System.Boolean`
  * `System.Single`
  * `System.Double`
  * `System.DateTime`
  * `System.TimeSpan`

## Enums ##
The parser can convert Enums from numbers or the Enum names (case insensitive). `[Flags]` enums are supported and can be parsed from comma separated strings.

## Nullable`<T>` ##
All value types may be used with `Nullable<T>`. Note that this does mean, that the option is required. If the option is not passed by the user, the value will be `null` after parsing.

## Collections ##
Collections of any type are supported as long as `ICollection<T>` is implemented and `T` can be parsed. The type of the property may be a concrete type of a collection or just `ICollection<T>`. If the property has a setter and is not initialized to an instance of the collection, the parser will try to create an instance. The parser can not create an instance for abstract types or types which have no default constructor. In these cases, initialize the property in the constructor of the container.

## KeyValuePair ##
Dynamic properties are supported by adding a collection of `KeyValuePair<TKey, TValue>`. This may also be a dictionary. However, `TKey` has to be of the type `string`. This allows to support properties like msbuild has them: `/p:compile=true /p:config=Release /p:out=`. `TValue` may also be of any supported type, except collections (which does not make sense imo).

Be careful when using `Dictionary<string,T>`. If the user tries to use the same property multiple times an exception will be raised, telling that "An item with the same key has already been added". You can avoid that by using `Collection<KeyValuePair<string,TValue>>`.

## Custom types ##
To add support from custom types
  * derive from `MiP.ShellArgs.StringConversion.StringParser<TTarget>` (recommended) or
  * implement `MiP.ShellArgs.StringConversion.IStringParser<out TTarget>`
  * add the type or an instance of it to the `MiP.ShellArgs.ParserSettings`

The easiest way is to derive from `StringParser<TTarget>`.

`ValueDescription` default implementation does return null. If overridden the value it returns will take precedence over the default value description. The description can still be overridden with the `[ValueDescriptionAttribute]` or the `ValueDescription(string)` method of the fluent api.

Override `CanParse(Type)` to decide if your parser can parse to the specified type.

Override `IsValue(Type, string)` to decide if your parser can parse the value to the specified type.

`TTarget Parse(string value)` is the method which does the actual work. Override it and parse the string to your custom type.

## Predefined Stringparsers ##
The following string parsers are predefined and always added to the parser:

`StringToKeyValuePairParser` used to parse arguments of the format "key=value" into a `KeyValuePair<string,TValue`.

`StringToBoolParser` can also parse "+" to true and "-" to false.

`StringToEnumParser` can parse enums case insensitive, but will try with a `TypeConverter` before using `Enum.Parse()`.

`StringToObjectParser` is a fallback parser which tries to find a `TypeConverter` for the type, and uses this to parse the value. This parser always returns true for `CanParse()` and `IsValid()`. This parser is the one which will be picked, when no other parser could be used. The `TypeConverter` used by this parser may raise an exception, if it can not parse the string.

Enums can only be parsed case sensitive with the with the `TypeConverter`; if the value is not valid for the enum, the value is tried with `Enum.Parse()`.