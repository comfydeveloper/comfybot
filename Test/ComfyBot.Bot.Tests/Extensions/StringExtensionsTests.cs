﻿namespace ComfyBot.Bot.Tests.Extensions
{
    using System.Reflection.Metadata;

    using ComfyBot.Bot.Extensions;

    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("{{parameter1}}", 1, true)]
        [TestCase("{{parameter2}}", 1, false)]
        [TestCase("{{parameter1}}", 2, true)]
        [TestCase("{{parameter3}} {{parameter1}}", 2, false)]
        public void CanHandleParametersShouldReturnCorrectResult(string reply, int parameterCount, bool expected)
        {
            bool result = reply.CanHandleParameters(parameterCount);

            Assert.AreEqual(expected, result);
        }
    }
}