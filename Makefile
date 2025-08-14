SERVICES := Auth Notifications Orders Restaurants Payments Locations
GATEWAY := Gateway

Auth_SHORT_KEY := au
Notifications_SHORT_KEY := not
Orders_SHORT_KEY := ord
Restaurants_SHORT_KEY := rest
Payments_SHORT_KEY := pay
Locations_SHORT_KEY := loc
Gateway_SHORT_KEY := gat

PHONY := $(foreach S,$(SERVICES),$(S)-ef-list $(S)-ef-add $(S)-ef-update $(S)-ef-remove)
.PHONY: $(PHONY) list-topics describe-topic delete-topic create-topic

help:
	@bash -c "grep -v '^---' ./help.md | sed 's/^## *//'  | tr -d '\`' |  awk 'NF {print; blank=0} !NF && !blank {print; blank=1}'"

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
$(eval $(call EF_RULES,Gateway))

# * ———————————————————————————— Local Running commands ————————————————————————————
#  make running command per service
define RUN_SERVICE
run-$(1):
	dotnet run --project services/$(1)/$(1).csproj $(ARGS)

run-b-$(1):
	dotnet build services/$(1)/$(1).csproj $(ARGS)

run-w-$(1):
	dotnet watch run --project services/$(1)/$(1).csproj -- $(ARGS)

r-$($(1)_SHORT_KEY):
	dotnet run --project services/$(1)/$(1).csproj $(ARGS)

rw-$($(1)_SHORT_KEY):
	dotnet watch run --project services/$(1)/$(1).csproj -- $(ARGS)

rb-$($(1)_SHORT_KEY):
	dotnet build services/$(1)/$(1).csproj $(ARGS)
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

t-$($(1)_SHORT_KEY):
	dotnet test tests/$(1).Tests/$(1).Tests.csproj $(ARGS) --environment "ASPNETCORE_ENVIRONMENT=Testing"
endef
$(foreach S,$(SERVICES),$(eval $(call TEST_SERVICE,$(S))))

# * ———————————————————————————— Kafka Commands ————————————————————————————
NAMESPACE=kafka
CLUSTER=my-cluster
KAFKA_POD := $(shell kubectl get pods -n kafka -l strimzi.io/name=$(CLUSTER)-kafka -o jsonpath='{.items[0].metadata.name}' 2>/dev/null)
KAFKA_CMD=kubectl exec -n $(NAMESPACE) $(KAFKA_POD) -- sh -c
BOOTSTRAP=localhost:9095
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

PAYLOAD ?={"value":"hello world"}
BASE64_PAYLOAD=$(shell echo '$(PAYLOAD)' | base64)
send-event:
	@echo "Sending event to topic: $(TOPIC)" 
	@echo "Payload: $(PAYLOAD)"
	@$(KAFKA_CMD) "echo $(BASE64_PAYLOAD) | base64 -d | $(SCRIPTS_PATH)/kafka-console-producer.sh --bootstrap-server $(BOOTSTRAP) --topic $(TOPIC)"

READ_ARGS ?= --from-beginning --timeout-ms 1000
read-topic:
	@echo "Reading all messages from topic: $(TOPIC)"
	$(KAFKA_CMD) "$(SCRIPTS_PATH)/kafka-console-consumer.sh --bootstrap-server $(BOOTSTRAP) --topic $(TOPIC) $(READ_ARGS)"

# * ———————————————————————————— K8S & Skaffold Commands ————————————————————————————
k8s-build-f:
	@skaffold run --cache-artifacts=false $(ARGS)

up-dev-clusters:
	@skaffold dev --filename ./skaffold-clusters.yml $(ARGS)

up-dev-dbs:
	@skaffold dev --filename ./skaffold-dbs.yml $(ARGS)

up-dev:
	@skaffold dev --filename ./skaffold.yml $(ARGS)

up-clusters:
	@skaffold run --filename ./skaffold-clusters.yml $(ARGS)

up-dbs:
	@skaffold run --filename ./skaffold-dbs.yml $(ARGS)

