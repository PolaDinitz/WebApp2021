using System;
using EnumsNET;

namespace WebApp2021.Utils
{
    public static class EnumExtensions
    {
        // Gets an enum and returns its description
        public static string GetDescription(this Enum GenericEnum)
        {
            return Enums.AsString(GenericEnum.GetType(), GenericEnum, EnumFormat.Description);
        }
    }
}