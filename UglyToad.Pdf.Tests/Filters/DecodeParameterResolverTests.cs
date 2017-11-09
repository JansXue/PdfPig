﻿namespace UglyToad.Pdf.Tests.Filters
{
    using System;
    using ContentStream;
    using Parser.Parts;
    using Pdf.Cos;
    using Pdf.Filters;
    using Xunit;

    public class DecodeParameterResolverTests
    {
        private readonly DecodeParameterResolver resolver=  new DecodeParameterResolver(new TestingLog());

        [Fact]
        public void NullDictionary_Throws()
        {
            Action action = () => resolver.GetFilterParameters(null, 0);

            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void NegativeIndex_Throws()
        {
            Action action = () => resolver.GetFilterParameters(new ContentStreamDictionary(), -1);

            Assert.Throws<ArgumentOutOfRangeException>(action);
        }

        [Fact]
        public void EmptyDictionary_ReturnsEmptyDictionary()
        {
            var result = resolver.GetFilterParameters(new ContentStreamDictionary(), 0);

            Assert.Empty(result);
        }
    }
}
