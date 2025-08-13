## Overview
SaaS application designed to help restaurants manage customers orders and monitor delivery progress in real time.

## Services
- Auth: manages user, user roles, roles permissions and authentication system.
- Gateway: as its name a gateway to route requests to the other services.
- Locations: manages the Delivery location.
- Orders: manages the customers food orders.
- Payments: manages the customers payments.
- Restaurants: manages the restaurants, menu's and menu items.
- Notifications: mailing service.
- Shared: this is a ClassLibrary meant to have shared logic across multiple microservices.

## Folder Structure
**baseline/template:**
```
Service/
├── Config/ # Any configs, usually they are taken from appsettings.json
├── Data/ # EF Core DbContext, migrations, seed data
├── Dtos/
├── Endpoints/ # Minimal API endpoints
├── Enums/ # Enums
├── Extensions/ # Service Extensions
├── Infra/ # yml files for kubernetes and docker files
├── Mappers/ # Manual Mappers using extensions
├── Middlewares/ # Service middlewares
├── Migrations/
├── Models/ # Service entities
├── Properties/
├── Repositories/
    └── IRepositories/ # Interfaces
├── Sagas/ # anything related to sagas (FailuresObservers / EventsConsumers / etc)
├── Services/
    └── IServices/ # Interfaces
├── Specifications/ # Service specifications
├── Utils/ # in case its needed we can have utilities here
└── Validation/
```

## Make Commands
[See the documentation](./help.md).