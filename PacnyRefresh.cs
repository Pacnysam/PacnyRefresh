using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace PacnyRefresh
{
	public class PacnyRefresh : Mod
    {
        public static PacnyRefresh Instance;

        PacnyRefresh() 
		{
            MusicSkipsVolumeRemap = true;
            Instance = this;
        }
	}
}
