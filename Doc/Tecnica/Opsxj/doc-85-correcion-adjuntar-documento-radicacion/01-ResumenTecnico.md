# CORRECION-ADJUNTAR-DOCUMENTO-RADICACION

- Ticket: DOC-85
- Cambio OpenSpec: doc-85-correcion-adjuntar-documento-radicacion
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Corregir la pérdida del radicado autoritativo en `ADJUNTARADICACION`: el estado seleccionado se resuelve una vez en servidor y se entrega explícitamente a plantilla, índices y almacenamiento.

## Alcance y compatibilidad

La implementación afecta únicamente la coordinación backend de Radicación Simplificada. Conserva el handler, los JavaScript compartidos, siete consumidores legacy y el almacenamiento existente. La documentación técnica canónica, incluida la reversa, está en `Doc/Actualizacion/RadicacionSimplificada/Adjunta/DOC-85-correccion-contexto-radicado-adjunto/README.md`.
