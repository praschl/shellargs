using System;

namespace MiP.ShellArgs.ContainerAttributes
{
    /// <summary>
    /// Used to specify that the option is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredAttribute : Attribute
    {
    }
}