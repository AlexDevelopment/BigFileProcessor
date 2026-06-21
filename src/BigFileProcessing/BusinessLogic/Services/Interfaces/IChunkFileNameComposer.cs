using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Interfaces
{
    public interface IChunkFileNameComposer
    {
        string GetFullFileName(string folder, int chunkIndex);
    }
}
