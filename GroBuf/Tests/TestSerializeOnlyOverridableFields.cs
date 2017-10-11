using GroBuf.DataMembersExtracters;
using NUnit.Framework;

namespace GroBuf.Tests
{
    [TestFixture]
    public class TestSerializeOnlyOverridableFields
    {
        [Test]
        public void AllPropertiesExtractor_GetOnlyProp()
        {
            var serializer = new Serializer(new AllPropertiesExtractor(), null, GroBufOptions.MergeOnRead);
            var expected = new ChildGetOnlyClass();
            Assert.DoesNotThrow(() => serializer.Serialize(expected));
        }

        [Test]
        public void PropertiesExtractor_GetOnlyProp()
        {
            var serializer = new Serializer(new PropertiesExtractor(), null, GroBufOptions.MergeOnRead);
            var expected = new ChildGetOnlyClass();
            Assert.DoesNotThrow(() => serializer.Serialize(expected));
        }

        [Test]
        public void AllPropertiesExtractor_WritableProp()
        {
            var serializer = new Serializer(new AllPropertiesExtractor(), null, GroBufOptions.MergeOnRead);
            var expected = new ChildWritableClass();
            Assert.DoesNotThrow(() => serializer.Serialize(expected));
        }

        [Test]
        public void PropertiesExtractor_WritableProp()
        {
            var serializer = new Serializer(new PropertiesExtractor(), null, GroBufOptions.MergeOnRead);
            var expected = new ChildWritableClass();
            Assert.DoesNotThrow(() => serializer.Serialize(expected));
        }

        private enum TestEnum1
        {
            First
        }

        private abstract class BaseGetOnlyClass
        {
            public virtual TestEnum1 States { get; }
        }

        private class ChildGetOnlyClass : BaseGetOnlyClass
        {
            public override TestEnum1 States { get; }
        }

        private abstract class BaseWritableClass
        {
            public virtual TestEnum1 States { get; }
        }

        private class ChildWritableClass : BaseGetOnlyClass
        {
            public override TestEnum1 States { get; }
        }
    }
}