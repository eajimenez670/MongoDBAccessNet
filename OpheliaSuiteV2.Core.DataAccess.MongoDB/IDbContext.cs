using MongoDB.Driver;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Define las características y comportamientos de un contexto
    /// de colecciones (Bases de datos) MongoDb
    /// </summary>
    public interface IDbContext
    {
        #region Properties

        /// <summary>
        /// Obtiene la instancia a la base de datos MongoDb
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// Obtiene la sesión actual
        /// </summary>
        IClientSessionHandle Session { get; }

        /// <summary>
        /// Valor que indica si el contexto se encuentra en una transacción
        /// </summary>
        bool InTransaction { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Inicia una nueva transacción
        /// </summary>
        void BeginTransaction();
        /// <summary>
        /// Confirma los cambios realizados
        /// </summary>
        void CommitChanges();
        /// <summary>
        /// Retrocede o descarta los cambios realizados
        /// </summary>
        void RollbackChanges();
        /// <summary>
        /// Obtiene un repositorio de tipo TRepository
        /// </summary>
        /// <typeparam name="TRepository">Tipo del repositorio</typeparam>
        /// <returns>Repositorio</returns>
        TRepository GetRepository<TRepository>() where TRepository : IRepository;
        /// <summary>
        /// Obtiene un servicio de dominio de tipo TRepository
        /// </summary>
        /// <typeparam name="TDomainService">Tipo del servicio</typeparam>
        /// <returns>Servicio</returns>
        TDomainService GetDomainService<TDomainService>() where TDomainService : IDomainService;

        #endregion
    }
}
