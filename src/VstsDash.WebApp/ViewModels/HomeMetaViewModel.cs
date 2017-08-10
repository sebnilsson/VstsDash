using System;
using System.Collections.Generic;

namespace VstsDash.WebApp.ViewModels
{
    public class HomeMetaViewModel
    {
        public IReadOnlyCollection<Project> Projects { get; set; }

        public class Project
        {
            public string Description { get; set; }

            public string Id { get; set; }

            public string Name { get; set; }

            public IReadOnlyCollection<Query> Queries { get; set; }

            public IReadOnlyCollection<Repository> Repositories { get; set; }

            public string State { get; set; }

            public IReadOnlyCollection<Team> Teams { get; set; }

            public string Url { get; set; }

            public class Query
            {
                public IReadOnlyCollection<Query> Children { get; set; }

                public DateTimeOffset CreatedDate { get; set; }

                public bool HasChildren { get; set; }

                public Guid Id { get; set; }

                public bool IsFolder { get; set; }

                public bool IsPublic { get; set; }

                public DateTimeOffset LastModifiedDate { get; set; }

                public string Name { get; set; }

                public string Path { get; set; }

                public string QueryType { get; set; }

                public string Url { get; set; }

                public string Wiql { get; set; }
            }

            public class Repository
            {
                public Guid Id { get; set; }

                public string Name { get; set; }

                public string RemoteUrl { get; set; }

                public string Url { get; set; }
            }

            public class Team
            {
                public string BacklogIterationId { get; set; }

                public string BacklogIterationName { get; set; }

                public IReadOnlyCollection<Board> Boards { get; set; }

                public string DefaultIterationId { get; set; }

                public string DefaultIterationName { get; set; }

                public string Description { get; set; }

                public string Id { get; set; }

                public IReadOnlyCollection<Iteration> Iterations { get; set; }

                public IReadOnlyCollection<Member> Members { get; set; }

                public string Name { get; set; }

                public string Url { get; set; }

                public class Board
                {
                    public Guid Id { get; set; }

                    public string Name { get; set; }

                    public string Url { get; set; }
                }

                public class Iteration
                {
                    public IReadOnlyCollection<Capacity> Capacities { get; set; }

                    public DateTime? FinishDate { get; set; }

                    public string Id { get; set; }

                    public string Name { get; set; }

                    public DateTime? StartDate { get; set; }

                    public IReadOnlyCollection<DayOff> TeamDaysOff { get; set; }

                    public string Url { get; set; }

                    public class Capacity
                    {
                        public IReadOnlyCollection<Activity> Activities { get; set; }

                        public IReadOnlyCollection<DayOff> DaysOff { get; set; }

                        public class Activity
                        {
                            public double CapacityPerDay { get; set; }

                            public string Name { get; set; }
                        }
                    }

                    public class DayOff
                    {
                        public DateTime End { get; set; }

                        public DateTime Start { get; set; }
                    }
                }

                public class Member
                {
                    public string DisplayName { get; set; }

                    public string Id { get; set; }

                    public string ImageUrl { get; set; }

                    public string UniqueName { get; set; }

                    public string Url { get; set; }
                }
            }
        }
    }
}