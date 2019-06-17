using System.Text.Json.Serialization.Tests;
using Xunit;

namespace System.Text.Json.Tests.RealWorld
{
    public class RealWorldTest : ITestClass
    {
        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Verify()
        {
            VerifySerializer();
            VerifyDocument();
            VerifyReader();
            VerifyWriter();
        }

        public virtual string PayloadAsString => throw new NotImplementedException();

        public byte[] PayloadAsUtf8Bytes => Encoding.UTF8.GetBytes(PayloadAsString);

        public virtual void VerifySerializer()
        {
            throw new NotImplementedException();
        }

        public virtual void VerifyDocument()
        {
            throw new NotImplementedException();
        }

        public virtual void VerifyReader()
        {
            throw new NotImplementedException();
        }

        public virtual void VerifyWriter()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void TestRealWorldUseCases()
        {

        }
    }
}
