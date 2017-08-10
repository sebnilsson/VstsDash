﻿using System;
using System.Collections.Generic;

namespace VstsDash.RestApi.ApiResponses
{
    public class QueryApiResponse
    {
        public IReadOnlyCollection<QueryApiResponse> Children { get; set; } = new List<QueryApiResponse>();

        public IReadOnlyCollection<Column> Columns { get; set; } = new List<Column>();

        public DateTimeOffset CreatedDate { get; set; }

        public bool HasChildren { get; set; }

        public Guid Id { get; set; }

        public bool IsFolder { get; set; }

        public bool IsPublic { get; set; }

        public DateTimeOffset LastModifiedDate { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string QueryType { get; set; }

        public IReadOnlyCollection<SortColumn> SortColumns { get; set; } = new List<SortColumn>();

        public string Url { get; set; }

        public string Wiql { get; set; }

        public class Column
        {
            public string Name { get; set; }

            public string ReferenceName { get; set; }

            public string Url { get; set; }
        }

        public class SortColumn
        {
            public bool Descending { get; set; }

            public Column Field { get; set; }
        }
    }
}