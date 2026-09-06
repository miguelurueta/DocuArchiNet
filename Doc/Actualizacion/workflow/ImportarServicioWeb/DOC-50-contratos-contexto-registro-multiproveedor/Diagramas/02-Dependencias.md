# Dependencias

```mermaid
flowchart TB
  Infra[Infraestructura futura] --> DTOs
  Infra --> Services
  Services --> DTOs
  Services --> Modelo
  Providers[Adaptadores futuros] --> Modelo
  Modelo --> BCL[.NET BCL]
  DTOs --> BCL
```
