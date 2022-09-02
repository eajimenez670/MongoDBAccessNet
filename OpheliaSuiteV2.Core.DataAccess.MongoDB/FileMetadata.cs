using System;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Clase base que representa una entidad metadata
    /// para un archivo cargado en una base de datos MongoDb
    /// </summary>
    public abstract class FileMetadata
    {
        /// <summary>
        /// Obtiene o asigna el Id de la entidad
        /// </summary>  
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }
        /// <summary>
        /// Obtiene o asigna el Id del archivo
        /// </summary>
        public virtual string FileId { get; set; }
    }

    /// <summary>
    /// Encapsula los datos de un archivo en el sistema de archivos de MongoDb
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad de metadata</typeparam>
    public sealed class FileInfo<TEntity> where TEntity : FileMetadata, new()
    {
        #region Members
        /// <summary>
        /// Id del archivo
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// Tamaño del archivo
        /// </summary>
        [BsonElement("length")]
        public long Length { get; set; }
        /// <summary>
        /// Tamaño de los chunks del archivo
        /// </summary>
        [BsonElement("chunkSize")]
        public int ChunkSize { get; set; }
        /// <summary>
        /// Fecha de modificación
        /// </summary>
        [BsonElement("uploadDate")]
        public DateTime UploadDate { get; set; }
        /// <summary>
        /// Formato Hash 
        /// </summary>
        [BsonElement("md5")]
        public string Md5 { get; set; }
        /// <summary>
        /// Nombre del archivo
        /// </summary>
        [BsonElement("filename")]
        public string Filename { get; set; }
        /// <summary>
        /// Meta data del archivo
        /// </summary>
        [BsonElement("metadata")]
        public dynamic Metadata { get; set; }

        #endregion
    }
}
