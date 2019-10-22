using System;
using System.ComponentModel;

namespace Natu.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string StringValue(this Enum value, bool addSpaces = true) {
            string output;
            var type = value.GetType();
            var fi = type.GetField(value.ToString());
            var attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            if (attrs != null
                && attrs.Length > 0) {
                output = attrs[0].Value;
            }
            else {
                output = value.ToString();
                if (addSpaces) {
                    output = output.AddSpaces();
                }
            }
            return output;
        }

        public static string Description(this Enum value)
        {
            var descriptionAttribute = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptionAttribute.Length > 0 ? descriptionAttribute[0].Description : value.ToString();
        }
    }

    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
