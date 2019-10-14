using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using Vue.Net.Javascript;
using Xunit;

namespace Vue.Net.Tests.Javascript
{
    public class TestBaseType { }

    public class TestSubType : TestBaseType { }

    public class JavascriptConverterTest
    {
        private Mock<JavascriptConverter<TestBaseType>> _mockConverter;

        public JavascriptConverterTest()
        {
            _mockConverter = new Mock<JavascriptConverter<TestBaseType>>() { CallBase = true };
        }

        [Fact]
        public void TestCanConvertTrueWhenType()
        {
            Assert.True(_mockConverter.Object.CanConvert(typeof(TestBaseType)));
        }

        [Fact]
        public void TestCanConvertFalseWhenNotType()
        {
            Assert.False(_mockConverter.Object.CanConvert(typeof(string)));
        }

        [Fact]
        public void TestCanConvertTrueWhenSubClassType()
        {
            Assert.True(_mockConverter.Object.CanConvert(typeof(TestSubType)));
        }

        [Fact]
        public void TestReadJsonThrows()
        {
            Assert.Throws<NotSupportedException>(() => _mockConverter.Object.ReadJson(default, default, default, default));
        }

        [Fact]
        public void TestWriteJsonThrowsIfNotJavascriptWriter()
        {
            Assert.Throws<ArgumentException>(() => _mockConverter.Object.WriteJson(
                new JsonTextWriter(TextWriter.Null),
                new TestBaseType(),
                JsonSerializer.CreateDefault()));
        }

        [Fact]
        public void TestWriteJsonThrowsIfNotCorrectValueType()
        {
            Assert.Throws<ArgumentException>(() => _mockConverter.Object.WriteJson(
                new OldJavascriptTextWriter(TextWriter.Null),
                "Not A TestBaseType",
                JsonSerializer.CreateDefault()));
        }

        [Fact]
        public void TestWriteJsonCallsWriteJavascript()
        {
            var writer = new OldJavascriptTextWriter(TextWriter.Null);
            var value = new TestBaseType();
            var serializer = JsonSerializer.CreateDefault();

            _mockConverter.Setup(converter => converter.WriteJavascript(writer, value, serializer)).Verifiable();
            _mockConverter.Object.WriteJson(writer, value, serializer);
            _mockConverter.Verify();
        }
    }
}
