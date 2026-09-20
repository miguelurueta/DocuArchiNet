# ACTUALIZACION-UTIL-CREA-EXPEDIENTE-SII

- Ticket: DOC-71
- Cambio OpenSpec: doc-71-actualizacion-util-crea-expediente-sii
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Separar la importación documental de la gestión de expedientes. La bandera autoritativa del trámite
selecciona `SinExpediente` o `GestionarExpediente`; la primera almacena y actualiza índices documentales
disponibles sin producir efectos físicos de expediente, y la segunda conserva DOC-67.

## Alcance y compatibilidad

- [x] No se modifican páginas ni controles. Se afectan exclusivamente modelos, repositorios,
  coordinadores y pruebas modernas de `ImportarServicioWeb`, más su adaptador E2E existente.
- [x] Se preservan el recorrido legacy, `AlmacenaDocumentoTareaWorkflow(...)`, la rama DOC-67 y el gate
  apagado. La reversa consiste en desplegar la versión anterior manteniendo el gate deshabilitado; no hay
  DDL nuevo ni reconstrucción histórica.
