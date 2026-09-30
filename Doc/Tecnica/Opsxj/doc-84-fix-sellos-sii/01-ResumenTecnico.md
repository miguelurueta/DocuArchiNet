# FIX-SELLOS-SII

- Ticket: DOC-84
- Cambio OpenSpec: doc-84-fix-sellos-sii
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Restaurar la representación completa de tipología, formato e icono al importar sellos SII en la lista Workflow, exclusivamente mediante inserción JavaScript.

## Alcance y compatibilidad

La solución agrega una proyección Workflow efímera y un appender exclusivo. ENLASE conserva su DTO, adaptador, grid y destino `rad`. No hay cambios de esquema ni recarga del servidor. La reversa consiste en revertir en conjunto las capas de proyección, el asset y su registro.
