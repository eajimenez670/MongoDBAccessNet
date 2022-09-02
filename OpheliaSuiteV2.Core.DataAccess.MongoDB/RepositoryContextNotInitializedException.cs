using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los datos de la excepción que ocurre
    /// cuando un repositorio no ha inicializado su contexto
    /// </summary>
    public class RepositoryContextNotInitializedException : Exception
    {
        #region Properties

        /// <summary>
        /// Obtiene el nombre del repositorio que no ha 
        /// inicializado el contexto
        /// </summary>
        public string RepositoryName { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="repositoryName">Nombre del repositorio</param>
        public RepositoryContextNotInitializedException(string repositoryName) : base("Repository context not initialized")
        {
            RepositoryName = repositoryName?.Trim() ?? throw new ArgumentNullException();
            if (RepositoryName == string.Empty) throw new ArgumentException();
        }

        #endregion
    }
}
