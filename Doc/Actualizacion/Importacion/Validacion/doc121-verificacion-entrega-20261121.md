# DOC-121 — verificación de entrega de la fase 01

Cambio `doc-121-importacion-excel-01-esquema-contratos-configuracion`, esquema spec-driven. Se aplicó openspec-verify-change sobre propuesta, diseño, especificación, tareas y entregables existentes. [Resultado estructurado](doc121-verificacion-entrega-20261121.json).

## Dictamen de alcance

**Fase 01 contractual completa: 16/16 tareas y 8/8 requisitos cubiertos por sus entregables.** Los 24 escenarios fueron revisados como contratos; no equivalen a 24 ejecuciones de aplicación. No quedan defectos críticos de esta entrega identificados por la revisión. Las fases 02–07 implementan autorización/conexión, lectura, recepción/preparación, confirmación, interfaz y aceptación real.

El usuario aclaró **«egresos solo es de pruebas»**. No corresponde modificar esa tabla como condición de terminar DOC-121. La exigencia de clave única se aplica cuando se selecciona una tabla como destino de carga o el módulo la crea. Las incidencias previamente observadas no se borran de la evidencia: describen la elegibilidad de aquella tabla, no un bloqueo general del módulo. EGRESOS sigue intacto por estas operaciones.

La plantilla SQL 05 y el contrato de cuenta dedicada están entregados. Su cuenta/host/DA y sus concesiones se configuran y comprueban al implementar la fase de conexión. No se declaran acreditados por la terminación contractual, y no impiden entregar los artefactos de fase 01.

## Completitud, corrección y coherencia

| Requisito | Entregable revisado | Evidencia y límite |
|---|---|---|
| D-01/RQ-01 | Inventario de catálogos/consumidores y SQL 02 aditivo. | Tipos y ordinales de instalación base verificados; código de consumidores conservado. No certifica regresión SII real. |
| D-02/RQ-02 | Guía, scripts separados, preflight/postflight y autorización posterior de 08. | Entrega manual como regla general; excepción expresa «aplica por mi» limitada a CONTRATOS y documentada. |
| D-03/RQ-03 | SQL 03, una tabla InnoDB de 22 columnas. | Tipos/nulabilidad/índices contrastados en la inspección base 01–03. Estados y transacciones del escritor se implementan después. |
| D-04/RQ-04 | SQL 01, plantilla 05 y configuración/autoridad de servidor. | Columna de permiso verificada; cuenta dedicada y permiso efectivo de usuario no certificados todavía. |
| D-05/RQ-05 | Procedimiento de adecuación y validación de clave del destino elegido. | ALTER 08 de CONTRATOS aplicado y verificado; EGRESOS de pruebas no es adecuación obligatoria. |
| D-06/RQ-06 | Request/response, identidad, multipart y revisión integral. | Contrato concreto para fases posteriores; endpoints y lector aún no implementados en DOC-121. |
| D-07/RQ-07 | Confirmación idempotente, DML atómico y CREATE separado. | Contrato revisado; rollback de carga, respuesta perdida y concurrencia se acreditan en las fases ejecutables. |
| D-08/RQ-08 | Auditor, matriz y evidencia diferenciada por corte. | 27 controles estáticos y trazabilidad; no atribuir al auditor conocimiento de la instalación actual. |

El auditor estático se corrigió para informar `NOT_EVALUATED_BY_STATIC_AUDIT` en lugar de emitir siempre PENDIENTE_EJECUCION_MANUAL. Sus contadores cero corresponden a lo que hace esa auditoría, no niegan el ALTER autorizado y registrado en un corte distinto. No se reescriben las evidencias históricas del 20/11.

## Validación utilizada

- Auditor local del paquete base 00–06: 27/27 controles aprobados, incluyendo identidad de las 195 entradas de ejecución DOC-120.
- Suites existentes de refinement/gobierno: 19/19 del corte anterior reutilizadas, sin cambios en ese tooling.
- OpenSpec estricto y refinement: contratos válidos y 15/15 controles de trazabilidad aprobados.
- Instalación base: 8 SELECT y 37 comprobaciones en el ambiente físico inspeccionado, según su primario.
- CONTRATOS: 08 aplicado por autorización expresa; un ALTER, 15 SELECT y un ajuste estricto solo de sesión. 625 filas y huella de las cuatro columnas preservadas, NOT NULL/UNIQUE completo verificados.
- Esta corrección de alcance no ejecutó nuevas consultas de base, DDL, DML o HTTP de aplicación. No se modificó EGRESOS ni se concedieron privilegios.

## Dependencias de las fases siguientes

La fase 02 deberá implementar y acreditar la resolución real RA/DA, cuenta dedicada y permiso propio en cada solicitud. Las fases posteriores comprobarán XLS/XLSX, límites, memoria/tiempos, rollback, concurrencia e interfaz con la infraestructura E2E existente y autorización de ambiente/cuentas cuando corresponda. Son verificaciones futuras conocidas, no tareas incompletas escondidas de DOC-121.

La entrega queda preparada para revisión por PR. No se ejecuta archivo OpenSpec, merge o cierre Jira por corregir este seguimiento.
