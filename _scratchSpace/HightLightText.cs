using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using kuujinbo.StackOverflow.iTextSharp._scratchSpace.FoundAtSO;

namespace kuujinbo.StackOverflow.iTextSharp._scratchSpace
{
    public class HightLightText
    {
        public void Go()
        {
            var filename = "samplechapter2.pdf";
            string[] highlightText = new string[] { "PDF", "Chunk" };
            var readerPath = Helpers.IO.GetInputFilePath(filename);
            var outputFile = Helpers.IO.GetClassOutputPath(this);

            highlightPDFAnnotation(readerPath, outputFile, highlightText);
        }


        // private void highlightPDFAnnotation(string readerPath, string outputFile, int pageno, string[] highlightText)
        private void highlightPDFAnnotation(string readerPath, string outputFile, string[] highlightText)
        {
            PdfReader reader = new PdfReader(readerPath);
            PdfContentByte canvas;
            using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (PdfStamper stamper = new PdfStamper(reader, fs))
                {

                    int pageCount = reader.NumberOfPages;
                    for (int pageno = 1; pageno <= pageCount; pageno++)
                    {

                        var strategy = new HighLightTextLocation();
                        strategy.UndercontentHorizontalScaling = 100;

                        string currentText = PdfTextExtractor.GetTextFromPage(reader, pageno, strategy);
                        for (int i = 0; i < highlightText.Length; i++)
                        {
                            List<Rectangle> MatchesFound = strategy.GetTextLocations(highlightText[i].Trim(), StringComparison.CurrentCultureIgnoreCase);
                            foreach (Rectangle rect in MatchesFound)
                            {
                                float[] quad = { rect.Left - 3.0f, rect.Bottom, rect.Right, rect.Bottom, rect.Left - 3.0f, rect.Top + 1.0f, rect.Right, rect.Top + 1.0f };
                                //Create our hightlight
                                PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);
                                //Set the color
                                highlight.Color = BaseColor.YELLOW;

                                PdfAppearance appearance = PdfAppearance.CreateAppearance(stamper.Writer, rect.Width, rect.Height);
                                PdfGState state = new PdfGState();
                                state.BlendMode = new PdfName("Multiply");
                                appearance.SetGState(state);
                                appearance.Rectangle(0, 0, rect.Width, rect.Height);
                                appearance.SetColorFill(BaseColor.YELLOW);
                                appearance.Fill();

                                highlight.SetAppearance(PdfAnnotation.APPEARANCE_NORMAL, appearance);

                                //Add the annotation
                                stamper.AddAnnotation(highlight, pageno);
                            }
                        }
                    }
                }
            }
            reader.Close();
        }
    }
}