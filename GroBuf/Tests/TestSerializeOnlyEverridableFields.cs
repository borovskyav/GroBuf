using GroBuf.DataMembersExtracters;
using NUnit.Framework;

namespace GroBuf.Tests
{
    [TestFixture]
    public class TestSerializeOnlyEverridableFields
    {
        [Test]
        public void AllPropertiesExtractor()
        {
            var serializer = new Serializer(new AllPropertiesExtractor(), null, GroBufOptions.MergeOnRead);
            var expected = new ChildClass();
            Assert.DoesNotThrow(() => serializer.Serialize(expected));
        }

        [Test]
        public void PropertiesExtractor()
        {
            var serializer = new Serializer(new PropertiesExtractor(), null, GroBufOptions.MergeOnRead);
            var expected = new ChildClass();
            Assert.DoesNotThrow(() => serializer.Serialize(expected));
        }

        private enum TestEnum1
        {
            First
        }

        private abstract class BaseClass
        {
            protected virtual TestEnum1[] States { get; }
        }

        private class ChildClass : BaseClass
        {
            protected override TestEnum1[] States => states;
            private static readonly TestEnum1[] states = { TestEnum1.First };
        }
    }
}