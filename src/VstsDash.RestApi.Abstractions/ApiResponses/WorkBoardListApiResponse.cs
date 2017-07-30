using System;

namespace VstsDash.RestApi.ApiResponses
{
    public class WorkBoardListApiResponse : ListApiResponseBase<WorkBoardListApiResponse.Item>
    {
        public class Item
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }
        }
    }
}