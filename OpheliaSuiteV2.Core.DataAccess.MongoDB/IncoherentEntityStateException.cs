using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los datos de la excepción que ocurre
    /// cuando el estado de una entidad es incoherente
    /// </summary>
    public class IncoherentEntityStateException : Exception
    {
        #region Properties

        /// <summary>
        /// Obtiene el nombre de la entidad
        /// </summary>
        public string EntityName { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="entityName">Nombre de la entidad incoherente</param>
        public IncoherentEntityStateException(string entityName) : base("Incoherent entity state")
        {
            EntityName = entityName?.Trim() ?? string.Empty;
        }

        #endregion
    }
}
