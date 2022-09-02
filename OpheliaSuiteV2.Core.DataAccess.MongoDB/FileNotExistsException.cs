using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los datos de la excepción que ocurre
    /// cuando un archivo no existe en la colección
    /// </summary>
    public class FileNotExistsException : Exception
    {
        #region Properties

        /// <summary>
        /// Obtiene el Id del archivo que no existe
        /// </summary>
        public string FileId { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="fileId">Id del archivo</param>
        public FileNotExistsException(string fileId) : base("File not exists")
        {
            fileId = fileId?.Trim() ?? throw new ArgumentNullException();
            if (fileId == string.Empty) throw new ArgumentException();
        }

        #endregion
    }
}
