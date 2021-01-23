﻿using Fpl.Search.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fpl.Search.Searching
{
    public class SearchClient : ISearchClient
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<SearchClient> _logger;
        private readonly SearchOptions _options;

        public SearchClient(IElasticClient elasticClient, ILogger<SearchClient> logger, IOptions<SearchOptions> options)
        {
            _elasticClient = elasticClient;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<IReadOnlyCollection<EntryItem>> SearchForEntry(string query, int maxHits)
        {
            var response = await _elasticClient.SearchAsync<EntryItem>(x => x
                .Index(_options.EntriesIndex)
                .From(0)
                .Size(maxHits)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f.Field(y => y.RealName, 1.5).Field(y => y.TeamName))
                        .Query(query))));

            return response.Documents;
        }

        public async Task<IReadOnlyCollection<LeagueItem>> SearchForLeague(string query, int maxHits, string countryToBoost = null)
        {
            var response = await _elasticClient.SearchAsync<LeagueItem>(x => x
                .Index(_options.LeaguesIndex)
                .From(0)
                .Size(maxHits)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f.Fields(y => y.Name))
                        .Query(query))));

            if (!string.IsNullOrEmpty(countryToBoost))
            {
                var hits = response.Hits.OrderByDescending(h => h.Score)
                    .ThenByDescending(h => h.Source.AdminCountry == countryToBoost ? 1 : 0);
                return hits.Select(h => h.Source).ToArray();
            }

            return response.Documents;
        }
    }

    public interface ISearchClient
    {
        Task<IReadOnlyCollection<EntryItem>> SearchForEntry(string query, int maxHits);
        Task<IReadOnlyCollection<LeagueItem>> SearchForLeague(string query, int maxHits, string countryToBoost = null);
    }
}
