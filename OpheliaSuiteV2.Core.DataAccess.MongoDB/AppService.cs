using Microsoft.AspNetCore.Http;

using OpheliaSuiteV2.Core.SSB.Lib;
using OpheliaSuiteV2.Core.SSB.Lib.Helpers;
using OpheliaSuiteV2.Core.SSB.Lib.Loggers;
using OpheliaSuiteV2.Core.SSB.Lib.Models;

using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Clase base de los servicios de aplicación
    /// </summary>
    public abstract class AppService : IAppService
    {
        #region Properties

        /// <summary>
        /// Contexto de datos
        /// </summary>
        public IDbContext Context { get; private set; }
        /// <summary>
        /// Obtiene el registrador de eventos
        /// </summary>
        public ILogger Logger { get; private set; }
        /// <summary>
        /// Contexto Http
        /// </summary>
        public IHttpContextAccessor HttpAppContext { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="context">Contexto de datos</param>
        /// <param name="logger">Regitrador de eventos</param>
        public AppService(IDbContext context, ILogger logger)
        {
            Context = context.NotNull(nameof(context));
            Logger = logger.NotNull(nameof(logger));
            if (Globals.ServiceProvider != null)
                HttpAppContext = (Globals.ServiceProvider.GetService(typeof(IHttpContextAccessor)).NotNull() as IHttpContextAccessor);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Da manejo a un excepción
        /// </summary>
        /// <param name="ex">Excepción a manejar</param>
        protected virtual RequestResult<TResult> HandleException<TResult>(Exception ex)
        {
            Logger.WriteException(ex);
            if (Context.InTransaction)
                Context.RollbackChanges();

            if (ex.GetType().Name == "UnauthorizedException")
            {
                if (HttpAppContext != null)
                {
                    HttpAppContext.HttpContext.Response.StatusCode = 401;
                    return null;
                }

                throw ex;
            }

            return RequestResult<TResult>.CreateError(ex.Message);
        }

        #endregion
    }
}
