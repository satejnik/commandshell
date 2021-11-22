using System;

namespace CommandShell.Metadata
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreCommandAttribute : Attribute
    {
    }
}
