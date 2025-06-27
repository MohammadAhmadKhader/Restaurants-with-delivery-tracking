SERVICES := Auth Reviews Orders Restaurants Payments Locations
GATEWAY := Gateway

Auth_SHORT_KEY := au
Reviews_SHORT_KEY := rev
Orders_SHORT_KEY := ord
Restaurants_SHORT_KEY := rest
Payments_SHORT_KEY := pay
Locations_SHORT_KEY := loc
Gateway_SHORT_KEY := gat

PHONY := $(foreach S,$(SERVICES),$(S)-ef-list $(S)-ef-add $(S)-ef-update $(S)-ef-remove)
.PHONY: $(PHONY) list-topics describe-topic delete-topic create-topic

help:
	@echo "Usage:"
	@echo "1- Migrations:"
	@echo "   make <ServiceName>-ef-list        				# List migrations"
	@echo "   make <ServiceName>-ef-add name=MyMigration  		# Add a migration"
	@echo "   make <ServiceName>-ef-update      				# Apply migrations"
	@echo "   make <ServiceName>-ef-remove      				# Remove last migration"
	@echo "----------------------------------------------------------------------------------------"
	@echo "2- Running Services:"
	@echo "   make run-<ServiceName>      						# Run single service using its name"
	@echo "   make run-all      								# Run all microservices"
	@echo "----------------------------------------------------------------------------------------"
	@echo "3- Testing Services:"
	@echo "   make test-<ServiceName>      						# Test single service using its name"
	@echo "----------------------------------------------------------------------------------------"
	@echo "4- Testing Github Workflow:"
	@echo "   make test-ci      								# Testing github workflow locally using act-cli"
	@echo "----------------------------------------------------------------------------------------"
	@echo "Note: <ServiceName> is case insensitive"

# * ———————————————————————————— Migration commands ————————————————————————————
define EF_RULES
$(1)-ef-list:
	@dotnet ef migrations list --json --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

$(1)-ef-add:
	@if [ -z "$(name)" ]; then \
	  echo "Error: name is not set. Usage: make $(1)-ef-add name=MigrationName"; exit 1; \
	fi
	@dotnet ef migrations add "$(name)" --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

$(1)-ef-update:
	@dotnet ef database update --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

$(1)-ef-remove:
	@dotnet ef migrations remove --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

# * short commands <----------------------------------------------

$($(1)_SHORT_KEY)-e-l:
	@dotnet ef migrations list --json --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

$($(1)_SHORT_KEY)-e-a:
	@if [ -z "$(name)" ]; then \
	  echo "Error: name is not set. Usage: make $(1)-ef-add name=MigrationName"; exit 1; \
	fi
	@dotnet ef migrations add "$(name)" --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

$($(1)_SHORT_KEY)-e-u:
	@dotnet ef database update --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)

$($(1)_SHORT_KEY)-e-r:
	@dotnet ef migrations remove --project services/$(1)/$(1).csproj --startup-project services/$(1) $(ARGS)
endef

$(foreach S,$(SERVICES),$(eval $(call EF_RULES,$(S))))

# * ———————————————————————————— Local Running commands ————————————————————————————
#  make running command per service
define RUN_SERVICE
run-$(1):
	dotnet run --project services/$(1)/$(1).csproj $(ARGS)

run-w-$(1):
	dotnet watch run --project services/$(1)/$(1).csproj -- $(ARGS)

r-$($(1)_SHORT_KEY):
	dotnet watch run --project services/$(1)/$(1).csproj $(ARGS)

rw-$($(1)_SHORT_KEY):
	dotnet watch run --project services/$(1)/$(1).csproj -- $(ARGS)
endef

$(foreach S,$(SERVICES),$(eval $(call RUN_SERVICE,$(S))))
$(eval $(call RUN_SERVICE,Gateway))

run-all:
	@echo "Running all Microservices..."; \
	$(foreach svc,$(SERVICES),dotnet run --project services/$(svc)/$(svc).csproj & ) \
	dotnet run --project gateway/$(GATEWAY).csproj

# * ———————————————————————————— Testing Commands ————————————————————————————
# Testing github workflow locally
test-ci:
	act --secret-file .env.act --pull=false

#  make running testing per service
define TEST_SERVICE
test-$(1):
	dotnet test tests/$(1).Tests/$(1).Tests.csproj $(ARGS) --environment "ASPNETCORE_ENVIRONMENT=Testing"
endef
$(foreach S,$(SERVICES),$(eval $(call TEST_SERVICE,$(S))))

# * ———————————————————————————— Kafka Commands ————————————————————————————
NAMESPACE=kafka
CLUSTER=my-cluster
KAFKA_POD := $(shell kubectl get pods -n kafka -l strimzi.io/name=$(CLUSTER)-kafka -o jsonpath='{.items[0].metadata.name}')
KAFKA_CMD=kubectl exec -n kafka $(KAFKA_POD) -- sh -c
BOOTSTRAP=localhost:9094
TOPIC ?=test-topic
SCRIPTS_PATH=/opt/kafka/bin

list-topics:
	@echo "Listing Kafka topics..."
	@$(KAFKA_CMD) "$(SCRIPTS_PATH)/kafka-topics.sh --bootstrap-server $(BOOTSTRAP) --list"

describe-topic:
	@echo "Describing topic: $(TOPIC)"
	@$(KAFKA_CMD) "$(SCRIPTS_PATH)/kafka-topics.sh --bootstrap-server $(BOOTSTRAP) --describe --topic $(TOPIC)"

delete-topic:
	@echo "Deleting topic: $(TOPIC)"
	@$(KAFKA_CMD) "$(SCRIPTS_PATH)/kafka-topics.sh --bootstrap-server $(BOOTSTRAP) --delete --topic $(TOPIC)"
	
delete-all-topics:
	@echo "Deleting ALL Kafka topics..."
	@$(KAFKA_CMD) 'for t in $$($(SCRIPTS_PATH)/kafka-topics.sh --bootstrap-server $(BOOTSTRAP) --list); \
	do echo "Deleting $$t"; \
	$(SCRIPTS_PATH)/kafka-topics.sh --bootstrap-server $(BOOTSTRAP) --delete --topic $$t; done'

TOPIC_OPTIONS = --partitions 1 --replication-factor 1
create-topic:
	@echo "Creating topic: $(TOPIC)"
	$(KAFKA_CMD) "$(SCRIPTS_PATH)/kafka-topics.sh --bootstrap-server $(BOOTSTRAP) --create --topic $(TOPIC) $(TOPIC_OPTIONS)"

PAYLOAD ?= {"value":"hello world"}
send-event:
	@echo "Sending event to topic: $(TOPIC)" 
	@echo "Payload: $(PAYLOAD)" \
	$(KAFKA_CMD) "echo \"$(PAYLOAD)\" | $(SCRIPTS_PATH)/kafka-console-producer.sh --bootstrap-server $(BOOTSTRAP) --topic $(TOPIC)"

READ_ARGS ?= --from-beginning --timeout-ms 1000
read-topic:
	@echo "Reading all messages from topic: $(TOPIC)"
	$(KAFKA_CMD) "$(SCRIPTS_PATH)/kafka-console-consumer.sh --bootstrap-server $(BOOTSTRAP) --topic $(TOPIC) $(READ_ARGS)"

# * ———————————————————————————— Skaffold Commands ————————————————————————————
k8s-build-f:
	@skaffold dev --cache-artifacts=false