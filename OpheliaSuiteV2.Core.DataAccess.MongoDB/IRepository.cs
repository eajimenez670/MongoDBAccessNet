using MongoDB.Bson;
using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Provee métodos para la administración de indices
    /// </summary>
    public sealed class IndexOption<TEntity>
    {
        #region Fields

        /// <summary>
        /// Instancia a la colección
        /// </summary>
        private readonly IMongoCollection<TEntity> _collection;

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="collection">Instancia a la colección</param>
        internal IndexOption(IMongoCollection<TEntity> collection)
        {
            _collection = collection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Obtiene todos los indices de la colección
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BsonDocument> GetIndexes()
        {
            return _collection.Indexes.List().ToList();
        }

        /// <summary>
        /// Crea un indice en la colección
        /// </summary>
        /// <param name="name">Nombre del indice</param>
        /// <param name="fields">Arreglo de campos que componen el indice</param>
        /// <param name="isUnique">Valor que indica si el indice es único</param>
        /// <param name="extendedProperties">Propiedades extentidas del indice si lo requiere</param>
        public void CreateIndex(string name, FieldIndex[] fields, bool isUnique = false, dynamic extendedProperties = null)
        {
            name = name?.Trim() ?? throw new ArgumentNullException();
            if (name == string.Empty) throw new ArgumentException();

            fields = fields ?? throw new ArgumentNullException();
            if (fields.Length == 0) throw new ArgumentException();

            IndexKeysDefinition<TEntity> idxKeys = null;
            CreateIndexOptions options = null;

            string[] fieldNames = fields.Select(f => f.Name).ToArray();

            if (fields.Any(f => f.IndexType == IndexType.Text))
            {
                string language = "spanish";
                int version = 3;
                if (extendedProperties != null)
                {
                    if ((extendedProperties as IDictionary<string, object>).Keys.Any(k => k.ToLower() == "defaultlanguage"))
                        language = (extendedProperties as IDictionary<string, object>).First(k => k.Key.ToLower() == "defaultlanguage").Value.ToString();
                    if ((extendedProperties as IDictionary<string, object>).Keys.Any(k => k.ToLower() == "textIndexVersion"))
                        int.TryParse((extendedProperties as IDictionary<string, object>).First(k => k.Key.ToLower() == "textIndexVersion").Value.ToString(), out version);
                }
                options = new CreateIndexOptions { Name = name, TextIndexVersion = 3, DefaultLanguage = language };
                foreach (string field in fieldNames)
                {
                    if (idxKeys == null)
                        idxKeys = Builders<TEntity>.IndexKeys.Text(field);
                    else
                        idxKeys = idxKeys.Text(field);
                }
            }
            else
            {
                options = new CreateIndexOptions { Name = name, Unique = isUnique };
                foreach (FieldIndex field in fields)
                {
                    if (idxKeys == null)
                    {
                        if (field.IndexType == IndexType.Ascending)
                            idxKeys = Builders<TEntity>.IndexKeys.Ascending(field.Name);
                        else
                            idxKeys = Builders<TEntity>.IndexKeys.Descending(field.Name);
                    }
                    else
                    {
                        if (field.IndexType == IndexType.Ascending)
                            idxKeys = idxKeys.Ascending(field.Name);
                        else
                            idxKeys = idxKeys.Descending(field.Name);
                    }
                }
            }
            if (idxKeys != null)
                _collection.Indexes.CreateOne(new CreateIndexModel<TEntity>(idxKeys, options));
        }

        #endregion

        #region Enums

        /// <summary>
        /// Tipo de indice
        /// </summary>
        public enum IndexType
        {
            /// <summary>
            /// Indice de texto
            /// </summary>
            Text,
            /// <summary>
            /// Cuaquier otro tipo de indice
            /// que no requiera busqueda parcial de texto.
            /// Se creará de forma ascendente
            /// </summary>
            Ascending,
            /// <summary>
            /// Cuaquier otro tipo de indice
            /// que no requiera busqueda parcial de texto.
            /// Se creará de forma descendente
            /// </summary>
            Descending
        }

        #endregion

        #region Classes

        /// <summary>
        /// Encapsula los datos de un campo que compone un indice
        /// y la configuración de dicho campo en el indice
        /// </summary>
        public sealed class FieldIndex
        {
            #region Properties

            /// <summary>
            /// Obtiene el nombre del campo
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// Obtiene el tipo del campo en el indice
            /// </summary>
            public IndexType IndexType { get; private set; }

            #endregion

            #region Builders

            /// <summary>
            /// Inicializa una nueva instancia de la clase
            /// </summary>
            /// <param name="name">Nombre del indice</param>
            /// <param name="indexType">Tipo</param>
            public FieldIndex(string name, IndexType indexType)
            {
                Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
                if (Name == string.Empty) throw new ArgumentNullException(nameof(name));
                IndexType = indexType;
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// Define los atributos y comportamientos de un repositorio MongoDb
    /// </summary>
    public interface IRepository
    {
        #region Properties

        /// <summary>
        /// Obtiene la instancia del contexto de datos
        /// </summary>
        IDbContext Context { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Inicializa el repositorio con un contexto
        /// </summary>
        /// <param name="context">Contexto con el que inicializará el repositorio</param>
        void Initialize(IDbContext context);

        /// <summary>
        /// Agrega una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        BsonDocument Add(BsonDocument entity);

        /// <summary>
        /// Actualiza una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <returns>Entidad actualizada</returns>
        BsonDocument Update(BsonDocument entity);

        /// <summary>
        /// Elimina una entidad de la colección
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <returns>Entidad eliminada</returns>
        BsonDocument Delete(BsonDocument entity);

        /// <summary>
        /// Elimina todas las entidades en la colección
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Elimina la colección completa de la base de datos
        /// </summary>
        void DropCollection();

        /// <summary>
        /// Buscar por un filtro definido
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<BsonDocument> FindBy(Filter<BsonDocument> filter);

        /// <summary>
        /// Lista todas las entidades de la colección
        /// </summary>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<BsonDocument> ListAll();

        /// <summary>
        /// Lista Las entidades que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<BsonDocument> List(Expression<Func<BsonDocument, bool>> predicate);

        #endregion
    }

    /// <summary>
    /// Define los atributos y comportamientos de un repositorio MongoDb
    /// </summary>
    public interface IRepository<TEntity> : IRepository where TEntity : EntityBase, new()
    {
        #region Properties

        /// <summary>
        /// Obtiene las opciones de administración de indices
        /// </summary>
        IndexOption<TEntity> IndexOption { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Agrega una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Actualiza una entidad en la colección
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <returns>Entidad actualizada</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Elimina una entidad de la colección
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <returns>Entidad eliminada</returns>
        TEntity Delete(TEntity entity);

        /// <summary>
        /// Obtiene un valor que indica si una entidad existe
        /// </summary>
        /// <param name="id">Id de la entidad</param>
        /// <returns>Valor que indica si la entidad existe</returns>
        bool Exists(string id);

        /// <summary>
        /// Buscar por el Id de la entidad
        /// </summary>
        /// <param name="id">Id de la entidad</param>
        /// <returns></returns>
        TEntity Find(string id);

        /// <summary>
        /// Buscar por un filtro definido
        /// </summary>
        /// <param name="filter">Filtro de busqueda</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<TEntity> FindBy(Filter<TEntity> filter);

        /// <summary>
        /// Lista todas las entidades de la colección
        /// </summary>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<TEntity> ListEntities();

        /// <summary>
        /// Lista Las entidades que cumplan con el criterio e búsqueda
        /// </summary>
        /// <param name="predicate">Predicado usado para la consulta</param>
        /// <returns>Enumeración de entidades resultado</returns>
        IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
