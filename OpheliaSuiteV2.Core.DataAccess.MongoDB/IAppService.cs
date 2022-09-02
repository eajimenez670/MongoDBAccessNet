using OpheliaSuiteV2.Core.SSB.Lib.Loggers;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Define los atributos y métodos de un servicio de aplicación
    /// </summary>
    public interface IAppService
    {
        #region Properties

        /// <summary>
        /// Contexto de datos
        /// </summary>
        IDbContext Context { get; }
        /// <summary>
        /// Obtiene el registrador de eventos
        /// </summary>
        ILogger Logger { get; }

        #endregion
    }
}
