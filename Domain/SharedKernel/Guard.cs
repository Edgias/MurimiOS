using System;

namespace Edgias.MurimiOS.Domain.SharedKernel
{
    public static class Guard
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="argumentValue" /> is null or default value.
        /// </summary>
        /// <param name="argumentValue">Value of parameter to check for null or default values.</param>
        /// <param name="argumentName">Name of parameter</param>
        public static void AgainstNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null || argumentValue == default)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException" /> if <paramref name="argumentValue" /> is null or empty.
        /// </summary>
        /// <param name="argumentValue">Value of parameter to check for null or empty values.</param>
        /// <param name="argumentName">Name of parameter</param>
        public static void AgainstNullOrEmpty(string argumentValue, string argumentName)
        {
            if (string.IsNullOrEmpty(argumentValue))
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="argumentValue" /> is zero.
        /// </summary>
        /// <param name="argumentValue">Value of parameter to check for zero value.</param>
        /// <param name="argumentName">Name of parameter</param>
        public static void AgainstZero(int argumentValue, string argumentName)
        {
            if (argumentValue == 0)
            {
                throw new ArgumentException($"Argument '{argumentName}' cannot be zero");
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException" /> if <paramref name="argumentValue" /> is zero.
        /// </summary>
        /// <param name="argumentValue">Value of parameter to check for zero value.</param>
        /// <param name="argumentName">Name of parameter</param>
        public static void AgainstZero(decimal argumentValue, string argumentName)
        {
            if (argumentValue == 0)
            {
                throw new ArgumentException($"Argument '{argumentName}' cannot be zero");
            }
        }

    }
}
