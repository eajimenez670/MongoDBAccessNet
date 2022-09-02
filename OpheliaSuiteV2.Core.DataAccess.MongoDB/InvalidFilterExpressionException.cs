using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los datos de la excepción que ocurre
    /// cuando la cadena de parámetros no cumple con la expresión
    /// </summary>
    public class InvalidFilterExpressionException : Exception
    {
        #region Properties

        /// <summary>
        /// Obtiene la cadena de parámetros
        /// </summary>
        public string QueryParams { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="queryParams">Cadena de parámetros</param>
        public InvalidFilterExpressionException(string queryParams) : base("Invalid filter expression")
        {
            QueryParams = queryParams?.Trim() ?? throw new ArgumentNullException();
            if (QueryParams == string.Empty) throw new ArgumentException();
        }

        #endregion
    }
}
