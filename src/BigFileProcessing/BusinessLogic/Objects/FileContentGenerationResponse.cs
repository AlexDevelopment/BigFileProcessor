using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Objects
{
    public record FileContentGenerationResponse
    {
        public int TotalRecords { get; init; } = 0;
        public string Content { get; init; } = string.Empty;
    }
}
