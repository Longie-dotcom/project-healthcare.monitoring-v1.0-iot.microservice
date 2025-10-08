using MongoDB.Driver;

namespace Infrastructure.Persistence.Context
{
    public class MongoDBContext
    {
        #region Attributes
        #endregion

        #region Properties
        public IMongoDatabase Database { get; }
        #endregion

        public MongoDBContext(MongoSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            Database = client.GetDatabase(settings.DatabaseName);
        }

        #region Methods
        #endregion
    }
}
