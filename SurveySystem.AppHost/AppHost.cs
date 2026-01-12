using Aspire.Hosting.Yarp;
using Aspire.Hosting.Yarp.Transforms;

var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIRECOMPUTE003
builder.AddContainerRegistry("docker-hub", "docker.io", "saragaber");
#pragma warning restore ASPIRECOMPUTE003

var k8s = builder.AddKubernetesEnvironment("k8s");
//var compose = builder.AddDockerComposeEnvironment("docker-env");

#region Catalog

var postgres = builder.AddPostgres("catalog-postgres-server");
var postgresdb = postgres.AddDatabase("catalog-postgres-db");

var migrationService = builder
    .AddProject<Projects.SurveySystem_Catalog_MigrationService>("catalog-migrationservice")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var catalogApi = builder.AddProject<Projects.SurveySystem_Catalog_API>("catalog-api")
    .WithReference(postgresdb)
    .WaitForCompletion(migrationService);

#endregion

#region Response

var responseMongo = builder
    .AddMongoDB("response-mongo-server")
    .WithMongoExpress();
var responseDb = responseMongo.AddDatabase("response-mongo-db");

var rabbitmq = builder.AddRabbitMQ("rabbit-mq");

var responseAPi = builder.AddProject<Projects.SurveySystem_Response_API>("response-api")
    .WithReference(responseDb)
    .WaitFor(responseDb)
    .WithReference(catalogApi)
    .WaitFor(catalogApi)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

#endregion

#region Reporting

var reportingDb = builder
    .AddRedis("reporting-redis-db")
    .WithRedisInsight();

var reportingApi = builder.AddProject<Projects.SurveySystem_Reporting_API>("reporting-api")
    .WithReference(reportingDb)
    .WaitFor(reportingDb)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

#endregion

#region Gateway

var gateway = builder.AddYarp("gateway")
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/catalog/{**catch-all}", catalogApi)
            .WithTransformPathRemovePrefix("/catalog")
            .WithMatchMethods("GET", "POST", "PUT", "DELETE");

        yarp.AddRoute("/responses/{**catch-all}", responseAPi)
            .WithTransformPathRemovePrefix("/responses")
            .WithMatchMethods("GET", "POST");

        yarp.AddRoute("/reports/{**catch-all}", reportingApi)
            .WithTransformPathRemovePrefix("/reports")
            .WithMatchMethods("GET");
    });

#endregion

builder.Build().Run();