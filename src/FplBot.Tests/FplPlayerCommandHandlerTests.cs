using System.Threading.Tasks;
using FplBot.ConsoleApps;
using FplBot.ConsoleApps.Clients;
using FplBot.Tests.Helpers;
using SlackConnector.Models;
using Xunit;
using Xunit.Abstractions;

namespace FplBot.Tests
{
    public class FplPlayerCommandHandlerTests
    {
        private readonly ITestOutputHelper _logger;

        public FplPlayerCommandHandlerTests(ITestOutputHelper logger)
        {
            _logger = logger;
        }
        
        [Theory]
        [InlineData("@fplbot player salah")]
        [InlineData("<@UREFQD887> player salah")]
        public async Task GetPlayerHandler(string input)
        {
            var client = CreateHandler();
            var playerData = await client.Handle(new SlackMessage
            {
                Text = input,
                ChatHub = new SlackChatHub()
            });
            
            Assert.Equal("Mohamed Salah", playerData.HandledMessage);
        }

        private static FplPlayerCommandHandler CreateHandler()
        {
            return new FplPlayerCommandHandler(new[] { new DummyPublisher() }, new FplClient());
        }

        [Theory]
        [InlineData("@fplbot player nonexistantplayer")]
        [InlineData("<@UREFQD887> player nonexistantplayer")]
        public async Task GetPlayerHandlerNonPlayer(string input)
        {
            var client = CreateHandler();
            var playerData = await client.Handle(new SlackMessage
            {
                Text = input,
                ChatHub = new SlackChatHub()
            });
            
            Assert.Equal("Not found", playerData.HandledMessage);
        }
    }
}