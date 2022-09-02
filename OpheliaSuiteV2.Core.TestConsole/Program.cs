using OpheliaSuiteV2.Core.DataAccess.MongoDb;

using System;

namespace OpheliaSuiteV2.Core.TestConsole {
    class Program {
        static void Main(string[] args) {
            DbClient dbClient = new ("mongodb://localhost:27017");
            DbContext dbContext = new ExampleDbContext(dbClient);
            GenericRepository<AuditMessageProject> repo1 = new(dbContext);
            GenericRepository<AuditMessageProject> repo2 = new(dbContext, "AuditMessage_OPH");

            AuditMessageProject message = new() { Name = $"Mensaje de auditoria: {DateTime.Now}" };

            repo1.Add(message);
            repo2.Add(message);

            Console.WriteLine("Ya!");
            Console.ReadKey();
        }
    }

    public class ExampleDbContext : DbContext {
        public ExampleDbContext(IDbClient client): base(client, "Example") {

        }
    }

    public class GenericRepository<TEntity> : Repository<TEntity> where TEntity : EntityBase, new() {
        public GenericRepository(string collectionName = null) : base(collectionName) { }
        public GenericRepository(IDbContext context, string collectionName = null) : base(context, collectionName) { }
    }

    public class AuditMessageProject: EntityBase {
        public string Name { get; set; }
    }
}
