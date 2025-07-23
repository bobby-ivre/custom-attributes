// ----------------------------------------------------------------------------------------------------------------------------------------
// Author:				Bobby Greaney
// ----------------------------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;

namespace LughNut.CustomAttributes
{
    /// <summary>
    /// Calls a function when the field is changed
    /// </summary>
    public class OnChangedCallAttribute : PropertyAttribute
    {
        public string methodName;
        public OnChangedCallAttribute(string methodNameNoArguments)
        {
            methodName = methodNameNoArguments;
        }
    }
}
