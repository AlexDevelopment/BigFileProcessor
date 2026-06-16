using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public record FileGenerationResponse : IResponse
    {
        public required string FileName { get; init; }
        public int NumberOfRecords { get; init; }
    }
}
