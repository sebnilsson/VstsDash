using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VstsDash.RestApi;

namespace VstsDash.WebApp.Controllers.Api
{
    [Route("api/files", Name = RouteNames.ApiFiles)]
    public class FilesController : ControllerBase
    {
        private readonly IStreamApiService _streamApiService;

        public FilesController(AppSettings appSettings, IStreamApiService streamApiService, IWorkApiService workApi)
            : base(appSettings, workApi)
        {
            if (appSettings == null) throw new ArgumentNullException(nameof(appSettings));
            if (workApi == null) throw new ArgumentNullException(nameof(workApi));

            _streamApiService = streamApiService ?? throw new ArgumentNullException(nameof(streamApiService));
        }

        [HttpGet]
        [ResponseCache(Duration = 600)]
        public async Task<IActionResult> Get(string url)
        {
            var response = await _streamApiService.GetResponse(url);

            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType.ToString();

            var result = new FileStreamResult(stream, contentType);
            return result;
        }
    }
}