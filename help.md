# USAGE GUIDE

## 1. Entity Framework Migrations

```
make <ServiceName>-ef-list                 # List EF migrations
make <ServiceName>-ef-add name=MyMigration # Add a new EF migration
make <ServiceName>-ef-update               # Apply EF migrations to database
make <ServiceName>-ef-remove               # Remove the latest EF migration
```

**Shortcuts:**

```
make <short>-e-l                           # List migrations (short)
make <short>-e-a name=MyMigration          # Add migration (short)
make <short>-e-u                           # Update DB (short)
make <short>-e-r                           # Remove migration (short)
```

---

## 2. Running Microservices

```
make run-<ServiceName>                     # Run service
make run-w-<ServiceName>                   # Watch-run service (auto reload)
make r-<short>                             # Run service (short)
make rw-<short>                            # Watch-run service (short)

make run-all                               # Run all services in background
```

---

## 3. Testing

```
make test-<ServiceName>                    # Run unit tests for a service
make test-ci                               # Run GitHub Actions workflow locally using act-cli
```

---

## 4. Kafka Management

```
make list-topics                           # List all Kafka topics
make describe-topic                        # Describe a Kafka topic
make delete-topic                          # Delete a Kafka topic
make delete-all-topics                     # Delete all Kafka topics
make create-topic                          # Create a Kafka topic

make send-event                            # Send Base64-encoded payload to topic
make read-topic                            # Read from a topic (can use READ_ARGS=...)
```

---

## 5. Kubernetes & Skaffold

```
make up                                    # Start services (skaffold)
make up-clusters                           # Start clusters only
make up-dbs                                # Start DBs only

make down                                  # Tear down services
make down-clusters                         # Tear down clusters only
make down-dbs                              # Tear down DBs only
make down-all                              # Tear down all (services + dbs + clusters)
```

---

## 6. Kubernetes Secrets per Service

```
make s-gen-<short>                         # Generate backend secret YAML from .env
make s-update-<short>                      # Apply secret
make s-read-<short>                        # Read secret from Kubernetes
make s-del-<short>                         # Delete secret
```

---

## 7. Postgres DB Secrets

```
make gen-<short>-pgs                       # Create Postgres credentials secret for service
```

---

## 8. Postgres Commands

```
make db-check-role-<short>                # Check DB role (leader/follower)
make db-con-<short>                       # Connect to DB via kubectl
make db-tables-<short>                    # List DB tables
make db-seed-<short>                      # Run seed job (K8s job)
```

---

## Notes
- `<ServiceName>` is case-sensitive and must match the actual directory name.
- `<short>` is the defined short name for each service:

| Service     | Short Key |
|-------------|-----------|
| Auth        | au        |
| Reviews     | rev       |
| Orders      | ord       |
| Restaurants | rest      |
| Payments    | pay       |
| Locations   | loc       |
| Gateway     | gat       |