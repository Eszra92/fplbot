using FplBot.Messaging.Contracts.Events.v1;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;

namespace FplBot.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController
{
    private readonly IMessageSession _session;
    private readonly IWebHostEnvironment _env;

    public DebugController(IMessageSession session, IWebHostEnvironment env)
    {
        _session = session;
        _env = env;
    }

    [HttpGet("goal")]
    public async Task<IActionResult> Goal(bool removed = false)
    {
        if (!_env.IsProduction())
        {
            var message = FixtureEvents(StatType.GoalsScored, removed);
            await _session.Publish(message);
            // await _session.Publish(FixtureEvents(StatType.Assists, removed));
            // await _session.Publish(FixtureEvents(StatType.OwnGoals, removed));
            // await _session.Publish(FixtureEvents(StatType.PenaltiesMissed, removed));
            // await _session.Publish(FixtureEvents(StatType.PenaltiesSaved, removed));
            // await _session.Publish(FixtureEvents(StatType.RedCards, removed));
            // await _session.Publish(FixtureEvents(StatType.YellowCards, removed));
            // await _session.Publish(FixtureEvents(StatType.Saves, removed));
            // await _session.Publish(FixtureEvents(StatType.Bonus, removed));
            return new AcceptedResult("", message);
        }

        return new UnauthorizedResult();

    }

    [HttpGet("removedfixture")]
    public async Task<IActionResult> RemovedFixture(bool removed = false)
    {
        if (!_env.IsProduction())
        {
            var removedFixture = new RemovedFixture(1, new(1, "Arsenal", "ARS"), new(2, "Chelsea", "CHE"));
            var message = new FixtureRemovedFromGameweek(1337, removedFixture);
            await _session.Publish(message);
            return new AcceptedResult("", message);
        }
        return new UnauthorizedResult();
    }

    private static FixtureEventsOccured FixtureEvents(StatType type, bool isRemoved)
    {
        List<FixtureEvents> fixtureEventsList = new();
        FixtureTeam home = new(1, "HOM", "HomeTeam");
        FixtureTeam away = new(2, "AWA", "Away");
        FixtureScore fixtureScore = new(home, away, 35, 0, 1);
        Dictionary<StatType,List<PlayerEvent>> statMap = new();
        PlayerDetails playerDetails1 = new(1, "Testerson");
        PlayerDetails playerDetails2 = new(2, Environment.MachineName);
        TeamType teamDetails = TeamType.Home;

        List<PlayerEvent> playerEvents = new()
        {
            new PlayerEvent(playerDetails1, teamDetails, IsRemoved: isRemoved),
            new PlayerEvent(playerDetails2, teamDetails, false),
        };

        statMap.Add(type, playerEvents);

        fixtureEventsList.Add(new FixtureEvents(fixtureScore, statMap));
        return new FixtureEventsOccured(fixtureEventsList);
    }
}
