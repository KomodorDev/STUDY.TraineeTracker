using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TraineeTracker.Extensions {

    /// <summary>
    /// Provides extension methods for working with enumeration types.
    /// </summary>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
    public static class EnumExtensions {

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves the value of the <see cref="DisplayAttribute.Name"/> property 
        /// for a given enum value. If no <see cref="DisplayAttribute"/> is applied,
        /// the enum's name is returned as a fallback.
        /// </summary>
        /// <param name="enumValue">The enum value whose display name should be retrieved.</param>
        /// <returns>
        /// The string specified by the <see cref="DisplayAttribute.Name"/>; 
        /// otherwise, the enum value’s name as a string.
        /// </returns>
        /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
        public static string GetDisplayName(this Enum enumValue) { // this: Signals that this is an extension for enums
            return enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName()
                ?? enumValue.ToString();
        }

        // ------------------------------------------------------
    }
}