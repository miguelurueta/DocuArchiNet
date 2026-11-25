# DOC-127 — Verificación integral, capacidad y cierre

- Ticket: DOC-127
- Cambio OpenSpec: doc-127-importacion-excel-07-verificacion-integral-cierre
- Clasificación: cross_cutting; perfil legacy-webforms-vb.

## Evidencia requerida

Build MSBuild net461 aislado, harness nativo DOC122–125, UI/cliente/adaptadores existentes y regresión afectada. Cada resultado incluye comando, SHA, hashes, ambiente y limitación. La QA manual anterior se conserva como declaración histórica con su alcance; no se atribuye aceptación humana nueva. Las tareas siguen abiertas hasta reunir su evidencia.

## QA/E2E WebForms

Corrección posterior de `form1`: Excel recibe un propietario de formulario independiente para sus controles; required y restricciones numéricas dejan de bloquear el árbol del gestor. Script doc127-1;81/81 regresiones locales PASS y huella HTTP estática coincidente. No acredita E2E autenticada nueva. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc127-correccion-formulario-20261125.md/json`. Avance20/33;7.1 mantiene pendiente el corte integral.

Aplicación posterior delegada,2026-11-25: **20/33 tareas**, con4.1 completada. SQL30–32 aplicados y COMMIT confirmados fuera de E2E; SQL33 posterior y119 comprobaciones PASS. Dos tablas vacías, fuentes10/11 y plantillas61–68 con16 campos;30 tablas y78 perfiles anteriores conservan sus huellas. Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc127-aplicacion-recursos-20261125.md/json`. No se ejecutaron E2E real ni EXPLAIN29; los siguientes párrafos conservan los cortes históricos.

Preparación posterior:11 SELECT confirmaron UNIQUE(NOMBRE_PLANTILLA), tres cabeceras/sus seis campos y los recursos anteriores. SQL30–33 y propuesta de18 ventanas entregados, sin aplicación/autenticación. Se conservan las fuentes funcionales verificadas y19/33 tareas;4.1 depende de confirmación manual y SELECT posterior. Evidencia separada: `doc127-preparacion-recursos-20261125.md/json`; no reemplaza el corte local anterior.

Entrega local DOC127: **19/33 tareas**, build net461 0 errores/308 advertencias, 59 pruebas focales de adaptadores, 4.616 checks nativos, 19 SII y 18 libros locales verificados mediante una prueba reproducible del lector. La regresión inicial de193 pruebas se conserva como corte previo a las ampliaciones; los conteos de suites se solapan. SQL28/29 y guía de operación entregados; 19 SELECT de control y cero SQL modificador/sesiones autenticadas nuevas. Evidencia y límites en `Doc/Actualizacion/Importacion/Validacion/doc127-implementacion-local-20261125.md/json`.

Reutilizar tools/e2e y runner DOC110 con scopes cerrados por ventana. E2E real requiere autorización de ambiente/cuentas/datos y secretos efímeros; no ejecutar SQL de instalación desde tests. Completar celdas1000/5000/10000 XLS/XLSX, por modo, tiempos y memoria, fronteras exactas y recuperación/autoridad. Evidencia previa: DOC124 preparó8000/12759; DOC125/126 confirmaron1000 por modo. No se atribuye5000/10000/20000 físicas ni cortes COMMIT/red. Validaciones documentales se registran en doc127-refinamiento-20261125.md/json.
