using System;

namespace kuujinbo.StackOverflow.iTextSharp.Helpers
{
    public static class Misc
    {
        public static string RepeatChar(char c, int n)
        {
            return new String(c, n);
        }

        public static string ClassName(object o)
        {
            return o.GetType().Name;
        }


    }
}