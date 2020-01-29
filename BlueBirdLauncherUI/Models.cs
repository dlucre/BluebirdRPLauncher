using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpEren.Fivem.ServerStatus.ServerAPI;

namespace BlueBirdLauncherUI
{
    public class FiveMServerStatusReturnModel
    {
        public bool server_online { get; set; }
        public Fivem raw_server_response { get; set; }

        public int max_users { get; set; }
        public int current_users { get; set; }

    }
}
