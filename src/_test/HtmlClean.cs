using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace kuujinbo.StackOverflow.iTextSharp._test
{
    class HtmlClean
    {
        public void Go()
        {
    var hDocument = new HtmlDocument()
    {
        OptionWriteEmptyNodes = true,
        OptionAutoCloseOnEnd = true
    };
    hDocument.LoadHtml("<div><img src='a.gif'><br><hr></div>");
    var closedTags  = hDocument.DocumentNode.WriteTo();
            Console.WriteLine(closedTags);
        }

    }
}
