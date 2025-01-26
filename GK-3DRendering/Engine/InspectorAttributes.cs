using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_3DRendering.Engine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InspectorIgnoreAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InspectorSliderAttribute<T> : Attribute
    {
        public T Min { get; }
        public T Max { get; }

        public InspectorSliderAttribute(T min, T max)
        {
            Min = min;
            Max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class InspectorColorAttribute : Attribute
    { }
}
