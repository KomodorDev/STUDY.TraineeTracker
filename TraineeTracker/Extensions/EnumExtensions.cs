using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TraineeTracker.Extensions{
    public static class EnumExtensions {
        public static string GetDisplayName(this Enum enumValue) { // this: Signals that this is an extension for enums
            return enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName()
                ?? enumValue.ToString();
        }
    }
}