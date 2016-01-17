using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.ProgramCode.Forms
{
    // http://stackoverflow.com/questions/34687905/
    // TODO: figure out Adobe XFA XML checksum - probably why sabing after
    // first time is broken
    public class UsageRightsCheckbox
    {
        public static string GetXmlPath(string path)
        {
            return Helpers.IO.GetOutputPath(path, ".xml");
        }

        // still testing....
        static XElement CloneElement(XElement element)
        {
            return new XElement(element.Name,
                element.Attributes(),
                element.Nodes().Select(n =>
                {
                    XElement e = n as XElement;
                    if (e != null)
                        return CloneElement(e);
                    return n;
                })
            );
        }


        // 1. save XFA form XML stream to a file to inspect form fields
        public void SaveDatasetsXml(string readerPath)
        {
            using (var reader = new PdfReader(readerPath))
            {
                var xfa = new XfaForm(reader);
                XmlDocument doc = xfa.DomDocument;
                //// remove namespace so XML is easier to read
                //if (!string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI))
                //{
                //    doc.DocumentElement.SetAttribute("xmlns", "");
                //    var temp = new XmlDocument();
                //    temp.LoadXml(doc.OuterXml);
                //    doc = temp;
                //}

                var sb = new StringBuilder();
                var xSettings = new XmlWriterSettings() 
                {
                    Indent = true,
                    WriteEndDocumentOnClose = true
                };
                using (var writer = XmlWriter.Create(sb, xSettings))
                {
                    doc.Save(writer);
                }
                File.WriteAllText(GetXmlPath(readerPath), sb.ToString());
            }
        }

        // 2. inspect results from step [1] above and update the 'datasets'
        // section of the XFA form with desired data 
        public string EditXfaXml(string readerPath)
        {
            using (var reader = new PdfReader(readerPath))
            {
                var xfa = new XfaForm(reader);
                XmlNode datasetsNode = xfa.DatasetsNode;
                XDocument xDocument = XDocument.Load(new XmlNodeReader(datasetsNode));
                xDocument.XPathSelectElement("//f1_010_0_").Value = "Name (not your trade name)";
                xDocument.XPathSelectElement("//c1_1_0_").Value = "Report4";
                // from 'xfa:datasets' in step [1] above
                XNamespace xNamespace = "http://www.xfa.org/schema/xfa-data/1.0/";
                XName xName = xNamespace + "datasets";

                XElement root = xDocument.Descendants(xName).First();
                XElement rootClone = CloneElement(root);

                Console.Write(rootClone.ToString());

                return rootClone.ToString();
                //return root.ToString();
                // return xDocument.Descendants(xName).First().ToString();
            }
        }

        public void FillAndSaveXfa()
        {
            var filename = "f941sb.pdf";
            var readerPath = Helpers.IO.GetInputFilePath(filename);
            var outputFile = Helpers.IO.GetOutputPath(filename);

            SaveDatasetsXml(readerPath);
            var xml = EditXfaXml(readerPath);

            // 3. fill out the form
            using (var reader = new PdfReader(readerPath))
            {
                using (var stamper = new PdfStamper(
                    reader,
                    new FileStream(outputFile, FileMode.Create),
                    '\0',
                    true))
                {
                    AcroFields acroFields = stamper.AcroFields;
                    //var xmlDocument = new XmlDocument();
                    //xmlDocument.LoadXml(xml);
                    //acroFields.Xfa.FillXfaForm(xmlDocument.DocumentElement);

                    using (var sr = new StringReader(xml))
                    {
                        acroFields.Xfa.FillXfaForm(XmlReader.Create(new StringReader(xml)));
                    }
                }
            }
        }


    }
}