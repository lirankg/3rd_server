using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Server.Models
{
    public class EntityState : ICloneable
    {
        public int startTime { get; set; }
        public int endTime { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public string logDescription { get; set; }
        public JObject data { get; set; }
        [JsonIgnore]
        public bool stateCompleted { get; set; }
        public bool isCurrentState { get; set; }

        public object Clone()
        {
            EntityState e = new EntityState()
            {
                startTime = this.startTime,
                data = this.data,
                endTime = this.endTime,
                latitude = this.latitude,
                logDescription = this.logDescription,
                longitude= this.longitude,
                stateCompleted = this.stateCompleted
            };
            return e;
        }
    }
}
