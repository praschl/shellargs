# Getting started #
First download the latest package, unzip it, and add a reference to MiP.ShellArgs.dll to your project.

## Most simplistic way to parse arguments to a class ##
### Define the argument container ###
Create a class and create a property for each option you want to make available to the user:
```
public enum Gender { Female, Male }
public class MyArgs
{
  public Gender Gender { get; set; }
  public string Name { get; set; }
  public DateTime DayOfBirth { get; set; }
  public int Weight { get; set; }
}
```

The created options [can be customized](Autowiring.md) by adding Attributes to the properties.

### Parse the arguments into the container ###
To parse the `string[] args` from your Main() method to the container class, add a `using MiP.ShellArgs;` and call
```
  public static void Main(string[] args)
  {
    MyArgs p = Parser.Parse<MyArgs>(args);

    Console.WriteLine("Gender: {0}", p.Gender);
    Console.WriteLine("Name: {0}", p.Name);
    Console.WriteLine("Birthday: {0}", p.DayOfBirth);
    Console.WriteLine("Weight: {0}", p.Weight);
  }
```

### Compile and test ###
Call your program from the console with
```
> Test.exe -gender Male -name Buddy -dayofbirth 2007-5-16 -weight 16
```

If you just want to try it out from your tests, call it like this
```
  [TestMethod]
  public void ArgsTest()
  {
    MyArgs p = Parser.Parse<MyArgs>(
               "-gender", "Male", "-name", "Buddy", "-dayofbirth", "2007-5-16", "-weight", "16");
  }
```

The arguments will be parsed to an instance of `MyArgs`, and contain the values.