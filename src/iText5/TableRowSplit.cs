using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.iText5
{
    public class TableRowSplit
    {
        public string OUT_FILE;
        public TableRowSplit()
        {
            OUT_FILE = Helpers.IO.GetClassOutputPath(this);
        }

        public void Go()
        {
            var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer vestibulum sollicitudin luctus. Curabitur at eros bibendum, porta risus a, luctus justo. Phasellus in libero vulputate, fermentum ante nec, mattis magna. Nunc viverra viverra sem, et pulvinar urna accumsan in. Quisque ultrices commodo mauris, et convallis magna. Duis consectetur nisi non ultrices dignissim. Aenean imperdiet consequat magna, ac ornare magna suscipit ac. Integer fermentum velit vitae porttitor vestibulum. Morbi iaculis sed massa nec ultricies. Aliquam efficitur finibus dolor, et vulputate turpis pretium vitae. In lobortis lacus diam, ut varius tellus varius sed. Integer pulvinar, massa quis feugiat pulvinar, tortor nisi bibendum libero, eu molestie est sapien quis odio. Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            var table = new PdfPTable(3) { WidthPercentage = 76 };
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.KeepTogether = true;
            table.SplitRows = true;

            using (FileStream stream = new FileStream(
                OUT_FILE,
                FileMode.Create,
                FileAccess.Write))
            {
                using (var document = new Document())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();
                    for (int i = 0; i < 25; ++i)
                    {
                        table.AddCell(new PdfPCell(new Phrase(i.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(i.ToString())));
                        table.AddCell(new PdfPCell(new Phrase(text)));
                    }
                    document.Add(table);
                }
            }
        }
    }
}