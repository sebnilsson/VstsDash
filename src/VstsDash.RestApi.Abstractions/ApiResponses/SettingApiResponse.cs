using System;
using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public class SettingApiResponse
    {
        public SettingApiResponseIteration BacklogIteration { get; set; }

        public SettingApiResponseIteration DefaultIteration { get; set; }

        public ICollection<string> WorkingDays { get; set; } = new List<string>();

        public class SettingApiResponseIteration
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string Path { get; set; }

            public string Url { get; set; }
        }
    }
}