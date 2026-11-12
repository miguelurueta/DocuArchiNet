# DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Estado de esta entrega: refinamiento propuesto; sin implementación ni aceptación operativa.

## Objetivo

Acreditar seguimiento existente SEG-01/02 antes de reorganizar UI-01–05. Mantener SEG-03 condicionado a una decisión expresa de operación/retención. Fuente Jira conservada en el cambio OpenSpec.

## Alcance y compatibilidad

Diez decisiones y diez requisitos propuestos; inventario concreto en design.md. El mapa ya se aplicó a tasks.md: 24 tareas (10 S, 14 M), cada una con área, origen D/RQ y verificación, con 0/24 completadas. plan-tareas-propuesto.md conserva la procedencia; tasks.md es la fuente del plan y su ejecución. El refinamiento continúa en borrador.

Hallazgos de inspección: cliente no envía filtros admitidos por servicio; contextual usa selección en lugar de nodo pulsado; menú Importación nace del host administrativo. Reproducir antes de corregir producto. Mantener contratos/servicios/asistente, recuperación por IDs, autoridad y efectos confirmados. Sin esquema nuevo, SII, almacenamiento compartido, workers, limpieza ni despliegue.

La reversa futura de navegación conserva referencias/datos/efectos; nunca revierte lotes o elimina staging. Esta entrega solo modifica planificación/documentación.
