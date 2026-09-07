# Componentes

```mermaid
flowchart LR
 P[Preflight] --> Plan[Plan tipado]
 I[Servicio intención] --> G[Guard]
 I --> R[Repositorio]
 R --> DB[(Tablas modernas)]
```
