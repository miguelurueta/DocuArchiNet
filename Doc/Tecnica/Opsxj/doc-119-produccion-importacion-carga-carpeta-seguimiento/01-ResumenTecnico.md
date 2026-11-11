# DOC-119 — PRODUCCION-IMPORTACION-CARGA-CARPETA-SEGUIMIENTO

- Cambio: `doc-119-produccion-importacion-carga-carpeta-seguimiento`.
- Clasificación: `cross_cutting`.
- Estado de esta entrega: refinamiento propuesto; sin implementación ni aceptación operativa.

## Objetivo

Acreditar seguimiento existente SEG-01/02 antes de reorganizar UI-01–05. Mantener SEG-03 condicionado a una decisión expresa de operación/retención. Fuente Jira conservada en el cambio OpenSpec.

## Alcance y compatibilidad

Diez decisiones y diez requisitos propuestos; inventario concreto en design.md y 24 tareas propuestas en plan-tareas-propuesto.md (10 S, 14 M). tasks.md aún conserva las ocho tareas genéricas hasta aplicación explícita del mapa.

Hallazgos de inspección: cliente no envía filtros admitidos por servicio; contextual usa selección en lugar de nodo pulsado; menú Importación nace del host administrativo. Reproducir antes de corregir producto. Mantener contratos/servicios/asistente, recuperación por IDs, autoridad y efectos confirmados. Sin esquema nuevo, SII, almacenamiento compartido, workers, limpieza ni despliegue.

La reversa futura de navegación conserva referencias/datos/efectos; nunca revierte lotes o elimina staging. Esta entrega solo modifica planificación/documentación.
