# Metadata

| Campo | Valor |
| --- | --- |
| Ticket | DOC-50 |
| Cambio OpenSpec | `doc-50-contratos-multi-proveedor` |
| Rama | `feature/DOC-50` |
| Fecha | 2026-09-06 |
| Estado | Implementación y verificación completas; listo para revisión |
| Versión contractual | `1.0` |
| Prompt | Backend 01 — contratos, contexto y registro multiproveedor |
| Fuente canónica | `Doc/Actualizacion/workflow/ImportarServicioWeb/CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md` |

Archivos entregados: seis VB en DTOs/Modelo/Services, tres suites CommonJS, ocho fixtures JSON, entradas del proyecto, artefactos OpenSpec y este paquete técnico.

Dependencias: BCL de .NET Framework 4.6.1 y puertos inyectables. No se agregaron paquetes, clientes HTTP, acceso a sesión ni persistencia.

Riesgos: solo `INTEGRACIONSII` está comprobado estáticamente; consumidores futuros podrían divergir de fixtures; publicar formas futuras no implica ejecución disponible. Deuda: adaptadores ASMX/proveedor, fuentes autorizadas concretas, preview mediado, persistencia e idempotencia de intenciones, concurrencia, reconciliación y timeouts efectivos.
