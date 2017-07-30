using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public abstract class ListApiResponseBase<TValue>
    {
        public int Count { get; set; }

        public ICollection<TValue> Value { get; set; } = new List<TValue>();
    }
}