up:
	@skaffold run --filename ./skaffold.yml $(ARGS)

down:
	@skaffold delete --filename ./skaffold.yml $(ARGS)

down-clusters:
	@skaffold delete --filename ./skaffold-clusters.yml $(ARGS)

down-dbs:
	@skaffold delete --filename ./skaffold-dbs.yml $(ARGS)

down-all:
	@skaffold delete --filename ./skaffold.yml
	@skaffold delete --filename ./skaffold-clusters.yml -p all
	@skaffold delete --filename ./skaffold-dbs.yml


SECRETS_NAMESPACE ?=$(NAMESPACE)
DEFAULT_PG_USERNAME=postgres
DEFAULT_PG_PASSWORD=123456

define K8S_SECRETS
s-gen-$($(1)_SHORT_KEY):
	@kubectl create secret generic 'backend-$(shell echo $(1) | tr A-Z a-z)-secret' --from-env-file=./services/$(1)/.env.k8s \
	 --namespace=$(SECRETS_NAMESPACE) --dry-run=client \
	 -o yaml | sed '/creationTimestamp/d' > ./services/$(1)/Infra/backend-secrets.yml

s-del-$($(1)_SHORT_KEY):
	@kubectl delete -f ./services/$(1)/Infra/backend-secrets.yml

s-read-$($(1)_SHORT_KEY):
	@kubectl get secret backend-$(shell echo $(1) | tr A-Z a-z)-secret -o json | jq '.data | map_values(@base64d)' --namespace=$(SECRETS_NAMESPACE)

s-update-$($(1)_SHORT_KEY):
	@kubectl apply -f ./services/$(1)/Infra/backend-secrets.yml
endef

$(foreach S,$(SERVICES),$(eval $(call K8S_SECRETS,$(S))))

define PG_SECRETS
gen-$($(1)_SHORT_KEY)-pgs:
	@kubectl create secret generic pg-$(shell echo $(1) | tr A-Z a-z)-credentials \
		--namespace=$(SECRETS_NAMESPACE) \
		--type=kubernetes.io/basic-auth \
		--from-literal=username=$$(or $(PG_USERNAME),$(DEFAULT_PG_USERNAME)) \
		--from-literal=password=$$(or $(PG_PASSWORD),$(DEFAULT_PG_PASSWORD)) \
		--dry-run=client -o yaml | sed '/creationTimestamp/d' \
		> ./services/$(1)/Infra/pg-secrets.yaml
endef

$(foreach S,$(SERVICES),$(eval $(call PG_SECRETS,$(S))))

DB_NUM =1
# t => leader (read, write)
# f => follower
define DB_COMMANDS
db-check-role-$($(1)_SHORT_KEY):
	@kubectl exec -it $(shell echo $(1) | tr A-Z a-z)-db-$(DB_NUM) -n kafka -- psql -U postgres -c "SELECT pg_is_in_recovery();"

db-con-$($(1)_SHORT_KEY):
	@kubectl exec -it $(shell echo $(1) | tr A-Z a-z)-db-$(DB_NUM) -n $(NAMESPACE) -- psql -U postgres -d foodDelivery_$(shell echo $(1) | tr A-Z a-z)

db-tables-$($(1)_SHORT_KEY):
	@kubectl exec -it $(shell echo $(1) | tr A-Z a-z)-db-$(DB_NUM) -n $(NAMESPACE) -- psql -U postgres -d foodDelivery_$(shell echo $(1) | tr A-Z a-z) -c "\dt"

db-seed-$($(1)_SHORT_KEY):
	@kubectl apply -f ./services/$(1)/Infra/seed-job.yml
endef

$(foreach S,$(SERVICES),$(eval $(call DB_COMMANDS,$(S))))

POD=? ""
k-env:
	@kubectl exec -n $(NAMESPACE) $(POD) -- env

# * ———————————————————————————— Stripe Commands ————————————————————————————
init-stripe:
	stripe listen --forward-to http://localhost:5113/payments/webhook