# DOC-125 — Confirmación y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: refinamiento técnico, sin implementación ni E2E escritora ejecutada.

## Objetivo

Confirmar cargas PREPARADA de DOC124 mediante los modos CREAR, AGREGAR y REEMPLAZAR, con filas/resultado durables, rollback íntegro e idempotencia por ID y contenido. El refinamiento tiene quince decisiones, quince requisitos, 44 escenarios y 31 tareas; sólo la entrega del preflight SELECT está completada.

## Alcance y compatibilidad

Reutilizar net461, MySQL5.1.37, MySql.Data6.2.3, lector y recursos RA/DA existentes. Mantener snapshots v1 y las 22 columnas operativas. Conservar máximo configurado 20000 y 10000000 bytes. La interfaz corresponde a fase06. Diseño vigente en Doc/Actualizacion/Importacion/Diseno/05-confirmacion-recuperacion-doc125.md y artefactos del cambio.
