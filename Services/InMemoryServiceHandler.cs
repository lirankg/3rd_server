using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Services
{
    public class InMemoryServiceHandler
    {
        private InMemoryService inMemoryService;
        private ResponseHandler responseHandler;
        private Dictionary<string, TriggerHandler> triggersCollection;
        private StreamHandler streamHandler;
        private int max_start_time;

        public InMemoryServiceHandler(InMemoryService inMemoryService, ResponseHandler responseHandler, StreamHandler streamHandler)
        {
            this.inMemoryService = inMemoryService;
            this.responseHandler = responseHandler;
            this.triggersCollection = new Dictionary<string, TriggerHandler>();
            this.streamHandler = streamHandler;
            //todo implement to understand when the sim is over
            //max_start_time = inMemoryService.GetAllEntities().SelectMany(s => s.States.Select(s1 => s1.StartTime)).Max();
        }

        internal List<Entity> GetFullRun()
        {
            return inMemoryService.GetAllEntities();
        }

        internal List<Object> GetInitialState()
        {
            List<Object> initState = new List<object>();
            var allEntities = inMemoryService.GetAllEntities();
            foreach (var entity in allEntities)
            {
                Object obj;
                if (entity.states != null)
                    obj = responseHandler.BuildReturnObject(entity, entity.states.Where(s => s.startTime == 0).FirstOrDefault());
                else
                    obj = responseHandler.BuildReturnObject(entity, null);
                initState.Add(obj);
            }
            return initState;
        }

        internal void initMem()
        {
            this.inMemoryService.InitMemoryForces();
        }

        internal object GetDataForEntity(string simulationId, int entityId)
        {
            if (!this.triggersCollection.ContainsKey(simulationId))
            {
                //todo error - no sim id exists
                return null;
            }
            else
            {
                var tc = this.triggersCollection[simulationId];
                return tc.GetDataForEntity(entityId);
            }
            //todo error - cant find the entity
            return null;
        }

        internal void startNewSim(string id)
        {
            if (this.triggersCollection.ContainsKey(id))
            {
                //todo error - duplicate id
            }
            else
            {
                var clonedEntities = GetFullRun().Select(item => (Entity)item.Clone()).ToList();
                TriggerHandler tr = new TriggerHandler(responseHandler, id, clonedEntities, streamHandler);
                this.triggersCollection.Add(id, tr);
                tr.StartNewSimulation();

            }

        }
    }
}
