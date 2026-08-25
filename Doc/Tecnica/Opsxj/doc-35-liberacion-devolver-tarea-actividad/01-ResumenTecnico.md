# LIBERACION-DEVOLVER-TAREA-ACTIVIDAD

- Ticket: DOC-35
- Cambio OpenSpec: doc-35-liberacion-devolver-tarea-actividad
- Clasificacion: cross_cutting

## Objetivo

Preparar una liberación futura y reversible de Devolver a actividad anterior. La decisión actual es solicitar aprobación operativa: DOC-34 aporta la evidencia técnica y el merge del PR #29 aporta la versión de referencia, pero no hay ambiente, ventana ni responsables autorizados.

## Alcance y compatibilidad

- Se incorporan exclusivamente documentos de decisión, matriz, runbook, compatibilidad y riesgos.
- La capacidad preservada incluye Webworkflow.aspx, los endpoints de preview y ejecución, ServicioDevolverActividad, el repositorio de conectores entrantes y los scripts de devolución.
- No se modifica código, configuración, contratos, datos, auditoría ni una tarea Workflow.
- Toda reversión futura se hará por paquete mediante la gestión de despliegue aprobada y solo afectará intentos nuevos.
