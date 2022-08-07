using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Normihelp
{
    public static class Corrector
    {
        public static Dictionary<string, Action<int, int>> CorrectionHandlers = new Dictionary<string, Action<int, int>>();
    }
}
