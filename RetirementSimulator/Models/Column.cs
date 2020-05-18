namespace RetirementSimulator.Models
{
    using DevExpress.Xpf.Grid;

    public class Column
    {
        public Column(string field, string header, ColumnFieldTypes type, string mask = null, FixedStyle fixedStyle = FixedStyle.None)
        {
            this.FieldName = field;
            this.Header = header;
            this.FieldType = type;
            this.Mask = mask;
            this.FixedStyle = fixedStyle;
        }

        public string FieldName { get; set; }

        public string Header { get; set; }

        public string Mask { get; set; }

        public ColumnFieldTypes FieldType { get; set; }

        public FixedStyle FixedStyle { get; set; }
    }
}