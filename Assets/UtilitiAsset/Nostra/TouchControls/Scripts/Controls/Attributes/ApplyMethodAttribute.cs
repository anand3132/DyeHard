using System;

namespace nostra.input
{
    [AttributeUsage ( AttributeTargets.Method, Inherited = true, AllowMultiple = false )]
    public class ApplyMethodAttribute : Attribute
    {

    };
}
