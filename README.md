## Overview
SaaS application designed to help restaurants manage customers orders and monitor delivery progress in real time.

## Services
- Auth: manages user, user roles, roles permissions and authentication system.
- Gateway: as its name a gateway to route requests to the other services.
- Locations: manages the Delivery location.
- Orders: manages the customers food orders.
- Payments: manages the customers payments.
- Restaurants: manages the restaurants, menu's and menu items.
- Reviews: manages the restaurants reviews and menu items reviews.
- Shared: this is a ClassLibrary meant to have shared logic across multiple microservices.

## Folder Structure
```
Service/
├── Data/ # EF Core DbContext, migrations, seed data
├── Dtos/
├── Endpoints/ # Minimal API endpoints
├── Models/ # Service entities
├── Mirgations/
├── Properties/
├── Repositories/
└── Services/
```