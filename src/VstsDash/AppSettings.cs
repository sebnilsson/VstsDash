using Microsoft.Extensions.Configuration;

namespace VstsDash
{
    public class AppSettings
    {
        public AppSettings(IConfiguration configuration)
        {
            AccessToken = configuration[nameof(AccessToken)];
            Account = configuration[nameof(Account)];
            DefaultProjectId = configuration[nameof(DefaultProjectId)];
            DefaultRepositoryId = configuration[nameof(DefaultRepositoryId)];
            DefaultTeamId = configuration[nameof(DefaultTeamId)];
        }

        public string AccessToken { get; }

        public string Account { get; }

        public string DefaultProjectId { get; }

        public string DefaultRepositoryId { get; }

        public string DefaultTeamId { get; }
    }
}