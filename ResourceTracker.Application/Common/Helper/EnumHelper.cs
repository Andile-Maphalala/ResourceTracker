using ResourceTracker.Application.Models.Enums;
using System.ComponentModel;

namespace ResourceTracker.Application.Common.Helper
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static ImageUploadTypeEnum? GetEnumValue(int? value)
        {
            if (value == null)
                return null;

            if (Enum.IsDefined(typeof(ImageUploadTypeEnum), value))
            {
                return (ImageUploadTypeEnum)value;
            }
            else
            {
                return null;
            }
        }
    }
}
