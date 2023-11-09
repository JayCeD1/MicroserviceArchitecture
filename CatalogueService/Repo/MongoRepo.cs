using CatalogueService.Entities;
using MongoDB.Driver;
using SharpCompress.Common;

namespace CatalogueService.Repo
{
    public class MongoRepo<T> : IRepo<T> where T : IEntity
    {

        private readonly IMongoCollection<T> dbCollection;

        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepo(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentException(nameof(entity));
            }
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
