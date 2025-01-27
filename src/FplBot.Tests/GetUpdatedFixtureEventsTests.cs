using Fpl.Client.Models;
using Fpl.EventPublishers.Models.Mappers;
using FplBot.Messaging.Contracts.Events.v1;

namespace FplBot.Tests;

public class GetUpdatedFixtureEventsTests
{
    [Fact]
    public static void When_NoEntries_ReturnsEmptyList()
    {
        AssertEmpty(null,null);
        AssertEmpty(new List<Fixture>(), null);
        AssertEmpty(null, new List<Fixture>());
    }

    [Fact]
    public static void When_NewAwayGoal_ReturnsAwayTeamGoalEvent()
    {
        var current = new List<Fixture>
        {
            TestBuilder.NoGoals(fixtureCode:1)
        };

        var latest = new List<Fixture>
        {
            TestBuilder.AwayTeamGoal(fixtureCode:1, goals: 1, minutes: 72)
        };

        var events = LiveEventsExtractor.GetUpdatedFixtureEvents(latest, current, new List<Player> { TestBuilder.Player()}, new List<Team> { TestBuilder.HomeTeam(), TestBuilder.AwayTeam()});
        var awayGoalEvent = events.First();
        Assert.Equal(123, awayGoalEvent.StatMap[StatType.GoalsScored].First().Player.Id);
        Assert.Equal(TeamType.Away, awayGoalEvent.StatMap[StatType.GoalsScored].First().Team);
        Assert.Equal(72, awayGoalEvent.FixtureScore.Minutes);
    }

    private static void AssertEmpty(ICollection<Fixture> latest, ICollection<Fixture> current)
    {
        var events = LiveEventsExtractor.GetUpdatedFixtureEvents(latest, current, new List<Player>(), new List<Team>());
        Assert.Empty(events);
    }
}
