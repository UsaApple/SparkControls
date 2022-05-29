using System;
using System.ComponentModel;

namespace SparkControls.Controls.Design
{
    public sealed class SparkNavigationBarSelectedPageConverter : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(String);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(String))
            {
                return ((SparkNavigateBarPanel)(value)).Name;
            }
            throw GetConvertToException(value, destinationType);
        }
    }
}