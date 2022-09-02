using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Implementación de un cliente MongoDb
    /// </summary>
    public sealed class DbClient : IDbClient
    {
        #region Properties

        /// <summary>
        /// Obtiene el cliente de conexión a MongoDb
        /// </summary>
        public IMongoClient Client { get; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="connectionstring">Cadena de conexión al servidor MongoDb</param>
        public DbClient(string connectionstring)
        {
            connectionstring = connectionstring?.Trim() ?? throw new ArgumentNullException(nameof(connectionstring));
            connectionstring = connectionstring == string.Empty ? throw new ArgumentException() : connectionstring;

            Client = new MongoClient(connectionstring);
        }

        /// <summary>
        /// Aqui se inicializa la configuración del driver
        /// </summary>
        static DbClient()
        {
            BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(MongoDB.Bson.BsonType.Double));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Destruye la instancia
        /// </summary>
        public void Dispose()
        {
            Console.WriteLine("Destruyendo la instancia");
        }

        #endregion
    }
}
