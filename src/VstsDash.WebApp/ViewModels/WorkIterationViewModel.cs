using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VstsDash.AppServices.WorkIteration;

namespace VstsDash.WebApp.ViewModels
{
    public class WorkIterationViewModel
    {
        public WorkIterationViewModel(string projectId, string teamId, string iterationId)
        {
            ProjectId = projectId;
            TeamId = teamId;
            IterationId = iterationId;
        }

        public IReadOnlyCollection<string> AreaPaths { get; private set; }

        public string IterationId { get; }

        public string ProjectId { get; }

        public Iteration Result { get; private set; }

        public string TeamId { get; }

        //public double TotalEffort => this.Result.Items.Sum(x => x.TotalEffort);

        //public double TotalRemainingWork => this.Result.Items.Sum(x => x.TotalRemainingWork);

        public async Task Update(WorkIterationAppService workIterationAppService)
        {
            Result = await workIterationAppService.GetWorkIteration(ProjectId, TeamId, IterationId);

            AreaPaths = Result.Items.Select(x => x.AreaPath).Distinct().OrderBy(x => x).ToList();
        }
    }
}