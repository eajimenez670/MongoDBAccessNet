using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Implementa un repositorio
    /// a una colección de archivos en una base de datos MongoDb
    /// </summary>
    public class BucketRepository<TEntity> : IBucketRepository<TEntity> where TEntity : FileMetadata, new()
    {
        #region Fields

        /// <summary>
        /// Instancia a la colección de entidades
        /// </summary>
        protected IMongoCollection<FileInfo<TEntity>> _entities;
        /// <summary>
        /// Instancia a la colección de metadata de las entidades
        /// </summary>
        protected IMongoCollection<TEntity> _metadataEntities;
        /// <summary>
        /// Instancia al repositorio de archivos
        /// </summary>
        protected IGridFSBucket _bucket;

        #endregion

        #region Properties

        /// <summary>
        /// Obtiene la instancia del contexto de datos
        /// </summary>
        public IDbContext Context { get; private set; }

        /// <summary>
        /// Obtiene el nombre del bucket de archivos
        /// </summary>
        protected string BucketName { get; private set; }

        /// <summary>
        /// Obtiene las opciones de administración de indices
        /// </summary>
        public IndexOption<TEntity> IndexOption { get; private set; }

        /// <summary>
        /// Obtiene las opciones de administración de indices internos
        /// </summary>
        internal IndexOption<FileInfo<TEntity>> IndexOptionInternal { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="context">Contexto al que pertenece el repositorio</param>
        /// <param name="bucketName">Nombre del bucket de archivos</param>
        public BucketRepository(IDbContext context, string bucketName = null)
        {
            Initialize(context, bucketName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inicializa el repositorio con un contexto
        /// </summary>
        /// <param name="context">Contexto con el que inicializará el repositorio</param>
        /// <param name="bucketName">Nombre del bucket de archivos</param>
        public virtual void Initialize(IDbContext context, string bucketName = null)
        {
            Context = context ?? throw new ArgumentNullException();
            BucketName = bucketName?.Trim() ?? typeof(TEntity).Name;
            BucketName = $"{BucketName}_bucket";
            _entities = Context.Database.GetCollection<FileInfo<TEntity>>($"{BucketName}.files");
            _metadataEntities = Context.Database.GetCollection<TEntity>($"{BucketName}.metadata");
            _bucket = new GridFSBucket(Context.Database, new GridFSBucketOptions
            {
                BucketName = BucketName
            });
            IndexOption = new IndexOption<TEntity>(_metadataEntities);
            IndexOptionInternal = new IndexOption<FileInfo<TEntity>>(_entities);
            CreateDefaultIndexes();
        }

        /// <summary>
        /// Crea los indices por defecto que tendrá la colección
        /// de información de los archivos
        /// </summary>
        private void CreateDefaultIndexes()
        {
            IndexOption.CreateIndex("IX_FileId", new IndexOption<TEntity>.FieldIndex[] { new IndexOption<TEntity>.FieldIndex("FileId", IndexOption<TEntity>.IndexType.Ascending) });
            IndexOptionInternal.CreateIndex("IX_FileName", new IndexOption<FileInfo<TEntity>>.FieldIndex[] { new IndexOption<FileInfo<TEntity>>.FieldIndex("filename", IndexOption<FileInfo<TEntity>>.IndexType.Ascending) });
        }

        /// <summary>
        /// Crea las opciones por defecto usadas para cargar un archivo
        /// </summary>
        /// <returns>Objeto de opciones</returns>
        private GridFSUploadOptions CreateUploadOptions()
        {
            var metadata = new
            {
                OphAssemblyVersion = this.GetType().Assembly.GetName().Version.ToString(),
                OphCreationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return new GridFSUploadOptions
            {
                Metadata = metadata.ToBsonDocument()
            };
        }

        /// <summary>
        /// Obtiene tamaño total de los archivos en kb's
        /// </summary>
        /// <returns>Tamaño total</returns>
        public decimal GetTotalFileSize() => (_entities.AsQueryable().Sum(f => f.Length) / 1024);

        /// <summary>
        /// Lista los archivos que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<FileInfo<TEntity>> FindOverFileInfo(Expression<Func<FileInfo<TEntity>, bool>> predicate)
        {
            var res = _entities.AsQueryable().Where(predicate).ToList();
            return res;
        }

        /// <summary>
        /// Lista las entidades que cumplan con el filtro
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<FileInfo<TEntity>> FindOverFileInfoByFilter(Filter<FileInfo<TEntity>> filter)
        {
            if (filter == null || filter.InternalFilter == null) return new List<FileInfo<TEntity>>();

            return _entities.Find(filter.InternalFilter).ToList();
        }

        /// <summary>
        /// Lista las entidades de metadata que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<TEntity> FindOverMetadata(Expression<Func<TEntity, bool>> predicate)
        {
            var res = _metadataEntities.AsQueryable().Where(predicate).ToList();
            return res;
        }

        /// <summary>
        /// Lista las entidades de metadata que cumplan con el filtro
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<TEntity> FindOverMetadataByFilter(Filter<TEntity> filter)
        {
            if (filter == null || filter.InternalFilter == null) return new List<TEntity>();

            return _metadataEntities.Find(filter.InternalFilter).ToList();
        }

        /// <summary>
        /// Carga el archivo de un arreglo de bytes
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <param name="content">Arreglo de bytes que contiene el archivo</param>
        /// <param name="metadata">Entidad de metadatos</param>
        /// <returns>Id de carga del archivo</returns>
        public TEntity UploadFromBytes(string fileName, byte[] content, TEntity metadata)
        {
            return UploadFromStream(fileName, new MemoryStream(content), metadata);
        }

        /// <summary>
        /// Carga el archivo de un arreglo de bytes
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <param name="content">Arreglo de bytes que contiene el archivo</param>
        /// <param name="metadata">Entidad de metadatos</param>
        /// <returns>Id de carga del archivo</returns>
        public TEntity UploadFromStream(string fileName, Stream content, TEntity metadata)
        {
            fileName = fileName?.Trim() ?? throw new ArgumentNullException(nameof(fileName));
            if (fileName == string.Empty) throw new ArgumentNullException(nameof(fileName));
            metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));

            var obj = _bucket.UploadFromStream(fileName.ToLower(), content, CreateUploadOptions());
            metadata.FileId = obj.ToString();
            _metadataEntities.InsertOne(metadata);

            return metadata;
        }

        /// <summary>
        /// Descarga el archivo a un arreglo de bytes
        /// </summary>
        /// <param name="id">Id de la entidad metadata del archivo a descargar</param>
        /// <returns>Arreglo de bytes del archivo</returns>
        public byte[] DownloadAsBytes(string id)
        {
            id = id?.Trim() ?? throw new ArgumentNullException(nameof(id));
            if (id == string.Empty) throw new ArgumentNullException(nameof(id));

            TEntity entity = FindOverMetadataByFilter(new Filter<TEntity>().AndEq(nameof(FileMetadata.Id), id)).FirstOrDefault();
            if (entity == null) throw new FileNotExistsException(id);

            var file = _bucket.DownloadAsBytes(new ObjectId(entity.FileId));
            return file;
        }

        /// <summary>
        /// Descarga el archivo a una secuencia de bytes
        /// </summary>
        /// <param name="id">Id de la entidad metadata del archivo a descargar</param>
        /// <param name="target">Secuencia de bytes destino</param>
        /// <returns>Secuencia de bytes del archivo</returns>
        public void DownloadToStream(string id, Stream target)
        {
            id = id?.Trim() ?? throw new ArgumentNullException(nameof(id));
            if (id == string.Empty) throw new ArgumentNullException(nameof(id));

            TEntity entity = FindOverMetadataByFilter(new Filter<TEntity>().AndEq(nameof(FileMetadata.Id), id)).FirstOrDefault();
            if (entity == null) throw new FileNotExistsException(id);

            target.Seek(0, SeekOrigin.Begin);
            _bucket.DownloadToStream(new ObjectId(entity.FileId), target);
        }

        /// <summary>
        /// Elimina un archivo por su id
        /// </summary>
        /// <param name="id">Id de la entidad metadata del archivo a descargar</param>
        public void Delete(string id)
        {
            id = id?.Trim() ?? throw new ArgumentNullException(nameof(id));
            if (id == string.Empty) throw new ArgumentNullException(nameof(id));

            TEntity entity = FindOverMetadataByFilter(new Filter<TEntity>().AndEq(nameof(FileMetadata.Id), id)).FirstOrDefault();
            if (entity == null) throw new FileNotExistsException(id);

            _bucket.Delete(new ObjectId(entity.FileId));
            _metadataEntities.DeleteOne(new Filter<TEntity>().AndEq(nameof(FileMetadata.Id), id).InternalFilter);
        }

        #endregion
    }
}
