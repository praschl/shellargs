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
            FluentWithOption();
            Documentation_GettingStartedMain();
            Documentation_WithOption();
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

            parser.AutoWire<User>(
                u => u.WithOption(x => x.Name).Do(p => { })
                    .WithOption(x => x.Count).Do(pc => counter += pc.Value)
                    .WithOption(x => x.Variables.CurrentValue()).Do(pc => Console.WriteLine(pc.Container.Variables))
                );

            parser.WithOption(b => b.Named("AddInt")
                .Alias("add", "a")
                .AtPosition(1)
                .Required()
                .As<int>()
                .Do(i => counter += i.Value)
                );

            parser.OnOptionParsed(pc => Console.WriteLine(pc.Option, pc.Value));

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

            parser.AutoWire<ValueHolder>();
            parser.AutoWire<User>();

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

            parser.AutoWire(user);
            parser.AutoWire(holder);

            parser.Parse("name", "me", "-value", "1");

            Console.WriteLine(user);
            Console.WriteLine(holder);
        }

        private static void FluentWithOption()
        {
            var parser = new Parser();

            parser.WithOption("demo1",
                b => b.AtPosition(1)
                    .Required()
                    .As<int>() // single value
                    .Do(pc => Console.WriteLine(pc.Option, pc.Value)));

            parser.WithOption("demoCollection",
                b => b.Collection.As<int>() // collections
                    .Do(pc =>
                        {
                            Console.WriteLine(pc.Option);
                            Console.WriteLine(pc.Value);
                            // NOTE: the following is planned, but not implemented yet
                            //pc.Parser.WithOption("dynamicNewOption", o => o.As<int>().Do(Console.WriteLine));
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

        private static void Documentation_WithOption()
        {
            int sum = 0;

            var parser = new Parser();

            parser.WithOption(b => b.Named("add")
                .Alias("a", "ad")
                .ValueDescription("a number")
                .AtPosition(1)
                .Required()
                .As<int>()
                .Do(pc => sum += pc.Value));

            parser.WithOption(b => b.Named("sub")
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