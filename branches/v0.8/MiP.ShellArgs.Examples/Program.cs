using System;
using System.Collections.Generic;

using MiP.ShellArgs.StringConversion;

namespace MiP.ShellArgs.Examples
{
    internal class Program
    {
        private static void Main()
        {
            Simple();
            Complex();
            TwoInstancesGeneric();
            TwoExistingInstances();
            FluentRegisterOption();
            Documentation_GettingStartedMain();
            Documentation_RegisterOption();
        }

        private static void Simple()
        {
            var user = Parser.Parse<User>("-name", "me");
            Console.WriteLine(user);
        }

        private static void Complex()
        {
            int counter = 0;

            var settings = new ParserSettings();
            settings.ParseTo<object>().With<StringToObjectParser>();
            settings.ParseTo<bool>().With<StringToObjectParser>();
            settings.PrefixWith('/', '-');
            settings.AssignWith(':', '=');
            settings.EnableShortBooleans(true);

            var parser = new Parser(settings);

            parser.RegisterContainer<User>(
                u => u.With(x => x.Name).Do(p => { })
                    .With(x => x.Count).Do(pc => counter += pc.Value)
                    .With(x => x.Variables.CurrentValue()).Do(pc => Console.WriteLine(pc.Container.Variables))
                );

            parser.RegisterOption(b => b.Named("AddInt")
                .Alias("add", "a")
                .AtPosition(1)
                .Required()
                .As<int>()
                .Do(i => counter += i.Value)
                );

            parser.RegisterOption("SubInt", b => b.As<int>().Do(pc => Console.WriteLine(pc.Option)));

            parser.OptionValueParsed += (o, e) => Console.WriteLine(e.ParsingContext.Option, e.ParsingContext.Value);

            //

            var user = parser.Parse("-name", "me", "-add", "1")
                .Result<User>();

            Console.WriteLine(user);
        }

        private static void TwoInstancesGeneric()
        {
            User user;
            ValueHolder holder;

            var parser = new Parser();

            parser.RegisterContainer<ValueHolder>();
            parser.RegisterContainer<User>();

            parser.Parse("name", "me", "-value", "1")
                .ResultTo(out user)
                .ResultTo(out holder);

            Console.WriteLine(user);
            Console.WriteLine(holder);
        }

        private static void TwoExistingInstances()
        {
            var user = new User();
            var holder = new ValueHolder();

            var parser = new Parser();

            parser.RegisterContainer(user);
            parser.RegisterContainer(holder);

            parser.Parse("name", "me", "-value", "1");

            Console.WriteLine(user);
            Console.WriteLine(holder);
        }

        private static void FluentRegisterOption()
        {
            var parser = new Parser();

            parser.RegisterOption("demo1",
                b => b.AtPosition(1)
                    .Required()
                    .As<int>() // single value
                    .Do(pc => Console.WriteLine(pc.Option, pc.Value)));

            parser.RegisterOption("demoCollection",
                b => b.Collection.As<int>() // collections
                    .Do(pc =>
                        {
                            Console.WriteLine(pc.Option);
                            Console.WriteLine(pc.Value);
                            // NOTE: the following is planned, but not implemented yet
                            //pc.Parser.RegisterOption("dynamicNewOption", o => o.As<int>().Do(Console.WriteLine));
                        }));
            //
            parser.Parse("-hello", "1");
        }

        private static void Documentation_GettingStartedMain(params string[] args)
        {
            var p = Parser.Parse<MyArgs>(args);

            Console.WriteLine("Gender: {0}", p.Gender);
            Console.WriteLine("Name: {0}", p.Name);
            Console.WriteLine("Birthday: {0}", p.DayOfBirth);
            Console.WriteLine("Weight: {0}", p.Weight);
        }

        private static void Documentation_RegisterOption()
        {
            int sum = 0;

            var parser = new Parser();

            parser.RegisterOption(b => b.Named("add")
                .Alias("a", "ad")
                .ValueDescription("a number")
                .AtPosition(1)
                .Required()
                .As<int>()
                .Do(pc => sum += pc.Value));

            parser.RegisterOption(b => b.Named("sub")
                .Collection
                .As<int>()
                .Do(pc => sum -= pc.Value));

            parser.Parse("-add", "10", "-sub", "1", "2");

            Console.WriteLine("Sum: {0}", sum);
        }
    }

    public class User
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public List<string> Variables { get; set; }
    }

    public class ValueHolder
    {
        public int Value { get; set; }
    }

    public enum Gender
    {
        Female,
        Male
    }

    public class MyArgs
    {
        public Gender Gender { get; set; }
        public string Name { get; set; }
        public DateTime DayOfBirth { get; set; }
        public int Weight { get; set; }
    }
}