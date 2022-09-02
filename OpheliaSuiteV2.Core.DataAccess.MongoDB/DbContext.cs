using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using OpheliaSuiteV2.Core.SSB.Lib;

using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Implementación de un contexto de colecciones (Base de datos)
    /// MongoDb
    /// </summary>
    public abstract class DbContext : IDbContext
    {
        #region Properties

        /// <summary>
        /// Obtiene la base de datos del contexto
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Obtiene la sesión actual
        /// </summary>
        public IClientSessionHandle Session { get; }

        /// <summary>
        /// Valor que indica si el contexto se encuentra en una transacción
        /// </summary>
        public bool InTransaction { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="client">Instancia del cliente de conexión a MongoDb</param>
        /// <param name="database">Nombre de la base de datos</param>
        public DbContext(IDbClient client, string database = null)
        {
            client = client ?? throw new ArgumentNullException(nameof(client));
            database = database?.Trim() ?? string.Empty;
            database = (database == string.Empty ? this.GetType().Name : database);

            Database = client.Client.GetDatabase(database);
            Session = Database.Client.StartSession();
        }

        /// <summary>
        /// Inicia una nueva transacción
        /// </summary>
        public void BeginTransaction()
        {
            if (Session != null && Session.Client.Settings.ConnectionMode == ConnectionMode.ReplicaSet && !Session.IsInTransaction)
            {
                Session.StartTransaction();
                InTransaction = true;
            }
        }
        /// <summary>
        /// Confirma los cambios realizados
        /// </summary>
        public void CommitChanges()
        {
            if (Session != null && Session.Client.Settings.ConnectionMode == ConnectionMode.ReplicaSet && Session.IsInTransaction)
            {
                Session.CommitTransaction();
                InTransaction = false;
            }
        }
        /// <summary>
        /// Retrocede o descarta los cambios realizados
        /// </summary>
        public void RollbackChanges()
        {
            if (Session != null && Session.Client.Settings.ConnectionMode == ConnectionMode.ReplicaSet && Session.IsInTransaction)
            {
                Session.AbortTransaction();
                InTransaction = false;
            }
        }
        /// <summary>
        /// Obtiene un repositorio de tipo TRepository
        /// </summary>
        /// <typeparam name="TRepository">Tipo del repositorio</typeparam>
        /// <returns>Repositorio</returns>
        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            if (Globals.ServiceProvider == null) return default;

            TRepository rep = Globals.ServiceProvider.GetService<TRepository>();
            rep.Initialize(this);

            return rep;
        }

        /// <summary>
        /// Obtiene un servicio de dominio de tipo TRepository
        /// </summary>
        /// <typeparam name="TDomainService">Tipo del servicio</typeparam>
        /// <returns>Servicio</returns>
        public TDomainService GetDomainService<TDomainService>() where TDomainService : IDomainService
        {
            if (Globals.ServiceProvider == null) return default;

            TDomainService rep = Globals.ServiceProvider.GetService<TDomainService>();
            rep.SetContext(this);

            return rep;
        }

        #endregion
    }
}
