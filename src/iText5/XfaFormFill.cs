using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// http://stackoverflow.com/questions/41510154
namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class XfaFormFill
    {
        public static readonly string XML_INFILE = Helpers.IO.GetInputFilePath("i-9_data.xml");
        public PdfReader PDF_READER = Helpers.IO.GetInputReader("i-9.pdf");
        public string OUTFILE;

        public XfaFormFill()
        {
            OUTFILE = Helpers.IO.GetClassOutputPath(this);
        }

        public string FillXml(Dictionary<string, string> fields)
        {
            // XML_INFILE => physical path to XML file exported from I-9
            XDocument xDoc = XDocument.Load(XML_INFILE);
            foreach (var kvp in fields)
            {
                // handle multiple elements in I-9 form
                var elements = xDoc.XPathSelectElements(
                    string.Format("//{0}", kvp.Key)
                );
                if (elements.Count() > 0)
                {
                    foreach (var e in elements) { e.Value = kvp.Value; }
                }
            }

            return xDoc.ToString();
        }

        public void Go()
        {


            var fields = new Dictionary<string, string>()
            {
                { "textFieldLastNameGlobal", "Doe" },
                { "textFieldFirstNameGlobal", "Jane" }
            };
            var filledXml = FillXml(fields);

            using (var ms = new MemoryStream())
            {
                // PDF_READER => I-9 PdfReader instance
                using (PDF_READER)
                {
                    // I-9 has password security
                    PdfReader.unethicalreading = true;
                    // maintain usage rights on output file
                    using (var stamper = new PdfStamper(PDF_READER, ms, '\0', true))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(filledXml);
                        stamper.AcroFields.Xfa.FillXfaForm(doc.DocumentElement);
                    }
                }
                File.WriteAllBytes(OUTFILE, ms.ToArray());
            }


        }
    }
}