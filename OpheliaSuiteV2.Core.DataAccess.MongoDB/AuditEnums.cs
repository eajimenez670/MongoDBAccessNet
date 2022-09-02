namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Proyectos de auditoria por vertical
    /// </summary>
    public enum AuditVertical
    {
        /// <summary>
        /// Ophelia
        /// </summary>
        OPHELIA,
        /// <summary>
        /// Davinci
        /// </summary>
        DAVINCI,
        /// <summary>
        /// Athenea
        /// </summary>
        ATHENEA,
        /// <summary>
        /// Klinic
        /// </summary>
        KLINIC,
        /// <summary>
        /// Andrómeda
        /// </summary>
        ANDROMEDA,
        /// <summary>
        /// Watching
        /// </summary>
        WATCHING,
        /// <summary>
        /// Goodday
        /// </summary>
        GOODDAY,
        /// <summary>
        /// Motor de reglas
        /// </summary>
        BRM,
        /// <summary>
        /// Gestor documental
        /// </summary>
        DMS
    }

    /// <summary>
    /// Acciones de auditoría
    /// </summary>
    public enum AuditAction
    {
        /// <summary>
        /// Crear
        /// </summary>
        Create,
        /// <summary>
        /// Leer
        /// </summary>
        Read,
        /// <summary>
        /// Modificar
        /// </summary>
        Update,
        /// <summary>
        /// Eliminar
        /// </summary>
        Delete
    }

    /// <summary>
    /// Tipos de mensajes de auditoría
    /// </summary>
    public enum AuditMessageType
    {
        /// <summary>
        /// Error
        /// </summary>
        Error,
        /// <summary>
        /// Información
        /// </summary>
        Info,
        /// <summary>
        /// Excepción
        /// </summary>
        Exception,
        /// <summary>
        /// Advertencia
        /// </summary>
        Warning,
        /// <summary>
        /// Debug
        /// </summary>
        Debug
    }
}
