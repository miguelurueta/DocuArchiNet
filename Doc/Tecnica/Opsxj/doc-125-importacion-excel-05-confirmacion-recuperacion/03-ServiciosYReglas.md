# DOC-125 — Confirmación y recuperación

- Ticket: DOC-125
- Cambio OpenSpec: doc-125-importacion-excel-05-confirmacion-recuperacion
- Clasificacion: cross_cutting (Transversal)
- Estado: refinamiento técnico, sin implementación ni E2E escritora ejecutada.

## Servicios y reglas

ServicioConfirmacionExcel coordinará autoridad, identidad, replay histórico y revalidación. MySqlCargaExcelRepository conservará el lock por ID y añadirá lock de destino en otra conexión compatible con MySQL5.1. MySqlConfirmacionExcelWriter realizará DML parametrizado y publicación transaccional; MySqlEstructuraExcel inspeccionará elegibilidad, creación y procedencia recuperable. Estos tres archivos VB nuevos se registrarán en vbproj.

Un resultado CONFIRMADA idéntico se devuelve antes de resolver archivo, límites o plantilla actuales. Para una carga por ejecutar se valida el archivo completo, snapshots, política, límites y esquema. Un error de commit mantiene resultado incierto; no degrada una confirmación durable. Los INSERT agrupados comparten una sola transacción, sin pausa persistida ni commits parciales.
