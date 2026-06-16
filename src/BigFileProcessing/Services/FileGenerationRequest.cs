using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public record FileGenerationRequest : IRequest
    {
        public int NumberOfRecords { get; init; }
    }
}
