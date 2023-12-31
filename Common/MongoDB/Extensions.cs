﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            //Register MongoDB Serializers
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var mongoClient = new MongoClient(configuration.GetConnectionString("ConnString"));
                return mongoClient.GetDatabase(configuration.GetConnectionString("Database"));
            });

            return services;
        }

        public static IServiceCollection AddMongoRepo<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddScoped<IRepo<T>>(serviceProvide =>
            {
                var database = serviceProvide.GetService<IMongoDatabase>();
                return new MongoRepo<T>(database, collectionName);
            });

            return services;
        }
    }

}
