using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VstsDash.AppServices.WorkLeaderboard
{
    public class Score
    {
        public Score(IEnumerable<Point> points)
        {
            if (points == null) throw new ArgumentNullException(nameof(points));

            var pointList = points.ToList();

            Goals = GetPointsCollection(pointList.Where(x => x.Type == TeamMemberPointType.Goal));
            Assists = GetPointsCollection(pointList.Where(x => x.Type == TeamMemberPointType.Assist));
            Points = GetPointsCollection(pointList);
        }

        public static Score Empty => new Score(Enumerable.Empty<Point>());

        public IReadOnlyCollection<Point> Assists { get; }

        public IReadOnlyCollection<Point> Goals { get; }

        public IReadOnlyCollection<Point> Points { get; }

        private static IReadOnlyCollection<Point> GetPointsCollection(IEnumerable<Point> points)
        {
            var ordered = points.OrderByDescending(x => x.EarnedAt)
                .ThenByDescending(x => x.Value)
                .ThenBy(x => x.Description)
                .ToList();

            return new ReadOnlyCollection<Point>(ordered);
        }
    }
}