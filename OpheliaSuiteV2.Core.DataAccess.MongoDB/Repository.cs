using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using OpheliaSuiteV2.Core.SSB.Lib.Enums;
using OpheliaSuiteV2.Core.SSB.Lib.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb {

    /// <summary>
    /// Contexto de la colección de mongo
    /// </summary>
    public abstract class Repository : IRepository {
        #region Fields

        /// <summary>
        /// Instancia a la colección mongo
        /// </summary>
        private IMongoCollection<BsonDocument> _collection;

        #endregion

        #region Properties

        /// <summary>
        /// Obtiene la instancia del contexto de datos
        /// </summary>
        public IDbContext Context { get; protected set; }

        /// <summary>
        /// Obtiene el nombre de la colección en la base de datos
        /// </summary>
        protected string CollectionName { get; private set; }

        /// <summary>
        /// Obtiene la cantidad total de registros
        /// </summary>
        public long Count {
            get {
                var stats = GetStats();
                long count = 0;
                if (stats.Contains("count"))
                    count += stats["count"].AsInt32;

                return count;
            }
        }

        /// <summary>
        /// Obtiene el tamaño total de los indices
        /// </summary>
        public long IndexSize {
            get {
                var stats = GetStats();
                long size = 0;
                if (stats.Contains("totalIndexSize"))
                    size += stats["totalIndexSize"].AsInt32;

                return size;
            }
        }

        /// <summary>
        /// Obtiene el tamaño total de la colección
        /// </summary>
        public long Size {
            get {
                var stats = GetStats();
                long size = 0;
                if (stats.Contains("size"))
                    size += stats["size"].AsInt32;

                return size;
            }
        }

        /// <summary>
        /// Obtiene el tamaño total de la colección con indices
        /// </summary>
        public long TotalSize {
            get {
                return (IndexSize + Size);
            }
        }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="collectionName">Nombre de la colección</param>
        /// <param name="context">Contexto al que pertenece el repositorio</param>
        protected Repository(string collectionName, IDbContext context = null) {
            CollectionName = collectionName?.Trim() ?? throw new ArgumentNullException(collectionName);
            if (CollectionName == string.Empty) throw new ArgumentException("Error");

            Context = context;
            _collection = Context.Database.GetCollection<BsonDocument>(CollectionName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asegura que la entidad tenga una propiedad Id
        /// de tipo <see cref="MongoDB.Bson.ObjectId"/> o
        /// de tipo <see cref="string"/> marcada con el atributo
        /// <see cref="BsonRepresentationAttribute"/>
        /// </summary>
        /// <param name="entity">Entidad a verificar</param>
        protected static void EnsurePropertyId(object entity) {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            Type type = entity.GetType();
            PropertyInfo prop = type.GetProperty("Id");
            if (prop != null) {
                if (prop.PropertyType != typeof(MongoDB.Bson.ObjectId)) {
                    if (prop.PropertyType == typeof(string)) {
                        var idAttribute = prop.GetCustomAttribute<BsonIdAttribute>();
                        if (idAttribute != null) {
                            var repreAttribute = prop.GetCustomAttribute<BsonRepresentationAttribute>();
                            if (repreAttribute != null) {
                                if (repreAttribute.Representation != MongoDB.Bson.BsonType.ObjectId)
                                    throw new PropertyIdNotFoundException(type.Name);
                            } else
                                throw new PropertyIdNotFoundException(type.Name);
                        } else
                            throw new PropertyIdNotFoundException(type.Name);
                    } else
                        throw new PropertyIdNotFoundException(type.Name);
                }
            } else
                throw new PropertyIdNotFoundException(type.Name);
        }

        /// <summary>
        /// Asegura que el contexto se haya inicializado
        /// </summary>
        protected void EnsureContextInitialized() {
            if (Context == null)
                throw new RepositoryContextNotInitializedException(GetType().Name);
        }

        /// <summary>
        /// Inicializa el repositorio con un contexto
        /// </summary>
        /// <param name="context">Contexto con el que inicializará el repositorio</param>
        public virtual void Initialize(IDbContext context) {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _collection = Context.Database.GetCollection<BsonDocument>(CollectionName);
        }

        /// <summary>
        /// Obtiene un documento con las estadísticas de la colección
        /// </summary>
        /// <returns></returns>
        public BsonDocument GetStats() {
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument { { "collstats", CollectionName } });
            return _collection.Database.RunCommand(command);
        }

        /// <summary>
        /// Agrega una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        public BsonDocument Add(BsonDocument entity) {
            EnsureContextInitialized();
            _collection.InsertOne(Context.Session, entity);

            return entity;
        }

        /// <summary>
        /// Actualiza una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <returns>Entidad actualizada</returns>
        public BsonDocument Update(BsonDocument entity) {
            EnsureContextInitialized();
            if (!entity.Contains("_id"))
                throw new PropertyIdNotFoundException(nameof(entity));
            var id = entity["_id"];
            _collection.ReplaceOne(Context.Session, Builders<BsonDocument>.Filter.Eq("_id", id), entity);

            return entity;
        }

        /// <summary>
        /// Elimina una entidad de la colección
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <returns>Entidad eliminada</returns>
        public BsonDocument Delete(BsonDocument entity) {
            EnsureContextInitialized();
            if (!entity.Contains("_id"))
                throw new PropertyIdNotFoundException(nameof(entity));
            var id = entity["_id"];
            _collection.DeleteOne(Context.Session, Builders<BsonDocument>.Filter.Eq("_id", id));

            return entity;
        }

        /// <summary>
        /// Elimina todas las entidades en la colección
        /// </summary>
        public void DeleteAll() {
            EnsureContextInitialized();
            _collection.DeleteMany(Context.Session, Builders<BsonDocument>.Filter.Empty);
        }

        /// <summary>
        /// Elimina la colección completa de la base de datos
        /// </summary>
        public void DropCollection() {
            EnsureContextInitialized();
            _collection.Database.DropCollection(Context.Session, CollectionName);
        }

        /// <summary>
        /// Buscar por un filtro definido
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<BsonDocument> FindBy(Filter<BsonDocument> filter) {
            EnsureContextInitialized();
            if (filter == null || filter.InternalFilter == null) return new List<BsonDocument>();

            if (filter.InternalSort != null)
                return _collection.Find(filter.InternalFilter).Sort(filter.InternalSort).ToList();

            return _collection.Find(filter.InternalFilter).ToList();
        }

        /// <summary>
        /// Lista todas las entidades de la colección
        /// </summary>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<BsonDocument> ListAll() {
            EnsureContextInitialized();
            return _collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        }

        /// <summary>
        /// Lista Las entidades que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<BsonDocument> List(Expression<Func<BsonDocument, bool>> predicate) {
            EnsureContextInitialized();
            return _collection.AsQueryable().Where(predicate).ToList();
        }

        #endregion
    }

    /// <summary>
    /// Define los métodos básicos del repositorio
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class Repository<TEntity> : Repository, IRepository<TEntity> where TEntity : EntityBase, new() {
        #region Fields

        /// <summary>
        /// Instancia a la colección mongo
        /// </summary>
        private IMongoCollection<TEntity> _collection;

        #endregion

        #region Properties

        /// <summary>
        /// Obtiene las opciones de administración de indices
        /// </summary>
        public IndexOption<TEntity> IndexOption { get; private set; }

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="collectionName">Nombre de la colección</param>
        protected Repository(string collectionName = null) : base((string.IsNullOrWhiteSpace(collectionName) ? typeof(TEntity).Name : collectionName.Trim())) { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="context">Contexto al que pertenece el repositorio</param>
        /// <param name="collectionName">Nombre de la colección</param>
        protected Repository(IDbContext context, string collectionName = null) : base((string.IsNullOrWhiteSpace(collectionName) ? typeof(TEntity).Name : collectionName.Trim()), context) {
            Initialize(context);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inicializa el repositorio con un contexto
        /// </summary>
        /// <param name="context">Contexto con el que inicializará el repositorio</param>
        public override void Initialize(IDbContext context) {
            context = context ?? throw new ArgumentNullException(nameof(context));
            base.Initialize(context);
            Context = context;
            _collection = Context.Database.GetCollection<TEntity>(CollectionName);
            IndexOption = new IndexOption<TEntity>(Context.Database.GetCollection<TEntity>(CollectionName));
        }

        /// <summary>
        /// Agrega una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        public TEntity Add(TEntity entity) {
            EnsureContextInitialized();
            EnsurePropertyId(entity);
            _collection.InsertOne(Context.Session, entity);

            entity.ChangeTracker = TrackerState.Unchanged;

            return entity;
        }

        /// <summary>
        /// Actualiza una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <returns>Entidad actualizada</returns>
        public TEntity Update(TEntity entity) {
            EnsureContextInitialized();
            EnsurePropertyId(entity);
            _collection.ReplaceOne(Context.Session, Builders<TEntity>.Filter.Eq("Id", entity.Id), entity);

            entity.ChangeTracker = TrackerState.Unchanged;

            return entity;
        }

        /// <summary>
        /// Elimina una entidad de la colección
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <returns>Entidad eliminada</returns>
        public TEntity Delete(TEntity entity) {
            EnsureContextInitialized();
            EnsurePropertyId(entity);
            _collection.DeleteOne(Context.Session, Builders<TEntity>.Filter.Eq("Id", entity.Id));

            return entity;
        }

        /// <summary>
        /// Obtiene un valor que indica si una entidad existe
        /// </summary>
        /// <param name="id">Id de la entidad</param>
        /// <returns>Valor que indica si la entidad existe</returns>
        public bool Exists(string id) {
            EnsureContextInitialized();

            return (_collection.CountDocuments(new Filter<TEntity>().AndEq("Id", id.NotNullOrEmpty(nameof(id)).Trim()).InternalFilter) > 0);
        }

        /// <summary>
        /// Buscar por el Id de la entidad
        /// </summary>
        /// <param name="id">Id de la entidad</param>
        /// <returns>Entidad resultado</returns>
        public TEntity Find(string id) {
            EnsureContextInitialized();

            return _collection.Find(new Filter<TEntity>().AndEq("Id", id.NotNullOrEmpty(nameof(id)).Trim()).InternalFilter).FirstOrDefault();
        }

        /// <summary>
        /// Buscar por un filtro definido
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<TEntity> FindBy(Filter<TEntity> filter) {
            EnsureContextInitialized();
            if (filter == null || filter.InternalFilter == null) return new List<TEntity>();

            if (filter.InternalSort != null)
                return _collection.Find(filter.InternalFilter).Sort(filter.InternalSort).ToList();

            return _collection.Find(filter.InternalFilter).ToList();
        }

        /// <summary>
        /// Lista todas las entidades de la colección
        /// </summary>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<TEntity> ListEntities() {
            EnsureContextInitialized();
            return _collection.Find(Builders<TEntity>.Filter.Empty).ToList();
        }

        /// <summary>
        /// Lista Las entidades que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        public IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate) {
            EnsureContextInitialized();
            return _collection.AsQueryable().Where(predicate).ToList();
        }

        #endregion
    }
}
