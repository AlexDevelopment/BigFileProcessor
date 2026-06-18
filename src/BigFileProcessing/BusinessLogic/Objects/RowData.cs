using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Objects
{
    public record RowData(int Number, string Text)
    {
        public override string ToString() => $"{Number}.{Text}";        
    }
}
