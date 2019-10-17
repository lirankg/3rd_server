using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Models
{
    public class Entity : ICloneable
    {
        public int id { get; set; }
        public string name { get; set; }
        public side side { get; set; }
        public string image { get; set; }
        [JsonIgnore]
        public List<EntityState> states { get; set; }

        public object Clone()
        {
            Entity e = new Entity()
            {
                id = this.id,
                name = this.name,
                side = this.side,
                states = this.states != null ? this.states.Select(item => (EntityState)item.Clone()).ToList() : null,
                image = this.image
            };
            return e;


        }
    }
}
