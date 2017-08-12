using System;

namespace VstsDash.AppServices.WorkTeamBoard
{
    public class Point
    {
        public Point(
            TeamMemberPointType type,
            double value,
            string id,
            string description,
            DateTimeOffset earnedAt,
            bool hasBonus = false)
        {
            Type = type;
            Value = !hasBonus ? value : (value + 1);
            Id = id;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            EarnedAt = earnedAt;
            HasBonus = hasBonus;
        }

        public string Description { get; }

        public DateTimeOffset EarnedAt { get; }

        public bool HasBonus { get; }

        public string Id { get; }

        public TeamMemberPointType Type { get; }

        public double Value { get; }
    }
}