using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands
{
    internal class TimeScaleCommand : SR2CCommand
    {
        public override string ID => "timescale";

        public override string Usage => "timescale <scale>";

        public override string Description => "Modifies game speed";

        public override bool Execute(string[] args)
        {
            try
            {
                Time.timeScale = float.Parse(args[0]);
                return true;
            }
            catch { return false; }
        }
    }
}
