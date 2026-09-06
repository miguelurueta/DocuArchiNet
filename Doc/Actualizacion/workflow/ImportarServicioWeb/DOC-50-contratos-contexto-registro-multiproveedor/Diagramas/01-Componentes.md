# Componentes

```mermaid
flowchart LR
  Future[ASMX futuro] --> DTO[DTOs v1]
  Future --> S[ServicioImportarServicioWeb]
  S --> V[ValidadorContextoImportacion]
  S --> R[RegistroProveedoresImportacion]
  V --> A[Puerto de autorización]
  R --> P[IExternalImportProvider]
  S --> M[Modelo y contexto inmutable]
```
