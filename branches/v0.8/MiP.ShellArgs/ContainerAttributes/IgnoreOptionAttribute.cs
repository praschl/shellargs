using System;

namespace MiP.ShellArgs.ContainerAttributes
{
    /// <summary>
    /// Used to ignore the property, and exclude it from autowiring.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreOptionAttribute : Attribute
    {
    }
}