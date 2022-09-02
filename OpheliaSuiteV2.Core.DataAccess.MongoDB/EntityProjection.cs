using Newtonsoft.Json.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb {

    /// <summary>
    /// Encapsula el resultado de una proyección de entidad
    /// </summary>
    public class ProjectionResult {
        /// <summary>
        /// Cantidad total de registros
        /// </summary>
        public long Count { get; set; }
        /// <summary>
        /// Tamaño de página
        /// </summary>
        public long PageSize { get; set; }
        /// <summary>
        /// Página actual
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Resultado de la proyección
        /// </summary>
        public IEnumerable<dynamic> Result { get; set; }
    }

    internal class EntityProjection : IDynamicMetaObjectProvider, IDictionary<string, object> {
        #region Members

        private readonly FieldNames fields;
        private object[] values;

        #endregion

        #region Builders

        public EntityProjection() : this(new FieldNames(new string[] { }), new object[] { }) { }

        private EntityProjection(FieldNames fields, object[] values) {
            this.fields = fields ?? throw new ArgumentNullException(nameof(fields));
            this.values = values ?? throw new ArgumentNullException(nameof(values));
        }

        #endregion

        #region Methods

        public DynamicMetaObject GetMetaObject(Expression parameter) {
            return new DynamicRowMetaObject(parameter, System.Dynamic.BindingRestrictions.Empty, this);
        }

        public bool Remove(string key) {
            int index = fields.IndexOfName(key);
            if (index < 0 || index >= values.Length || values[index] is DeadValue)
                return false;
            values[index] = DeadValue.Default;
            return true;
        }

        public bool Remove(KeyValuePair<string, object> item) {
            IDictionary<string, object> dic = this;
            return dic.Remove(item.Key);
        }

        public bool TryGetValue(string key, out object value) {
            var index = fields.IndexOfName(key);
            if (index < 0) { // No existe
                value = null;
                return false;
            }
            // Existe
            value = index < values.Length ? values[index] : null;
            if (value is DeadValue) {
                value = null;
                return false;
            }
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public object this[string key] {
            get {
                TryGetValue(key, out object val);
                return val;
            }
            set {
                SetValue(key, value, false);
            }
        }

        public ICollection<string> Keys {
            get {
                return this.Select(kv => kv.Key).ToArray();
            }
        }

        public ICollection<object> Values {
            get {
                return this.Select(kv => kv.Value).ToArray();
            }
        }

        public int Count {
            get {
                int count = 0;
                for (int i = 0; i < values.Length; i++) {
                    if (!(values[i] is DeadValue))
                        count++;
                }
                return count;
            }
        }

        public bool IsReadOnly => false;

        public void Add(string key, object value) {
            SetValue(key, value, true);
        }

        public void Add(KeyValuePair<string, object> item) {
            IDictionary<string, object> dic = this;
            dic.Add(item.Key, item.Value);
        }

        public void Clear() {
            for (int i = 0; i < values.Length; i++)
                values[i] = DeadValue.Default;
        }

        public bool Contains(KeyValuePair<string, object> item) {
            return TryGetValue(item.Key, out object value) && Equals(value, item.Value);
        }

        public bool ContainsKey(string key) {
            int index = fields.IndexOfName(key);
            if (index < 0 || index >= values.Length || values[index] is DeadValue)
                return false;
            return true;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
            foreach (var kv in this) {
                array[arrayIndex++] = kv;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            var names = fields.Fields;
            for (var i = 0; i < names.Length; i++) {
                object value = i < values.Length ? values[i] : null;
                if (!(value is DeadValue)) {
                    yield return new KeyValuePair<string, object>(names[i], value);
                }
            }
        }

        public object SetValue(string key, object value) {
            return SetValue(key, value, false);
        }

        private object SetValue(string key, object value, bool isAdd) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            //Cuando el campo a agregar sea _id se cambia a Id
            key = (key == "_id" ? ResolvePropertyName("Id") : ResolvePropertyName(key));

            int index = fields.IndexOfName(key);
            if (index < 0) {
                index = fields.AddField(key);
            } else if (isAdd && index < values.Length && !(values[index] is DeadValue)) {
                // Semanticamente el valor ya existe
                throw new ArgumentException("An item with the same key has already been added", nameof(key));
            }
            int oldLength = values.Length;
            if (oldLength <= index) {
                Array.Resize(ref values, fields.FieldCount);
                for (int i = oldLength; i < values.Length; i++) {
                    values[i] = DeadValue.Default;
                }
            }
            return values[index] = value;
        }

        private string ResolvePropertyName(string name) {
            NamingStrategy strategy = null;
            if (SessionConfig.NamingStrategy == NamingStrategyType.CamelCase)
                strategy = new CamelCaseNamingStrategy();

            if (strategy != null)
                return strategy.GetPropertyName(name, false);

            return name;
        }

        #endregion

        #region Classes

        private sealed class DeadValue {
            public static readonly DeadValue Default = new DeadValue();
            private DeadValue() { /* Oculta el constructor */
            }
        }

        #endregion
    }

    #region Dynamic

    internal sealed class FieldNames {
        #region Members

        private string[] _fieldNames;
        /// <summary>
        /// Obtiene el arreglo de nombres de propiedades
        /// </summary>
        public string[] Fields => this._fieldNames;

        /// <summary>
        /// Diccionario de los indices de las propiedades
        /// </summary>
        private readonly Dictionary<string, int> _fieldNamesLookup;

        /// <summary>
        /// Obtiene la cantidad de propiedades que hay
        /// </summary>
        public int FieldCount => this._fieldNames.Length;

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        /// <param name="fieldNames">Arreglo de nombres de propiedades</param>
        public FieldNames(string[] fieldNames) {
            this._fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));

            this._fieldNamesLookup = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
            for (int i = fieldNames.Length - 1; i >= 0; i--) {
                string key = fieldNames[i];
                if (key != null)
                    this._fieldNamesLookup[key] = i;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Obtiene el indice de un nombre de propiedad
        /// </summary>
        /// <param name="name">Nombre de la propiedad</param>
        /// <returns>Indice en el que se encuentra</returns>
        public int IndexOfName(string name) {
            return (name != null && this._fieldNamesLookup.TryGetValue(name, out int result)) ? result : -1;
        }

        /// <summary>
        /// Agrega un nuevo nombre de propiedad al arreglo
        /// </summary>
        /// <param name="name">Nombre de la nueva propiedad</param>
        /// <returns>El indice que ocupa el nuevo nombre</returns>
        public int AddField(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (this._fieldNamesLookup.ContainsKey(name))
                throw new InvalidOperationException("El campo ya existe: " + name);
            int oldLen = this._fieldNames.Length;
            Array.Resize(ref this._fieldNames, oldLen + 1);
            this._fieldNames[oldLen] = name;
            this._fieldNamesLookup[name] = oldLen;
            return oldLen;
        }

        /// <summary>
        /// Obtiene un valor que indica si el campo existe
        /// </summary>
        /// <param name="name">Nombre del campo</param>
        /// <returns>Valor que indica si el campo existe</returns>
        public bool FieldExists(string name) => name != null && this._fieldNamesLookup.ContainsKey(name);

        #endregion
    }

    internal sealed class DynamicRowMetaObject : DynamicMetaObject {
        private static readonly MethodInfo getValueMethod = typeof(IDictionary<string, object>).GetProperty("Item").GetGetMethod();
        private static readonly MethodInfo setValueMethod = typeof(EntityProjection).GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });

        public DynamicRowMetaObject(System.Linq.Expressions.Expression expression, System.Dynamic.BindingRestrictions restrictions) : base(expression, restrictions) { }

        public DynamicRowMetaObject(System.Linq.Expressions.Expression expression, System.Dynamic.BindingRestrictions restrictions, object value) : base(expression, restrictions, value) { }

        private System.Dynamic.DynamicMetaObject CallMethod(MethodInfo method, System.Linq.Expressions.Expression[] parameters) {
            var callMethod = new System.Dynamic.DynamicMetaObject(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Convert(Expression, LimitType), method, parameters), System.Dynamic.BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            return callMethod;
        }

        public override System.Dynamic.DynamicMetaObject BindGetMember(System.Dynamic.GetMemberBinder binder) {
            var parameters = new System.Linq.Expressions.Expression[]
                                 {
                                         System.Linq.Expressions.Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        // Necesario para el soporte en Visual Basic
        public override System.Dynamic.DynamicMetaObject BindInvokeMember(System.Dynamic.InvokeMemberBinder binder, System.Dynamic.DynamicMetaObject[] args) {
            var parameters = new System.Linq.Expressions.Expression[]
                                 {
                                         System.Linq.Expressions.Expression.Constant(binder.Name)
                                 };

            var callMethod = CallMethod(getValueMethod, parameters);

            return callMethod;
        }

        public override System.Dynamic.DynamicMetaObject BindSetMember(System.Dynamic.SetMemberBinder binder, System.Dynamic.DynamicMetaObject value) {
            var parameters = new System.Linq.Expressions.Expression[]
                                 {
                                         System.Linq.Expressions.Expression.Constant(binder.Name),
                                         value.Expression,
                                 };

            var callMethod = CallMethod(setValueMethod, parameters);

            return callMethod;
        }
    }

    #endregion
}
