using System;
using System.Threading.Tasks;
using FplBot.ConsoleApps.Clients;
using FplBot.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace FplBot.Tests
{
    public class FplClientTests
    {
        private readonly ITestOutputHelper _logger;
        private IFplClient _client;

        public FplClientTests(ITestOutputHelper logger)
        {
            _logger = logger;
            _client = Factory.CreateClient(logger);
        }
        
        [Fact]
        public async Task GetStandings()
        {
            var standings = await _client.GetStandings("579157");
            Assert.StartsWith(":star:", standings);
        }

        [Theory]
        [InlineData("salah")]
        [InlineData("mané")]
        [InlineData("firmino")]
        [InlineData("henderson")]
        [InlineData("wijnaldum")]
        [InlineData("tavares")]
        [InlineData("robertson")]
        [InlineData("van dijk")]
        [InlineData("matip")]
        [InlineData("trent")]
        [InlineData("alisson")]
        public async Task GetPlayer(string input)
        {
            var playerData = await _client.GetAllFplDataForPlayer(input);
            Assert.Contains(input, playerData, StringComparison.InvariantCultureIgnoreCase);
        }
        
        // Check 
        [Fact]
        public async Task CacheTest()
        {
            await _client.GetStandings("579157");
            await _client.GetStandings("579157");
        }
    }
}