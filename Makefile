SERVICES := Auth Reviews Orders Restaurants Payments Locations
GATEWAY := Gateway

PHONY := $(foreach S,$(SERVICES),$(S)-ef-list $(S)-ef-add $(S)-ef-update $(S)-ef-remove)
.PHONY: $(PHONY)

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
	@echo "Note: <ServiceName> is case insensitive"

# * ———————————————————————————— Migration commands ————————————————————————————
define EF_RULES
$(1)-ef-list:
	@dotnet ef migrations list --project services/$(1)/$(1).csproj --startup-project services/$(1)

$(1)-ef-add:
	@if [ -z "$(name)" ]; then \
	  echo "Error: name is not set. Usage: make $(1)-ef-add name=MigrationName"; exit 1; \
	fi
	@dotnet ef migrations add "$(name)" --project services/$(1)/$(1).csproj --startup-project services/$(1)

$(1)-ef-update:
	@dotnet ef database update --project services/$(1)/$(1).csproj --startup-project services/$(1)

$(1)-ef-remove:
	@dotnet ef migrations remove --project services/$(1)/$(1).csproj --startup-project services/$(1)
endef

$(foreach S,$(SERVICES),$(eval $(call EF_RULES,$(S))))

# * ———————————————————————————— Local Running commands ————————————————————————————
#  make command per service
define RUN_SERVICE
run-$(1):
	dotnet run --project services/$(1)/$(1).csproj
endef

$(foreach S,$(SERVICES),$(eval $(call RUN_SERVICE,$(S))))
$(eval $(call RUN_SERVICE,GATEWAY))

run-all:
	@echo "Running all Microservices..."; \
	$(foreach svc,$(SERVICES),dotnet run --project services/$(svc)/$(svc).csproj & ) \
	dotnet run --project gateway/$(GATEWAY).csproj