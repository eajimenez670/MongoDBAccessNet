using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los datos de la excepción que ocurre
    /// cuando no se encuentra una propiedad Id dentro de
    /// una entidad
    /// </summary>
    public class PropertyIdNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// Obtiene el nombre de la entidad que en donde
        /// no se encontró la propiedad
        /// </summary>
        public string EntityName { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="entityName">Nombre de la entidad</param>
        public PropertyIdNotFoundException(string entityName) : base("Property id not found")
        {
            EntityName = entityName?.Trim() ?? throw new ArgumentNullException();
            if (EntityName == string.Empty) throw new ArgumentException();
        }

        #endregion
    }
}
