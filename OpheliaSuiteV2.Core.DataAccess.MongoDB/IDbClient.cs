using System;

using MongoDB.Driver;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Define los atributos y comportamientos de un cliente MongoDb
    /// </summary>
    public interface IDbClient : IDisposable
    {
        #region Properties

        /// <summary>
        /// Obtiene el cliente MongoDb que contiene la conexión
        /// </summary>
        IMongoClient Client { get; }

        #endregion
    }
}
