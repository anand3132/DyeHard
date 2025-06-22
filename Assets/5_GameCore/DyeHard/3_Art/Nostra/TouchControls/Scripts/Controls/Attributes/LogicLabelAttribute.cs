using System;

namespace nostra.input
{
    [AttributeUsage ( AttributeTargets.Field, Inherited = true, AllowMultiple = false )]
    public class LogicLabelAttribute : Attribute
    {
        public readonly string memberName, trueLabel, falseLabel;

        public LogicLabelAttribute ( string memberName, string trueLabel, string falseLabel )
        {
            this.memberName = memberName;
            this.trueLabel = trueLabel;
            this.falseLabel = falseLabel;
        }
    };
}
