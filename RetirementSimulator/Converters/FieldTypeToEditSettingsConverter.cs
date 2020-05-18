namespace RetirementSimulator.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    using DevExpress.Xpf.Editors;
    using DevExpress.Xpf.Editors.Settings;

    using RetirementSimulator.Models;

    public class FieldTypeToEditSettingsConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var column = (Column)value;

            object editor;

            switch (column.FieldType)
            {
                case ColumnFieldTypes.String:
                    editor = new TextEditSettings();
                    break;

                case ColumnFieldTypes.Int:
                case ColumnFieldTypes.Currency:
                    editor = new TextEditSettings { Mask = column.Mask, MaskUseAsDisplayFormat = true,  MaskType = MaskType.Numeric };
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return editor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}