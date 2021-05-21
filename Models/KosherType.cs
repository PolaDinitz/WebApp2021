using System.ComponentModel;

namespace WebApp2021.Models
{
    public enum KosherType
    {
        [Description("Not Kosher")]
        Not_Kosher,

        [Description("Meat")]
        Meat,

        [Description("Dairy")]
        Dairy,

        [Description("Parve")]
        Parve
    }
}
