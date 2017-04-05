﻿using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

// http://stackoverflow.com/questions/43201602
namespace kuujinbo.StackOverflow.iTextSharp.iText5
{

    public class LowerRightImage : IPdfPCellEvent
    {
        public Image Image { get; set; }

        public void CellLayout(
            PdfPCell cell,
            Rectangle position,
            PdfContentByte[] canvases)
        {
            if (Image == null) throw new InvalidOperationException("image is null");

            PdfContentByte canvas = canvases[PdfPTable.TEXTCANVAS];
            Image.SetAbsolutePosition(
                position.Right - Image.ScaledWidth - cell.PaddingRight,
                position.Bottom + cell.PaddingBottom
            );
            canvas.AddImage(Image);
        }
    }

    public class CellTextAndImageAlign
    {
        public void Go()
        {
            var outputFile = Helpers.IO.GetClassOutputPath(this);
            var imagePath = Helpers.IO.GetInputFilePath("100x100.jpg");


            using (var stream = new MemoryStream())
            {
                using (var document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    var table = new PdfPTable(2)
                    {
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        TotalWidth = 400f,
                        LockedWidth = true
                    };

                    var image = Image.GetInstance(imagePath);
                    image.ScaleAbsolute(40, 40);
                    var cellEvent = new LowerRightImage() { Image = image };

                    var testString =
            @"first name: {0}
last name: {0}
ID no: {0}";
                    for (int i = 0; i < 2; ++i)
                    {
                        var cell = new PdfPCell()
                        {
                            FixedHeight = 140f,
                            PaddingLeft = 30f,
                            PaddingRight = 10f,
                            PaddingTop = 20f,
                            PaddingBottom = 5f
                        };
                        cell.CellEvent = cellEvent;

                        var p = new Paragraph(string.Format(testString, i))
                        {
                            Alignment = Element.ALIGN_TOP | Element.ALIGN_LEFT
                        };
                        cell.AddElement(p);
                        table.AddCell(cell);
                    }
                    document.Add(table);
                }
                File.WriteAllBytes(outputFile, stream.ToArray());
            }


        }

    }
}