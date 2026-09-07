<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-52-extender-contratos

## Fuente y alcance

- Ticket: `DOC-52` — preflight, intención persistida e idempotencia.
- Dependencias: contratos DOC-50 y transporte DOC-51.
- Alcance: contratos extendidos, preflight sin efectos, intención persistida, concurrencia, SQL manual, pruebas y documentación.
- Exclusiones: ejecución de intención, proveedor SII concreto, ASMX, Session, caché y almacenamiento legacy.

## Contexto inspeccionado

- DTO, modelos e interfaces de `ImportarServicioWeb` ya contienen contratos base de preflight/intención.
- `ValidadorContextoImportacion` revalida identidad, permiso, tarea, ruta, trámite y proveedor sin mutar.
- `AdoNetDataExecutor`, `ModuleConnectionFactory` y repositorios modernos establecen inyección, transacción y parámetros.
- La exploración B-07/B-08/B-09 evidencia SQL concatenado legacy, contexto tomado del primer elemento y acoplamiento de cardinalidad.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Extender solo los tres archivos canónicos con snapshot inmutable, requisitos, elementos, estados y resultados. | DTO/modelos/interfaces DOC-50 | D-01 | RQ-01 | 1.1-1.3 |
| D-02 | Preflight revalida contexto y cada selección, genera el mismo plan para uno o varios elementos y no escribe. | `ValidadorContextoImportacion.vb`; B-09 | D-02 | RQ-02 | 2.1-2.3 |
| D-03 | La identidad externa genérica es `providerId + externalKey`; la semántica interna SII queda para su adaptador. | exploración, pregunta 3 | D-03 | RQ-03 | 1.2, 2.2 |
| D-04 | La equivalencia se calcula con SHA-256 de representación canónica ordenada de contexto, selección y requisitos. | requisito idempotencia | D-04 | RQ-04 | 3.1, 3.2 |
| D-05 | Crear/reutilizar intención dentro de transacción local con clave única y conflicto explícito si cambia la huella. | infraestructura ADO.NET; preguntas 4/5 | D-05 | RQ-05 | 3.3, 4.1-4.3 |
| D-06 | El guard coordina por idempotency key/contexto, pero la unicidad persistente es la garantía final ante carreras. | guards modernos; aceptación concurrente | D-06 | RQ-06 | 4.4, 5.3 |
| D-07 | SQL nuevo es parametrizado y usa tablas modernas propias; el script/rollback es manual y no toca caché legacy. | B-07/B-08 | D-07 | RQ-07 | 4.1-4.3, 6.1 |
| D-08 | Estado inicial `Creada`, elementos propios y auditoría mínima con operación/correlación sin datos sensibles. | modelo de fases; pregunta 8 | D-08 | RQ-08 | 3.3, 4.2, 5.2 |
| D-09 | Pruebas son focales con dobles ADO.NET y fixtures; E2E no aplica porque no hay endpoint/UI ni migración aplicada. | alcance DOC-52 | D-09 | RQ-09 | 5.1-5.4, 6.2-6.4 |

## Requisitos verificables

| ID | Resultado observable | Criterio | Riesgo/compatibilidad |
|---|---|---|---|
| RQ-01 | Contratos representan snapshot, plan e intención completa. | Build y pruebas contractuales. | Extensión aditiva v1. |
| RQ-02 | Preflight valida sin ejecutar escrituras. | Dobles fallan si se invoca persistencia/almacenamiento. | Legacy intacto. |
| RQ-03 | Cada elemento conserva proveedor, clave y tarea destino explícitos. | Individual/múltiple difieren solo en cantidad. | Sin inferir del primer elemento. |
| RQ-04 | Solicitudes equivalentes producen igual huella; cualquier cambio autoritativo produce otra. | Fixtures equivalente/conflicto. | Orden de entrada normalizado. |
| RQ-05 | Repetición equivalente reutiliza; incompatible retorna `IDEMPOTENCY_CONFLICT`. | Una sola intención ejecutable. | Transacción local. |
| RQ-06 | Dos creaciones concurrentes no duplican intención. | Simulación de colisión/unique key. | Guard no reemplaza constraint. |
| RQ-07 | Toda consulta usa parámetros y esquema nuevo reversible. | Auditoría fuente/SQL. | Aplicación manual autorizada. |
| RQ-08 | Intención guarda operación, contexto, requisitos, versión, fechas, correlación y estados por elemento. | Lectura round-trip. | Sin payload/secretos. |
| RQ-09 | Suite y build pasan sin red, DB real, E2E ni gate. | Evidencia reproducible. | Sin mutaciones ambientales. |

## Resultado

- Refinamiento aprobado para implementación.
- Documentación canónica: `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-52-preflight-intencion-idempotencia/`.
- El script SQL será entregable manual con precondiciones y rollback; no será ejecutado por el flujo.
