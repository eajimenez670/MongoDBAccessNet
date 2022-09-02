namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula las propiedades de configuración de una conexión a MongoDb
    /// </summary>
    public sealed class DbSettings
    {
        #region Properties

        /// <summary>
        /// Obtiene o asigna la cadena de conexión
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Obtiene o asigna el nombre de la base de datos
        /// </summary>
        public string Database { get; set; }

        #endregion
    }
}
