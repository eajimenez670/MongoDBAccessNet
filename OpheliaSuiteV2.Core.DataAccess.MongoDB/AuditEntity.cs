using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los datos de un mensaje de auditoria
    /// </summary>
    public class AuditMessage : EntityBase
    {
        /// <summary>
        /// Fecha
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Mensaje
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Tipo
        /// </summary>
        public string Type { get; set; }
    }
}
