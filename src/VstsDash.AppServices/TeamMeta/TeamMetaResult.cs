using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VstsDash.AppServices.TeamMeta
{
    public class TeamMetaResult
    {
        public TeamMetaResult(IList<TeamMetaProject> projects)
        {
            if (projects == null) throw new ArgumentNullException(nameof(projects));

            Projects = new ReadOnlyCollection<TeamMetaProject>(projects.ToList());
        }

        public IReadOnlyCollection<TeamMetaProject> Projects { get; }
    }
}