using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OpheliaSuiteV2.Core.SSB.Lib.Enums;
using System;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula las características básicas de una entidad MongoDb
    /// </summary>
    public abstract class EntityBase
    {
        #region Properties

        /// <summary>
        /// Obtiene o asigna el Id de la entidad
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// Obtiene o asigna el estado del rastreador de cambios de la entidad
        /// </summary>
        [BsonIgnore]
        public TrackerState ChangeTracker { get; set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        public EntityBase()
        {
            Id = null;
            ChangeTracker = TrackerState.Unchanged;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asegura que el estado de la entidad sea
        /// coherente con el valor de su Id
        /// </summary>
        public void EnsureState()
        {
            if (ChangeTracker == TrackerState.New) //Si es nueva no puede tener Id
            {
                if (Id != null && Id?.Trim() != string.Empty)
                    throw new IncoherentEntityStateException(GetType().Name);
            } //Si no es nueva debe tener Id
            else if (Id == null || Id?.Trim() == string.Empty)
                throw new IncoherentEntityStateException(GetType().Name);
        }

        #endregion

        #region Services

        /// <summary>
        /// Crea un nuevo id para una entidad
        /// </summary>
        /// <returns>Nuevo id</returns>
        public static string NewId()
        {
            string id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            return id;
        }

        #endregion
    }
}
