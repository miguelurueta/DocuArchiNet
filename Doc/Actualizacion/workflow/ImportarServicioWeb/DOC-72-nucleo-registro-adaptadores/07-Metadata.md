# Metadata, versionado y reversa

| Campo | Valor |
| --- | --- |
| Ticket | DOC-72 |
| Cambio OpenSpec | `doc-72-nucleo-intrfaz-integracion-sii` |
| Repositorio | `DocuArchiNet` |
| Rama de elaboración | `feature/DOC-72` |
| Tecnología | ASP.NET WebForms VB.NET 4.6.1, JavaScript IIFE/CommonJS, CSS |
| Contrato DTO | `SchemaVersion = "1.0"` en solicitudes/respuestas base |
| Gate | `WorkflowCentroTrabajoModernActive` |
| Proveedor frontend | `ImportarServicioWebProviderId`, vacío por defecto |
| Reversa | gate falso, usuarios y grupos vacíos |
| Diagrama contractual | `diagram-contract.json`, versión 1 |

## Convenciones documentales

- `JS.*`: símbolo JavaScript resoluble por AST.
- `VB.*`: símbolo .NET resoluble por reflexión.
- `<<external>>`: actor o sistema fuera del repositorio; no se resuelve contra código.
- `<<conceptual>>`: estado/idea usada para explicar el flujo; no representa clase.
- Rutas siempre relativas a la raíz `DocuArchiNet`.

## Historial relevante

- Implementación base: commit `920f4555`.
- Reubicación del paquete DOC-72: `90661491`.
- Consolidación documental ImportarServicioWeb: `889b0f46`.
