using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Server.Services
{
    public class TriggerHandler
    {
        private bool isPaused;
        private string simulationId;
        private ResponseHandler responseHandler;
        private int simulationTime;
        private Timer triggerJobsTimer;
        private int runInterval = 5;
        private List<Entity> allEntities;
        private StreamHandler streamHandler;

        public TriggerHandler(ResponseHandler responseHandler, string id, List<Entity> allEntities, StreamHandler streamHandler)
        {
            this.responseHandler = responseHandler;
            this.simulationId = id;
            this.simulationTime = 0;
            this.isPaused = true;
            this.allEntities = allEntities;
            this.streamHandler = streamHandler;
        }

        internal void StartNewSimulation()
        {
            //or - trigger every 5 seconds to see if we're still runing and then check from all the remaning if it's time yet
            if (triggerJobsTimer == null)
                triggerJobsTimer = new Timer();
            this.isPaused = false;
            triggerJobsTimer.Interval = 1000 * runInterval;//every 1 seconds
            triggerJobsTimer.Elapsed += LoopTimer_Elapsed;
            triggerJobsTimer.Start();
            LoopTimer_Elapsed(null, null);

        }



        private async void LoopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //cleanConnections();

            if (isPaused)
                return;

            this.simulationTime = updateSimulationTime();

            foreach (var entity in allEntities)
            {
                if (entity.states == null)
                    continue;
                foreach (var entityState in entity.states.Where(s => s.stateCompleted == false).OrderBy(s => s.startTime))
                {
                    if (this.simulationTime > entityState.startTime)
                    {
                        this.responseHandler.SendEntityToClient(this.simulationId, entity, entityState);
                        entityState.stateCompleted = true;

                        //set this state to true
                        entity.states.ForEach(s => s.isCurrentState = false);
                        entityState.isCurrentState = true;
                    }
                }

            }

        }

        private void cleanConnections()
        {
            var allSteams = this.streamHandler.getAllStreams();
            foreach (var stream in allSteams)
            {
                if ((DateTime.Now - stream.Value).TotalMinutes > 0.5)
                {
                    this.streamHandler.CloseConnection(stream.Key);
                }

            }
        }

        internal object GetDataForEntity(int entityId)
        {
            var ent = this.allEntities.Where(e => e.id == entityId).FirstOrDefault();
            if (ent == null)
                return null;
            var state = (ent.states != null) ? ent.states.Where(s => s.isCurrentState).FirstOrDefault() : null;

            object res = responseHandler.BuildReturnObject(ent, state);
            return res;
        }

        private int updateSimulationTime()
        {
            return this.simulationTime + runInterval;
        }
    }
}