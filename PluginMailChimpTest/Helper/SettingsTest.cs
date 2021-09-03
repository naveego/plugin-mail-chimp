using System;
using PluginMailChimp.Helper;
using Xunit;

namespace PluginMailChimpTest.Helper
{
    public class SettingsTest
    {
        [Fact]
        public void ValidateValidTest()
        {
            // setup
            var settings = new Settings
            {
            };

            // act
            settings.Validate();

            // assert
        }
    }
}