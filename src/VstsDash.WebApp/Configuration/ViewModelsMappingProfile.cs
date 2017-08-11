using System;
using System.Linq;
using Autofac;
using AutoMapper;
using VstsDash.AppServices.TeamMeta;
using VstsDash.AppServices.WorkActivity;
using VstsDash.AppServices.WorkIteration;
using VstsDash.AppServices.WorkTeamBoard;
using VstsDash.WebApp.ViewModels;

namespace VstsDash.WebApp.Configuration
{
    public class ViewModelsMappingProfile : Profile
    {
        public ViewModelsMappingProfile(IComponentContext context)
        {
            MapHomeMetaViewModel();

            MapWorkActivityViewModel();

            MapWorkTeamBoardViewModel();
        }

        public override string ProfileName => nameof(ViewModelsMappingProfile);

        private void MapHomeMetaViewModel()
        {
            CreateMap<TeamMetaResult, HomeMetaViewModel>()
                .ForMember(
                    dest => dest.Projects,
                    opt => opt.MapFrom(src => src.Projects.OrderByDescending(x => x.Teams.Count)));
            CreateMap<TeamMetaProject, HomeMetaViewModel.Project>()
                .ForMember(
                    dest => dest.Teams,
                    opt => opt.MapFrom(src => src.Teams.OrderByDescending(x => x.Members.Count)));
            CreateMap<TeamMetaProject.Query, HomeMetaViewModel.Project.Query>();
            CreateMap<TeamMetaProject.Repository, HomeMetaViewModel.Project.Repository>();
            CreateMap<TeamMetaTeam, HomeMetaViewModel.Project.Team>()
                .ForMember(
                    dest => dest.Iterations,
                    opt => opt.MapFrom(
                        src => src.Iterations.OrderByDescending(x => x.StartDate).ThenByDescending(x => x.FinishDate)))
                .ForMember(
                    dest => dest.Members,
                    opt => opt.MapFrom(src => src.Members.OrderBy(x => x.DisplayName).ThenBy(x => x.UniqueName)));
            CreateMap<TeamMetaTeam.Board, HomeMetaViewModel.Project.Team.Board>();
            CreateMap<TeamMetaIteration, HomeMetaViewModel.Project.Team.Iteration>();
            CreateMap<TeamMetaIteration.Capacity, HomeMetaViewModel.Project.Team.Iteration.Capacity>();
            CreateMap<TeamMetaIteration.Capacity.Activity, HomeMetaViewModel.Project.Team.Iteration.Capacity.Activity
            >();
            CreateMap<TeamMetaIteration.DayOff, HomeMetaViewModel.Project.Team.Iteration.DayOff>();
            CreateMap<TeamMetaMember, HomeMetaViewModel.Project.Team.Member>();
        }

        private void MapWorkActivityViewModel()
        {
            CreateMap<Activity, WorkActivityViewModel>()
                .ForMember(
                    dest => dest.Authors,
                    opt => opt.MapFrom(
                        src => src.Authors.Where(ac => ac.Author.MemberId != Guid.Empty)
                            .OrderByDescending(
                                r => r.CommitCount / (double)src.AuthorsCommitCountSum + r.CommitsTotalChangeCountSum
                                     / (double)src.AuthorsCommitsTotalChangeCountSum)))
                .ForMember(
                    dest => dest.Commits,
                    opt => opt.MapFrom(src => src.Commits.OrderByDescending(c => c.Commit.AuthorDate)))
                .ForMember(
                    dest => dest.Repos,
                    opt => opt.MapFrom(
                        src => src.Repos.OrderByDescending(r => r.AuthorCommits.Sum(ac => ac.Commits.Count))));
            CreateMap<Author, WorkActivityViewModel.Author>();
            CreateMap<TeamCapacity, WorkActivityViewModel.ActivityTeamCapacity>();
            CreateMap<TeamMemberCapacity, WorkActivityViewModel.ActivityTeamCapacity.ActivityTeamMemberCapacity>();
            CreateMap<Commit, WorkActivityViewModel.Commit>();
            CreateMap<CommitInfo, WorkActivityViewModel.CommitInfo>();
            CreateMap<Repo, WorkActivityViewModel.Repository>();

            CreateMap<AuthorCommits, WorkActivityViewModel.AuthorCommits>()
                .ForMember(
                    dest => dest.Commits,
                    opt => opt.MapFrom(src => src.Commits.OrderByDescending(c => c.Commit.AuthorDate)));
            CreateMap<RepoAuthors, WorkActivityViewModel.RepositoryAuthors>()
                .ForMember(
                    dest => dest.AuthorCommits,
                    opt => opt.MapFrom(src => src.AuthorCommits.OrderByDescending(ac => ac.Commits.Count)));
        }

        private void MapWorkTeamBoardViewModel()
        {
            CreateMap<TeamBoard, WorkTeamBoardViewModel>();
            CreateMap<TeamCapacity, WorkTeamBoardViewModel.TeamBoardTeamCapacity>();
            CreateMap<Player, WorkTeamBoardViewModel.Player>();
            CreateMap<TeamMemberCapacity, WorkTeamBoardViewModel.Player.PlayerCapacity>();
            CreateMap<Score, WorkTeamBoardViewModel.Player.PlayerScore>();
            CreateMap<Point, WorkTeamBoardViewModel.Player.PlayerScore.Point>();
        }
    }
}