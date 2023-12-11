using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace TPFive.Game
{
    /// <summary>
    /// Represents an option with no value.
    /// <exmaple>
    /// <br/>For example:
    /// <c>Optional&lt;int&gt; p = new NoValueOption();</c>
    /// </exmaple>
    /// </summary>
    public struct NoValueOption
    {
    }

    /// <summary>
    /// Serializable optional value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField]
        private bool hasValue;
        [SerializeField]
        private T value;

        public Optional(T initialValue)
        {
            hasValue = true;
            value = initialValue;
        }

        public readonly bool HasValue => hasValue;

        public readonly T Value => value;

        public static implicit operator Optional<T>(T value) => new Optional<T>(value);

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Reviewed")]
        public static implicit operator Optional<T>(NoValueOption _) => default;

        public static implicit operator T(Optional<T> value) => value.value;

        public override readonly string ToString()
        {
            return $"{value}({(HasValue ? "☑" : "☒")})";
        }
    }
}
