using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using MongoDB.Driver;

using OpheliaSuiteV2.Core.SSB.Lib.Helpers;
using OpheliaSuiteV2.Core.SSB.Lib.Loggers;
using OpheliaSuiteV2.Core.SSB.Lib.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb {
    /// <summary>
    /// Clase base de los servicios de aplicación que
    /// provee un método de proyección de entidades paginadas
    /// </summary>
    abstract public class EntityProjectionAppService : AppService {
        /// <summary>
        /// Inicialia una nueva instancia de la clase
        /// </summary>
        /// <param name="context">Contexto de datos</param>
        /// <param name="logger"></param>
        protected EntityProjectionAppService(IDbContext context, ILogger logger) : base(context, logger) { }

        /// <summary>
        /// Obtiene una proyección de resultados de una entidad
        /// de forma páginada
        /// </summary>
        /// <returns>Resultado de la proyección</returns>
        protected RequestResult<ProjectionResult> GetProjection<TEntity>(HttpRequest req) where TEntity : EntityBase, new() {
            try {
                req.NotNull();
                //Se agrega estrategia de serialización de propiedades
                SessionConfig.NamingStrategy = NamingStrategyType.Default;
                if (req.Headers.ContainsKey(SSB.Lib.Globals.OPHJSON_NAMING_STRATEGY_HEADER)) {
                    var strategyHeader = req.Headers[SSB.Lib.Globals.OPHJSON_NAMING_STRATEGY_HEADER];
                    if (strategyHeader.Count > 0) {
                        var strategy = strategyHeader[0].ToLower();
                        if (strategy == "camelcase")
                            SessionConfig.NamingStrategy = NamingStrategyType.CamelCase;
                    }
                }
                ProjectionResult projectionResult = new ProjectionResult();
                var accountCollection = Context.Database.GetCollection<TEntity>(typeof(TEntity).Name);
                var filter = Builders<TEntity>.Filter.Empty;
                ProjectionDefinition<TEntity> projection = Builders<TEntity>.Projection.Include("_id");
                //Obtenemos todo el queryString
                foreach (KeyValuePair<string, StringValues> kv in req.Query) {
                    if (kv.Key == "_id" || kv.Key == "Id")
                        continue;
                    if (kv.Key == "@Page" && int.TryParse(kv.Value[0], out _)) {
                        projectionResult.Page = int.Parse(kv.Value[0]);
                        continue;
                    }
                    if (kv.Key == "@PageSize" && int.TryParse(kv.Value[0], out _)) {
                        projectionResult.PageSize = int.Parse(kv.Value[0]);
                        continue;
                    }

                    if (kv.Value[0] == "1")
                        projection = projection.Include(kv.Key);
                    else
                        projection = projection.Exclude(kv.Key);
                }
                //Ejecutamos la consulta
                var iResult = accountCollection.Find(filter);
                //Validamos los datos del paginado
                projectionResult.Page = (projectionResult.Page < 1 ? 1 : projectionResult.Page);
                projectionResult.Count = iResult.CountDocuments();
                projectionResult.PageSize = (projectionResult.PageSize < 1 ? projectionResult.Count : projectionResult.PageSize);

                if (projectionResult.Count > projectionResult.PageSize)
                    iResult = iResult.Skip((int)((projectionResult.Page - 1) * projectionResult.PageSize)).Limit((int)projectionResult.PageSize);
                else {
                    projectionResult.Page = 1;
                    projectionResult.PageSize = projectionResult.Count;
                }

                if (projection != null)
                    projectionResult.Result = iResult.Project<EntityProjection>(projection).ToList();
                else
                    projectionResult.Result = iResult.ToList();

                return RequestResult<ProjectionResult>.CreateSuccessful(projectionResult);
            } catch (Exception ex) {
                Logger.WriteException(ex);
                return RequestResult<ProjectionResult>.CreateError(ex.Message);
            }
        }
    }
}
