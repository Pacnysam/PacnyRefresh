using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace PacnyRefresh
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class PacnyRefresh : Mod
	{
        //public override string Name => "Pacny`s Refresh";
        PacnyRefresh() 
		{
            MusicSkipsVolumeRemap = true;
        }
	}
}
