var builder = DistributedApplication.CreateBuilder(args);

// add seq logging
var seq = builder.AddSeq("seq")
                 .WithEndpoint(port: 5341, targetPort: 5341, name: "seq")
                 .WithEnvironment("ACCEPT_EULA", "Y")
                 .WithEnvironment("SEQ_LOG_LEVEL", "Verbose")
                 .WithLifetime(ContainerLifetime.Persistent);


// add postgres database
var username = builder.AddParameter(name: "username", value: "postgres");
var password = builder.AddParameter(name: "password", value: "123456");
var postgres = builder.AddPostgres("postgres-db", username, password, port: 5432)
                        .WithDataVolume()
                        .WithExternalHttpEndpoints()
                        .WithPgAdmin();
var postgresdb = postgres.AddDatabase(name: "CompanyDB", databaseName: "CompanyDB");


// add api with all dependencies
var api = builder.AddProject<Projects.Api>("company-api")
                 .WithHttpEndpoint(port: 5104, name: "company-api-http")
                 .WithHttpsEndpoint(port: 7194, name: "company-api-https")
                 .WithReference(seq)
                 .WithReference(postgresdb)
                 .WaitFor(seq)
                 .WaitFor(postgresdb);

builder.Build().Run();
