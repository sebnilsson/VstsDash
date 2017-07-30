using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VstsDash.AppServices.TeamMeta;
using VstsDash.RestApi;
using VstsDash.WebApp.ViewModels;

namespace VstsDash.WebApp.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly TeamMetaAppService _teamMetaAppService;

        public HomeController(
            AppSettings appSettings,
            IMapper mapper,
            TeamMetaAppService teamMetaAppService,
            IWorkApiService workApi)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));
            if (workApi == null) throw new ArgumentNullException(nameof(workApi));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _teamMetaAppService = teamMetaAppService ?? throw new ArgumentNullException(nameof(teamMetaAppService));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _teamMetaAppService.GetTeamMeta();

            var model = _mapper.Map<HomeMetaViewModel>(result);
            return View(model);
        }

        public async Task<IActionResult> Meta()
        {
            var result = await _teamMetaAppService.GetTeamMeta();

            var model = _mapper.Map<HomeMetaViewModel>(result);
            return View(model);
        }
    }
}