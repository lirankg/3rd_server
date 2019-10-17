using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public enum side { blue = 1, red = 2 }
    public enum forceType { soldier = 1, drown = 2, camera = 3, car = 4 }
    public enum messageType { logView = 1, mapMovement = 2, newConnection = 3 }
    public enum resultStatus { success = 1, warning = 2, criticalError = 3 }

}
