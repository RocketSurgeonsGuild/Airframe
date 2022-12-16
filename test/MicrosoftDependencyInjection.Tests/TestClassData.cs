using System.Collections;
using System.Collections.Generic;

namespace Rocket.Surgery.Airframe.Microsoft.Extensions.DependencyInjection.Tests
{
    public abstract class TestClassData : IEnumerable<object[]>
    {
        /// <inheritdoc/>
        public IEnumerator<object[]> GetEnumerator() => Enumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the internal enumerator.
        /// </summary>
        /// <returns>The object array enumerator.</returns>
        protected abstract IEnumerator<object[]> Enumerator();
    }
}