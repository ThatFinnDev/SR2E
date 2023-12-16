using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SR2E.Commands.Secret
{
    internal class FastModeCommand : SR2CCommand
    {
        public override string ID => "fastmode";

        public override bool Hidden => true;

        public override string Usage => "fastmode";
            
        public override string Description => "Speeds up time ( careful [: ) ";

        public override bool Execute(string[] args)
        {
            SceneContext.Instance.TimeDirector._timeFactor *= 1.75f;
            Time.timeScale += .5f;

            return true;
        }
    }
}
