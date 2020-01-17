using CommonLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public static class CommonSetting
    {
        public static SendingProtocol sendingProtocol { get; set; } = SendingProtocol.Multicast;
    }
}
