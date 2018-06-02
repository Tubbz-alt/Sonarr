using System;
using System.Collections.Generic;
using System.Linq;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.Core.Tv;
using Sonarr.Api.V3.Episodes;
using Sonarr.Http.Extensions;
using Sonarr.SignalR;

namespace Sonarr.Api.V3.Calendar
{
    public class CalendarModule : EpisodeModuleWithSignalR
    {
        public CalendarModule(IEpisodeService episodeService,
                              ISeriesService seriesService,
                              IUpgradableSpecification ugradableSpecification,
                              IBroadcastSignalRMessage signalRBroadcaster)
            : base(episodeService, seriesService, ugradableSpecification, signalRBroadcaster, "calendar")
        {
            GetResourceAll = GetCalendar;
        }

        private List<EpisodeResource> GetCalendar()
        {
            var start = DateTime.Today;
            var end = DateTime.Today.AddDays(2);
            var includeUnmonitored = Request.GetBooleanQueryParameter("unmonitored");
            var includeSeries = Request.GetBooleanQueryParameter("includeSeries");
            var includeEpisodeFile = Request.GetBooleanQueryParameter("includeEpisodeFile");

            var queryStart = Request.Query.Start;
            var queryEnd = Request.Query.End;

            if (queryStart.HasValue) start = DateTime.Parse(queryStart.Value);
            if (queryEnd.HasValue) end = DateTime.Parse(queryEnd.Value);

            var resources = MapToResource(_episodeService.EpisodesBetweenDates(start, end, includeUnmonitored), includeSeries, includeEpisodeFile);

            return resources.OrderBy(e => e.AirDateUtc).ToList();
        }
    }
}