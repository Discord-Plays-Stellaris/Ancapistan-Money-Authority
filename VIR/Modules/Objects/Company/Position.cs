using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIR.Modules.Objects.Company
{
    public class Position
    {
        public string ID;
        public int level;
        public string name;
        public Collection<Position> manages;
    }
}
