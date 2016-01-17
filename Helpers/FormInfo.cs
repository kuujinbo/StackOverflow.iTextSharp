using System.Text;
using iTextSharp.text.pdf;

namespace kuujinbo.StackOverflow.iTextSharp.Helpers
{
    public class FormInfo
    {
        private static string _formFieldTypeName(int formFieldType)
        {
            switch (formFieldType)
            {
                case AcroFields.FIELD_TYPE_CHECKBOX:
                    return "Checkbox";
                case AcroFields.FIELD_TYPE_COMBO:
                    return "Combobox";
                case AcroFields.FIELD_TYPE_LIST:
                    return "List";
                case AcroFields.FIELD_TYPE_NONE:
                    return "None";
                case AcroFields.FIELD_TYPE_PUSHBUTTON:
                    return "Pushbutton";
                case AcroFields.FIELD_TYPE_RADIOBUTTON:
                    return "Radiobutton";
                case AcroFields.FIELD_TYPE_SIGNATURE:
                    return "Signature";
                case AcroFields.FIELD_TYPE_TEXT:
                    return "Text";
                default:
                    return "UNKNOWN";
            }
        }

        public static string Dump(string fileName)
        {
            string section = Misc.RepeatChar('=', 38);
            StringBuilder dump = new StringBuilder();

            using (var reader = Helpers.IO.GetInputReader(fileName))
            {
                var acroFields = reader.AcroFields;

                foreach (string name in acroFields.Fields.Keys)
                {
                    var formFieldType = acroFields.GetFieldType(name);
                    var p = acroFields.GetFieldPositions(name)[0];

                    dump.AppendLine(string.Format(@"Name: ""{0}""", name));
                    dump.AppendLine(string.Format(@"Value: ""{0}""", acroFields.GetField(name)));
                    dump.AppendLine(string.Format(
                        "\tFieldType: {0}", _formFieldTypeName(formFieldType)
                    ));
                    if (formFieldType == AcroFields.FIELD_TYPE_CHECKBOX
                        || formFieldType == AcroFields.FIELD_TYPE_RADIOBUTTON)
                    {
                        var app = acroFields.GetAppearanceStates(name);
                        dump.AppendLine(string.Format(
                            "\tpossible values: [{0}]", string.Join(",", app)
                        ));
                    }
                    dump.AppendLine(string.Format("\tTop: {0}", p.position.Top));
                    dump.AppendLine(string.Format("\tLeft: {0}", p.position.Left));
                    dump.AppendLine(string.Format("\tWidth: {0}", p.position.Width));
                    dump.AppendLine(string.Format("\tHeight: {0}", p.position.Height));
                    dump.AppendLine(string.Format("\tPageNumber: {0}", p.page));
                }
                return dump.ToString();
            }
        }


    }
}