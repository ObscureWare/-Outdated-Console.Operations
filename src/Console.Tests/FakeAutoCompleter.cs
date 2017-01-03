namespace ConsoleTests
{
    using System.Collections.Generic;
    using Obscureware.Console.Operations;

    /// <summary>
    /// Fakes auto-completion mechanics, always returning empty list.
    /// </summary>
    internal class FakeAutoCompleter : IAutoComplete
    {
        /// <inheritdoc />
        public IEnumerable<string> MatchAutoComplete(string text)
        {
            yield break;
        }
    }
}