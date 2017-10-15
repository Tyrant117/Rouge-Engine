using System;
using UnityEngine;

namespace Rougelikeberry
{
    public class EnumExtensions
    {
        public static bool HasFlag(Enum variable, Enum value)
        {
            if(variable.GetType() != value.GetType())
            {
                if (LogFilter.logError) { Debug.LogFormat("Checked flags are not the same type"); }
            }

            ulong num = Convert.ToUInt64(value);
            ulong num2 = Convert.ToUInt64(variable);

            return (num2 & num) == num;
        }

    }
}