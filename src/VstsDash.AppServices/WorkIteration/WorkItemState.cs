using System;

namespace VstsDash.AppServices.WorkIteration
{
    public static class WorkItemState
    {
        public const string Commited = "Committed";

        public const string Done = "Done";

        public const string InProgress = "In Progress";

        public const string ToDo = "To Do";

        public static bool IsCommited(string state)
        {
            return Commited.Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDone(string state)
        {
            return Done.Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsInProgress(string state)
        {
            return InProgress.Equals(state, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsToDo(string state)
        {
            return ToDo.Equals(state, StringComparison.OrdinalIgnoreCase);
        }
    }
}