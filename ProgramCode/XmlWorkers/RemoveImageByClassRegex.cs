using System;
using System.Text.RegularExpressions;

// http://stackoverflow.com/questions/35216731 - PART 2
namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.XmlWorkers
{
    public class RemoveImageByClassRegex
    {
        const string HTML = @"
<div>
    <p class='img-desktop'>Paragraph</p>
    <div>
        <img src='somepath/desktop.jpg' class='img-desktop'>Desktop</img>
        <img src='somepath/mobile.jpg' class='img-mobile'>Mobile</img>
    </div>
    <div>
        <img src='somepath/desktop.jpg' alt='img-desktop' title='img-desktop' class=""img-desktop"">Desktop
</IMG>
        <img src='somepath/mobile.jpg' class='img-mobile'>Mobile</img>
    </div>
</div>";

        public void Go()
        {
            var line = new String('=', 40);
            var regex = new Regex(
                @"<img[^>]*class='?""?'?img-desktop""?[^>]*>.*?</img>",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline
            );

            Console.WriteLine("{0}\nBEFORE:\n{0}{1}", line, HTML);
            Console.WriteLine();
            Console.WriteLine("{0}\nAFTER:\n{0}{1}", line, regex.Replace(HTML, ""));
        }

    }
}