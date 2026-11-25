# DOC-127 — Verificación integral, capacidad y cierre

- Ticket: DOC-127
- Cambio OpenSpec: doc-127-importacion-excel-07-verificacion-integral-cierre
- Clasificación: cross_cutting; perfil legacy-webforms-vb.

## Evidencia requerida

Build MSBuild net461 aislado, harness nativo DOC122–125, UI/cliente/adaptadores existentes y regresión afectada. Cada resultado incluye comando, SHA, hashes, ambiente y limitación. La QA manual anterior se conserva como declaración histórica con su alcance; no se atribuye aceptación humana nueva. Las tareas siguen abiertas hasta reunir su evidencia.

## QA/E2E WebForms

Entrega local DOC127: **19/33 tareas**, build net461 0 errores/308 advertencias, 59 pruebas focales de adaptadores, 4.616 checks nativos, 19 SII y 18 libros locales verificados mediante una prueba reproducible del lector. La regresión inicial de193 pruebas se conserva como corte previo a las ampliaciones; los conteos de suites se solapan. SQL28/29 y guía de operación entregados; 19 SELECT de control y cero SQL modificador/sesiones autenticadas nuevas. Evidencia y límites en `Doc/Actualizacion/Importacion/Validacion/doc127-implementacion-local-20261125.md/json`.

Reutilizar tools/e2e y runner DOC110 con scopes cerrados por ventana. E2E real requiere autorización de ambiente/cuentas/datos y secretos efímeros; no ejecutar SQL de instalación desde tests. Completar celdas1000/5000/10000 XLS/XLSX, por modo, tiempos y memoria, fronteras exactas y recuperación/autoridad. Evidencia previa: DOC124 preparó8000/12759; DOC125/126 confirmaron1000 por modo. No se atribuye5000/10000/20000 físicas ni cortes COMMIT/red. Validaciones documentales se registran en doc127-refinamiento-20261125.md/json.
