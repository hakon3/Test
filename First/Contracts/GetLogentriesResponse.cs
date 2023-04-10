using System.Collections.Generic;

namespace First.Contracts
{
    internal record GetLogentriesResponse
    {
        public List<LogEntryDescription> LogEntries { get; set; }
    }
}
