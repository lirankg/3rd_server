using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowOrigin")]
    [ApiController]
    public class BackofficeController : Controller
    {
        private InMemoryServiceHandler inMemoryServiceHandler;
        private StreamHandler streamHandler;

        public BackofficeController(InMemoryServiceHandler inMemoryServiceHandler, StreamHandler streamHandler)
        {
            this.inMemoryServiceHandler = inMemoryServiceHandler;
            this.streamHandler = streamHandler;
        }

        [Route("GetFullRun")]
        public async Task<List<Entity>> GetFullRun()
        {
            return inMemoryServiceHandler.GetFullRun();
        }

        [Route("socketTester")]
        public IActionResult socketTester()
        {
            return View();
        }

        [HttpGet("startSimulation/{id}")]
        public void startSimulation(string id)
        {
            inMemoryServiceHandler.startNewSim(id);
        }

        [HttpGet("getDataForEntity/{simulationId}/{entityId}")]
        public Object getDataForEntity(string simulationId, int entityId)
        {
            return inMemoryServiceHandler.GetDataForEntity(simulationId, entityId);
        }

        [HttpGet("sendTestsMessage/{id}")]
        public void sendTestsMessage(string id)
        {
            for (int i = 0; i < 20; i++)
            {
             
                //this.streamHandler.SendMessageToId(id, e, messageType.logView);
                var allentitis = inMemoryServiceHandler.GetFullRun();
                foreach (var entity in allentitis)
                {
                    Random r = new Random();
                    EntityState es = new EntityState()
                    {
                        longitude = (33.258648 + r.NextDouble()).ToString(),
                        latitude= (35.134572 + r.NextDouble()).ToString(),
                    };
                    var entityeResponse = BuildReturnObject(entity, es);
                    this.streamHandler.SendMessageToId(id, entityeResponse, messageType.mapMovement);

                }
                Thread.Sleep(1000);
            }
        }

        //for tests only -remove this !
        internal object BuildReturnObject(Entity entity, EntityState entityState)
        {
            return new
            {
                entity = entity,
                state = entityState
            };
        }

        [HttpGet("getInitialState")]
        public List<Object> getInitialState()
        {
            return this.inMemoryServiceHandler.GetInitialState();
        }

        [HttpGet("initMemory")]
        public void initMemory()
        {
            inMemoryServiceHandler.initMem();
        }

    }
}