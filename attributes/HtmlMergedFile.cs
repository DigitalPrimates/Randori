using SharpKit.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace randori.attributes
{
    [JsType(Export = false)]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class HtmlMergedFile : Attribute
    {
        // Summary:
        //     The target merged file name
        public string Filename
        {
            get;
            set;
        }
        //
        // Summary:
        //     The source files to merge
        public string[] Sources
        {
            get;
            set;
        }
    }
}
