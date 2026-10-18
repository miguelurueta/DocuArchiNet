# DOC-105 — Regresión transversal

- Ticket: DOC-105
- Cambio OpenSpec: doc-105-regresion-transversal
- Clasificacion: cross_cutting

- Cambio OpenSpec: `doc-105-regresion-transversal`.
- Clasificación: transversal.
- Impacto: `cross_cutting`. Estado: archivo local con salvedades autorizado, 86/91 tareas completas; especificaciones no sincronizadas.

## Objetivo

Demostrar que las mejoras DOC-100 a DOC-104 conservan rendimiento, transporte, permisos y guardado en Workflow, Correspondencia, Producción, aprobación/externo y consumidores simples del handler.

## Solución

Un runner dentro de `tools/e2e` usa cinco adaptadores cerrados y un medidor común. Ejecuta cuatro perfiles de red en caché fría/caliente, separa bytes únicos de físicos y conserva estados `PASSED`, `FAILED`, `BLOCKED` y `NOT_APPLICABLE`.

## Alcance y compatibilidad

Incluye correcciones productivas de carga inicial, contador/toolbar, zoom/rotación, descarga/impresión, posición/transparencia/geometría del grafo y altura del modal de Producción. Prepara representaciones temporales de visualización sin sustituir el original almacenado. La configuración versionada mantiene la preparación desactivada y el ejecutable vacío; su activación exige configuración administrativa y revisión de licencia/despliegue. Conserva permisos y rutas de negocio. Migración usa el handler/visores propios, no el visor express.

Workflow focal final PASSED: 34 páginas, tres aperturas, mediana 350 ms, 390.850 bytes del preview al primer canvas. No acredita cobertura total: continúan pendientes 7.5/7.5.5, 7.6, 8.6 y 9.6. Detalle vigente en `Doc/Actualizacion/VisorPdf/Implementacion/DOC-105/errores-pendientes.md`.

## Reversa

Restaurar coordinadamente los componentes productivos y sus referencias según `Doc/Actualizacion/VisorPdf/Implementacion/DOC-105/despliegue-rollback.md`; retirar solo el runner no revierte esta implementación. El ensayo real de rollback no está acreditado y permanece como salvedad explícita.
