using System;
using BccPay.Core.Shared.Converters;
using Xunit;

namespace BccPay.Core.Test
{
    public class AddressConventerTests
    {
        [Fact]
        public void AddressConventerNorTest()
        {
            var norwayShortResult1 = AddressConverter.ConvertCountry("NOR");
            Assert.Equal("NOR", norwayShortResult1);
        }
        [Fact]
        public void AddressConventerNoTest()
        {
            var norwayShortResult2 = AddressConverter.ConvertCountry("NO");
            Assert.Equal("NOR", norwayShortResult2);
        }
        [Fact]
        public void AddressConventerNorwayTest()
        {
            var norwayShortResult3 = AddressConverter.ConvertCountry("Norway");
            Assert.Equal("NOR", norwayShortResult3);
        }
    }
}