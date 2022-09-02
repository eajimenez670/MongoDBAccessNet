using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace OpheliaSuiteV2.Core.DataAccess.MongoDb
{
    /// <summary>
    /// Encapsula los filtros a usar para una consulta
    /// </summary>
    public class Filter<TEntity> where TEntity : class, new()
    {
        #region Properties

        /// <summary>
        /// Filtros usados
        /// </summary>
        internal FilterDefinition<TEntity> InternalFilter;
        /// <summary>
        /// Tipo de ordenamiento
        /// </summary>
        internal SortDefinition<TEntity> InternalSort;

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// </summary>
        public Filter() { }

        #endregion

        #region Methods

        /// <summary>
        /// Agrega un condición de igualación al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Eq(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Eq(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición negada de igualación al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndNotEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Ne(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Ne(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MAYOR QUE al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndGt<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Gt(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Gt(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MENOR QUE al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndLt<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Lt(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Lt(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MAYOR OR IGUAL QUE al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndGtEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Gte(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Gte(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MENOR OR IGUAL QUE al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndLtEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Lte(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Lte(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición TEXT al filtro unida por un operador And
        /// </summary>
        /// <param name="text">Texto a buscar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndText(string text)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Text(text);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Text(text));

            return this;
        }

        /// <summary>
        /// Agrega un condición IN al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="values">Valores del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndIn<TField>(string fieldName, params TField[] values)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.In(fieldName, values);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.In(fieldName, values));

            return this;
        }

        /// <summary>
        /// Agrega un condición NOT IN al filtro unida por un operador And
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="values">Valores del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> AndNotIn<TField>(string fieldName, params TField[] values)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Nin(fieldName, values);
            else
                InternalFilter = Builders<TEntity>.Filter.And(InternalFilter, Builders<TEntity>.Filter.Nin(fieldName, values));

            return this;
        }

        /// <summary>
        /// Agrega un condición de igualación al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Eq(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Eq(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición TEXT al filtro unida por un operador And
        /// </summary>
        /// <param name="text">Texto a buscar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrText(string text)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Text(text);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Text(text));

            return this;
        }

        /// <summary>
        /// Agrega un condición negada de igualación al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrNotEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Ne(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Ne(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MAYOR QUE al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrGt<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Gt(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Gt(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MENOR QUE al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrLt<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Lt(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Lt(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MAYOR OR IGUAL QUE al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrGtEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Gte(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Gte(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición MENOR OR IGUAL QUE al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="value">Valor del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrLtEq<TField>(string fieldName, TField value)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Lte(fieldName, value);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Lte(fieldName, value));

            return this;
        }

        /// <summary>
        /// Agrega un condición IN al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="values">Valores del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrIn<TField>(string fieldName, params TField[] values)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.In(fieldName, values);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.In(fieldName, values));

            return this;
        }

        /// <summary>
        /// Agrega un condición NOT IN al filtro unida por un operador Or
        /// </summary>
        /// <typeparam name="TField">Tipo de dato del campo a igualar</typeparam>
        /// <param name="fieldName">Nombre del campo a igualar</param>
        /// <param name="values">Valores del campo a igualar</param>
        /// <returns>Filtro resultado</returns>
        public Filter<TEntity> OrNotIn<TField>(string fieldName, params TField[] values)
        {
            if (InternalFilter == null)
                InternalFilter = Builders<TEntity>.Filter.Nin(fieldName, values);
            else
                InternalFilter = Builders<TEntity>.Filter.Or(InternalFilter, Builders<TEntity>.Filter.Nin(fieldName, values));

            return this;
        }

        #endregion

        #region Factories

        /// <summary>
        /// Crea un filtro a partir de una expresión
        /// </summary>
        /// <param name="expression">Expresión a usar para crear el filtro</param>
        /// <returns>Filtro creado</returns>
        public static Filter<TEntity> Create(FilterExpression<TEntity> expression)
        {
            expression = expression ?? throw new ArgumentNullException(nameof(expression));
            return expression.BuildFilter();
        }

        /// <summary>
        /// Crea un filtro a partir de una definición
        /// </summary>
        /// <param name="definition">Definición a usar para crear el filtro</param>
        /// <returns>Filtro creado</returns>
        public static Filter<TEntity> Create(FilterDefinition<TEntity> definition) {
            definition = definition ?? throw new ArgumentNullException(nameof(definition));
            return new Filter<TEntity> { InternalFilter = definition };
        }

        #endregion
    }

    /// <summary>
    /// Representa la construcción de filtros basados en una expresión
    /// </summary>
    public sealed class FilterExpression<TEntity> where TEntity : class, new()
    {
        #region Properties

        /// <summary>
        /// Campo y tipo de  ordenamiento del filtro
        /// </summary>
        public string OrderByField { get; private set; }
        /// <summary>
        /// Expresión con el filtro para aplicar
        /// </summary>
        public string QueryParams { get; private set; }

        #endregion

        #region Const

        /// <summary>
        /// Corresponde al separador de ordenamiento
        /// </summary>
        private const char SORTING_SEPARAT_OR = '_';
        /// <summary>
        /// Corresponde al separador de filtros de campos para
        /// una operación logica and
        /// </summary>
        private const char PARAMS_SEPARATOR_AND = '&';
        /// <summary>
        /// Separador campo valor de un filtro especifico
        /// </summary>
        private const char FIELD_SEPARATOR = '=';
        /// <summary>
        /// Corresponde a la expresión que se va evaluar para la 
        /// validación de filtrosm
        /// </summary>
        private const string EXPRESSION_EVALUATE = "^([a-zA-Z0-9_]+_(eq|gt|gteq|in|lt|lteq|noteq|notin|text)=([a-zA-Z0-9 _.#-]|\\([a-zA-Z0-9 _'\",.#-]+,*\\))*(&|\\|)*)+$";
        /// <summary>
        /// Corresponde al tipo de filtro de cadena
        /// </summary>
        private const string STRING_EXPRESSION = "^\\((('[a-zA-Z0-9 _+,;.:!\"#$%&/\\(\\)=?¡¿*{}\\[\\]|@<>-]*'|#),*)+\\)$";
        /// <summary>
        /// Corresponde a un filtro de tipo entero
        /// </summary>
        private const string NUMERIC_EXPRESSION = "^\\(([0-9]+(\\.[0-9]+)*,*)+\\)$";

        #endregion

        #region Builders

        /// <summary>
        /// Inicializa los valores por defecto de ordenamiento y el query
        /// </summary>
        /// <param name="orderByField"></param>
        /// <param name="queryParams"></param>
        public FilterExpression(string orderByField, string queryParams)
        {
            OrderByField = orderByField?.Trim() ?? string.Empty;
            QueryParams = queryParams?.Trim() ?? string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Construir objeto de tipo Filter basado en una expresión 
        /// </summary>
        /// <returns></returns>
        internal Filter<TEntity> BuildFilter()
        {
            Filter<TEntity> filter = new Filter<TEntity>();

            //TODO Validar Expresion...
            if (!Regex.IsMatch(QueryParams, EXPRESSION_EVALUATE, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
                throw new InvalidFilterExpressionException(QueryParams);

            //Constuir filtro basado en un Query
            if (!string.IsNullOrEmpty(QueryParams))
            {
                char[] separators = new char[] { '&', '|' };
                string[] queryParams = QueryParams.Split(separators);
                if (queryParams != null)
                {
                    char connector = '&';
                    int filterLength = 0;
                    foreach (string filterParam in queryParams)
                    {
                        string[] queryValue = filterParam.Split(FIELD_SEPARATOR);
                        string[] queryField = queryValue[0].Split(SORTING_SEPARAT_OR);
                        BuildQueryParams(ref filter, queryField[0], queryField[1].ToLower(), queryValue[1], connector);
                        filterLength += queryValue[0].Length + queryValue[1].Length + 1;
                        connector = filterLength >= QueryParams.Length
                            ? connector
                            : Convert.ToChar(QueryParams.Substring(filterLength, 1));
                        filterLength += 1;
                    }
                }
            }
            //Asignar ordenamiento especifico
            if (!string.IsNullOrEmpty(OrderByField))
                filter.InternalSort = BuildSort();

            return filter;
        }

        /// <summary>
        /// Responsable de construir el ordenamiento
        /// </summary>
        /// <returns>Ordenamiento definido</returns>
        private SortDefinition<TEntity> BuildSort()
        {
            //Obtener información del ordenamiento
            string[] sortingParams = OrderByField.Split(SORTING_SEPARAT_OR);
            string sortingField = sortingParams[0];
            string sortingType = sortingParams[1].ToLower();
            //Constuir ordenamiento especifico
            SortDefinitionBuilder<TEntity> builder = Builders<TEntity>.Sort;
            SortDefinition<TEntity> sort = sortingType.Equals("asc")
                ? builder.Ascending(sortingField)
                : builder.Descending(sortingField);
            return sort;
        }

        /// <summary>
        /// Encargado de construir el filtro de acuerdo al 
        /// tipo de operador
        /// </summary>
        /// <param name="filter">Información del filtro a contruir</param>
        /// <param name="field">Nombre del campo por el que se va filtrar</param>
        /// <param name="operatorType">Tipo de operador a contruir</param>
        /// <param name="value">Valor del campo  que se va filtrar</param>
        /// <param name="connector">operador logico para la comparación</param>
        private void BuildQueryParams(ref Filter<TEntity> filter, string field, string operatorType, string value, char connector)
        {
            switch (operatorType)
            {
                case "eq":
                    value = value?.Trim() == "#" ? null : value;
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndEq(field, value);
                    else
                        filter.OrEq(field, value);
                    break;
                case "gt":
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndGt(field, value);
                    else
                        filter.OrGt(field, value);
                    break;
                case "gteq":
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndGtEq(field, value);
                    else
                        filter.OrGtEq(field, value);
                    break;
                case "in":
                    if (Regex.IsMatch(value, STRING_EXPRESSION)) //Si es cadena
                    {
                        List<string> vals = new List<string>();
                        foreach (string v in value.Substring(1, value.Length - 2).Replace("'", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            vals.Add(v?.Trim() == "#" ? null : v);

                        if (connector.Equals(PARAMS_SEPARATOR_AND))
                            filter.AndIn(field, vals.ToArray());
                        else
                            filter.OrIn(field, vals.ToArray());
                    }
                    else if (Regex.IsMatch(value, NUMERIC_EXPRESSION)) //Si es numero
                    {
                        List<double> vals = new List<double>();
                        foreach (string v in value.Substring(1, value.Length - 2).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            vals.Add(double.Parse(v, CultureInfo.CreateSpecificCulture("en-US")));

                        if (connector.Equals(PARAMS_SEPARATOR_AND))
                            filter.AndIn(field, vals.ToArray());
                        else
                            filter.OrIn(field, vals.ToArray());
                    }
                    else
                        throw new InvalidFilterExpressionException(QueryParams);
                    break;
                case "lt":
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndLt(field, value);
                    else
                        filter.OrLt(field, value);
                    break;
                case "lteq":
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndLtEq(field, value);
                    else
                        filter.OrLtEq(field, value);
                    break;
                case "noteq":
                    value = value?.Trim() == "#" ? null : value;
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndNotEq(field, value);
                    else
                        filter.OrNotEq(field, value);
                    break;
                case "notin":
                    if (Regex.IsMatch(value, STRING_EXPRESSION)) //Si es cadena
                    {
                        List<string> vals = new List<string>();
                        foreach (string v in value.Substring(1, value.Length - 2).Replace("'", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            vals.Add(v?.Trim() == "#" ? null : v);

                        if (connector.Equals(PARAMS_SEPARATOR_AND))
                            filter.AndNotIn(field, vals.ToArray());
                        else
                            filter.OrNotIn(field, vals.ToArray());
                    }
                    else if (Regex.IsMatch(value, NUMERIC_EXPRESSION)) //Si es numero
                    {
                        List<double> vals = new List<double>();
                        foreach (string v in value.Substring(1, value.Length - 2).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            vals.Add(double.Parse(v, CultureInfo.CreateSpecificCulture("en-US")));

                        if (connector.Equals(PARAMS_SEPARATOR_AND))
                            filter.AndNotIn(field, vals.ToArray());
                        else
                            filter.OrNotIn(field, vals.ToArray());
                    }
                    else
                        throw new InvalidFilterExpressionException(QueryParams);
                    break;
                case "text":
                    if (connector.Equals(PARAMS_SEPARATOR_AND))
                        filter.AndText(value);
                    else
                        filter.OrText(value);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
