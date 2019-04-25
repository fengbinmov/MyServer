using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer.Demo
{
    sealed class RoomSystem
    {
        private static RoomSystem _roomSystem;
        public static RoomSystem RS
        {
            get
            {
                if (_roomSystem == null)
                    _roomSystem = new RoomSystem();
                return _roomSystem;
            }
        }

        public Dictionary<string,RoomContainer> roomMarks = new Dictionary<string, RoomContainer>();

    }
}
