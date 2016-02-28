using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using HtmlAgilityPack;
using iTextSharp.tool.xml.pipeline.html;

namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.XmlWorkers
{
    public class ImageHandler
    {

        
    }

    class ImageProvider : IImageProvider
    {
        public string GetImageRootPath()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public Image Retrieve(string src)
        {
            throw new NotImplementedException();
        }

        public void Store(string src, Image img)
        {
            throw new NotImplementedException();
        }
    }
}