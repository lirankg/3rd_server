using CsvHelper;
using Microsoft.Extensions.Options;
using Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    public class InMemoryService
    {
        private CustomConfiguration customConfig;
        private TsvHandler tsvHandler;
        private List<Entity> entitiesCollection;

        #region initializations
        public InMemoryService(IOptions<CustomConfiguration> customConfig, TsvHandler tsvHandler)
        {
            this.customConfig = customConfig.Value;
            this.tsvHandler = tsvHandler;
            InitMemory(true);

            //todo - add an hourly refresh
        }

        internal List<Entity> GetAllEntities()
        {
            return this.entitiesCollection;
        }

        internal void InitMemoryForces()
        {
            InitMemory(false);
        }
        private async void InitMemory(bool isFirstInit)
        {
            //get the data from DB and set the in-mem
            try
            {
                await ReadDimensionFromLocalDb(customConfig.LocalDbFilePathDimesion);
                await ReadDataFromLocalDb(customConfig.LocalDbFilePathData);

            }
            catch (Exception ex)
            {

                // throw;
            }
        }

        private async Task ReadDataFromLocalDb(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                using (var csv = new CsvReader(sr))
                {
                    csv.Configuration.Delimiter = "\t";
                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.Context.HeaderRecord;
                    while (await csv.ReadAsync())
                    {
                        try
                        {
                            EntityState es = new EntityState();
                            int entityId = Int32.Parse(csv.GetField("entity_id"));
                            es.startTime = Int32.Parse(csv.GetField("start_time"));
                            es.endTime = Int32.Parse(csv.GetField("end_time"));
                            es.latitude = (csv.GetField("latitude"));
                            es.longitude = (csv.GetField("longitude"));
                            es.logDescription = (csv.GetField("log_description"));

                            es.data = new Newtonsoft.Json.Linq.JObject();
                            foreach (var header in headers)
                            {
                                if (header.StartsWith("data_"))
                                {
                                    es.data[header] = csv.GetField(header);
                                }
                            }
                            ExpandEntityWithState(entityId, es);
                        }
                        catch (Exception ex)
                        {
                            //todo something happend and this object cant be initialized
                            throw;
                        }
                    }

                }
            }
        }

        private void ExpandEntityWithState(int entityId, EntityState es)
        {
            var entity = this.entitiesCollection.Where(e => e.id == entityId);
            if (entity.Count() == 1)
            {
                var e = entity.First();
                if (e.states == null)
                    e.states = new List<EntityState>();
                if (es.startTime == 0)
                {
                    es.stateCompleted = true;
                    es.isCurrentState = true;
                }
                e.states.Add(es);
            }
            else
            {
                //todo error handling
            }

        }

        private async Task ReadDimensionFromLocalDb(string filePath)
        {
            this.entitiesCollection = new List<Entity>();
            using (TextReader reader = File.OpenText(filePath))
            {
                CsvReader csv = new CsvReader(reader);
                csv.Configuration.Delimiter = "\t";
                csv.Configuration.MissingFieldFound = null;
                while (csv.Read())
                {
                    Entity entity = csv.GetRecord<Entity>();
                    this.entitiesCollection.Add(entity);
                }
            }
        }


        #endregion initializations

    }
}
