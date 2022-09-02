namespace OpheliaSuiteV2.Core.DataAccess.MongoDb {
    /// <summary>
    /// Encapsula datos de configuración que persisten
    /// durante la sesión actual
    /// </summary>
    public static class SessionConfig {
        /// <summary>
        /// Nombre de la estrategia de nombrado
        /// usada para serializar los nombres de las
        /// propiedades en las entidades
        /// </summary>
        public static NamingStrategyType NamingStrategy { get; set; } = NamingStrategyType.Default;
    }

    /// <summary>
    /// Tipo de estrategia de nombrado
    /// </summary>
    public enum NamingStrategyType {
        /// <summary>
        /// Default
        /// </summary>
        Default,
        /// <summary>
        /// Formato
        /// </summary>
        CamelCase
    }
}
