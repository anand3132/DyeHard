using System;

namespace nostra.input
{
    [AttributeUsage ( AttributeTargets.Field, Inherited = true, AllowMultiple = false )]
    public class LabelAttribute : Attribute
    {
        public readonly string label;

        public LabelAttribute ( string label )
        {
            this.label = label;
        }
    };
}
