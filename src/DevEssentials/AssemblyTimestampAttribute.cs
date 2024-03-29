﻿namespace System
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public class AssemblyTimestampAttribute : Attribute
    {
        public AssemblyTimestampAttribute(long ticks)
        {
            Timestamp = new DateTime(ticks);
        }

        public AssemblyTimestampAttribute(string ticks)
        {
            Timestamp = new DateTime(long.Parse(ticks));
        }

        public DateTime Timestamp { get; }

    }
}
