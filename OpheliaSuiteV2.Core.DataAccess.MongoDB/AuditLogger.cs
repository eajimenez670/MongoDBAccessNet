using OpheliaSuiteV2.Core.SSB.Lib.Loggers;

using System;
using System.Text;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Logger de auditoría
    /// </summary>
    public class AuditLogger : Logger
    {
        #region Fields

        /// <summary>
        /// Contexto del módulo SSB
        /// </summary>
        private readonly IDbContext _auditContext;
        /// <summary>
        /// Repositorio de mensajes de auditoria
        /// </summary>
        private readonly IRepository<AuditMessage> _repository;

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una instancia de la clase
        /// </summary>
        public AuditLogger(IDbContext auditContext) : base()
        {
            _auditContext = auditContext ?? throw new ArgumentException("Error", nameof(auditContext));
            _repository = _auditContext.GetRepository<IRepository<AuditMessage>>();
            _repository.IndexOption.CreateIndex("idx_types", new IndexOption<AuditMessage>.FieldIndex[] { new IndexOption<AuditMessage>.FieldIndex(nameof(AuditMessage.Type), IndexOption<AuditMessage>.IndexType.Text) });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Guarda el mensaje en Mongo
        /// </summary>
        /// <param name="auditMessage">Información del mensaje</param>
        private void SaveMessage(AuditMessage auditMessage)
        {
            _repository.Add(auditMessage);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Escribe un mensaje tipo Información en el Registro de Auditoria
        /// </summary>
        /// <param name="message">Mensaje</param>
        public override void WriteInfoMessage(string message)
        {
            AuditMessage msg = new ()
            {
                Type = nameof(AuditMessageType.Info),
                Date = DateTime.Now,
                Message = message
            };

            try
            {
                SaveMessage(msg);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        /// <summary>
        /// Escribe un mensjae tipo Error en el registro de Auditora
        /// </summary>
        /// <param name="message">Mensaje</param>
        public override void WriteErrorMessage(string message)
        {
            AuditMessage msg = new ()
            {
                Type = nameof(AuditMessageType.Error),
                Date = DateTime.Now,
                Message = message
            };

            try
            {
                SaveMessage(msg);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        /// <summary>
        /// Escribe un mensaje tipo Debug en el Registro de Auditoria
        /// </summary>
        /// <param name="message">Mensaje</param>
        public override void WriteDebugMessage(string message)
        {
            AuditMessage msg = new ()
            {
                Type = nameof(AuditMessageType.Debug),
                Date = DateTime.Now,
                Message = message
            };

            try
            {
                SaveMessage(msg);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        /// <summary>
        /// Escribe un mensaje tipo Alerta en el Registro de Auditoria
        /// </summary>
        /// <param name="message">Mensaje</param>
        public override void WriteWarningMessage(string message)
        {
            AuditMessage msg = new ()
            {
                Type = nameof(AuditMessageType.Warning),
                Date = DateTime.Now,
                Message = message
            };

            try
            {
                SaveMessage(msg);
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        /// <summary>
        /// Escribe un mensaje tipo Excepción en el Registro de Auditoria
        /// </summary>
        /// <param name="ex">Excepción</param>
        /// <param name="message">Mensaje</param>
        public override void WriteException(Exception ex, string message = "")
        {
            message = message?.Trim() ?? "";

            StringBuilder sb = new ();
            sb.Append(message);
            if (sb.Length > 0)
                sb.AppendLine().AppendLine();

            WriteException(ex, ref sb);

            AuditMessage msg = new ()
            {
                Type = nameof(AuditMessageType.Exception),
                Date = DateTime.Now,
                Message = sb.ToString()
            };

            try
            {
                SaveMessage(msg);
            }
            catch (Exception exc) { Console.WriteLine(exc.Message); } //Evitamos el error de escritura en el log de eventos
        }

        /// <summary>
        /// Destruye la instancia
        /// </summary>
        public override void Dispose()
        {
            return;
        }

        #endregion
    }

}
