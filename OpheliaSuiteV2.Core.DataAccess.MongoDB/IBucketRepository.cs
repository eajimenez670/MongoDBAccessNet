using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Define los atributos y comportamientos de un repositorio
    /// a una colección de archivos en una base de datos MongoDb
    /// </summary>
    public interface IBucketRepository<TEntity> where TEntity : FileMetadata, new()
    {
        #region Properties

        /// <summary>
        /// Obtiene las opciones de administración de indices
        /// </summary>
        IndexOption<TEntity> IndexOption { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Obtiene tamaño total de los archivos en kb's
        /// </summary>
        /// <returns>Tamaño total</returns>
        decimal GetTotalFileSize();
        /// <summary>
        /// Lista las entidades que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<FileInfo<TEntity>> FindOverFileInfo(Expression<Func<FileInfo<TEntity>, bool>> predicate);
        /// <summary>
        /// Lista las entidades que cumplan con el filtro
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<FileInfo<TEntity>> FindOverFileInfoByFilter(Filter<FileInfo<TEntity>> filter);
        /// <summary>
        /// Lista las entidades de metadata que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<TEntity> FindOverMetadata(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// Lista las entidades de metadata que cumplan con el filtro
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<TEntity> FindOverMetadataByFilter(Filter<TEntity> filter);
        /// <summary>
        /// Carga el archivo de un arreglo de bytes
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <param name="content">Arreglo de bytes que contiene el archivo</param>
        /// <param name="metadata">Entidad de metadatos</param>
        /// <returns>Id de carga del archivo</returns>
        TEntity UploadFromBytes(string fileName, byte[] content, TEntity metadata);
        /// <summary>
        /// Carga el archivo de un arreglo de bytes
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <param name="content">Arreglo de bytes que contiene el archivo</param>
        /// <param name="metadata">Entidad de metadatos</param>
        /// <returns>Id de carga del archivo</returns>
        TEntity UploadFromStream(string fileName, Stream content, TEntity metadata);
        /// <summary>
        /// Descarga el archivo a un arreglo de bytes
        /// </summary>
        /// <param name="id">Id de la entidad metadata del archivo a descargar</param>
        /// <returns>Arreglo de bytes del archivo</returns>
        byte[] DownloadAsBytes(string id);
        /// <summary>
        /// Descarga el archivo a una secuencia de bytes
        /// </summary>
        /// <param name="id">Id de la entidad metadata del archivo a descargar</param>
        /// <param name="target">Secuencia de bytes destino</param>
        /// <returns>Secuencia de bytes del archivo</returns>
        void DownloadToStream(string id, Stream target);
        /// <summary>
        /// Elimina un archivo por su id
        /// </summary>
        /// <param name="id">Id de la entidad metadata del archivo a descargar</param>
        void Delete(string id);

        #endregion
    }
}
