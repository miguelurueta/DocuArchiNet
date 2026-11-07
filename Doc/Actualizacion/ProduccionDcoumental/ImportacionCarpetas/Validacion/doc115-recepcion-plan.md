# DOC-115 — plan de comprobación, 2026-11-03

Estado: planificación refinada sin ejecución de producto. Fuente contractual única: ../Diseno/02-contratos-operacion.md; mapa único: ../Diseno/design.md. Decisiones y tareas en openspec/changes/doc-115-produccion-importacion-carga-carpeta-lotes. Este archivo no certifica pruebas reales ni autoriza recursos.

| Capacidad | Evidencia exigida | Estado DOC-115 | Dependencia y responsable |
| --- | --- | --- | --- |
| 07-A esquema/confirmación/rollback/reclamaciones concurrentes | MySQL real, conexiones independientes, SELECT antes/después | NO_EJECUTADO | Autorización específica y datos aislados; equipo del ambiente |
| RECIB-01 confirmación/replay/payload distinto | HTTP y filas/snapshot verificados; registrar serialización de sesión | NO_EJECUTADO | Perfil/plan real autorizado; equipo del ambiente |
| RECIB-02 truncado/ID cruzado/ruta/cuota/replay | HTTP y conteos de recepción/bytes/documentos | NO_EJECUTADO | Lotes/archivos sintéticos autorizados; equipo del ambiente |
| RECIB-03 corte/respuesta perdida | Interrupción controlada, consulta persistida y reenvío | NO_EJECUTADO | Ventana de red y recursos autorizados; equipo del ambiente |

## Reutilización inventariada

tools/e2e/package.json y playwright.config.cjs aportan comandos/reporters/evidencia. tests/support/authenticated-workflow-session.cjs ya soporta Producción. doc111-importacion-carpetas.cjs contiene transporte/fixtures/consulta pero su flujo completo ejecuta documentos: no reutilizarlo ciegamente. doc114-plan-importacion.cjs aporta controles SELECT y preparación sin efectos documentales. scripts/compile-doc111-persistence.cjs y tests/doc110-perfil-importacion-behavior.test.cjs reutilizan compiladores Framework y harness existente; run-doc111-persistence.ps1 activa perfil 24 y no se ejecuta bajo autorización inferida. fixtures/ y scripts/ alojarán solo adiciones mínimas DOC-115 descritas en design.md, todavía no implementadas.

## Límites explícitos

07-A no requiere navegador ni servicio 06 ejecutable. 07-B usa preparación real disponible desde DOC-114. Una misma sesión puede serializar HTTP: no certifica carrera MySQL. MaximoBytesLote vigente es ULong.MaxValue: cuota agregada finita se caracteriza con inyección local hasta configuración aprobada. ReconciliarRecepcion es interno y no se publica para pruebas. El script SQL 02 ya tiene evidencia histórica de aplicación: no se ejecuta de nuevo. Corte antes de .bin exige comprobar el intento antes de otra clave; incierto no se libera por timeout.

Guardar SHA de fuente/backend servido, comandos, ambiente, recursos/huellas SELECT, estado por caso y motivos de no ejecución. No almacenar credenciales/cookies/cadenas de conexión ni trazas sensibles. Comprobar conteos documentales y preservar lotes anteriores; no invocar AvanzarLote ni escritores documentales.

## Resultado local posterior a aprobación

Implementados especificación RECIB, soporte, runner delgado, fixture de presupuestos y nueve pruebas Node. Regresión seleccionada: 76/76 Node PASS; harness Framework: 2/2 PASS, incluyendo disco real con doble de persistencia. Playwright --list descubre tres escenarios sin ejecutarlos. Tareas locales: 22/28; abiertas 2.1–2.4 y 5.1–5.2. El corte de red y el descarte de resultado solo están implementados, no ejecutados en ambiente real. RECIB-03 no acredita caída del servidor. Informe reproducible: doc115-implementacion-local-20261103.md.

## Corte vigente — 2026-11-04

07-A y RECIB-01/02/03: EJECUTADO_OK, con sus autorizaciones y huellas propias. Las tablas NO_EJECUTADO anteriores corresponden a planificación y no al estado actual. Informes vigentes: doc115-07a-resultados.md y doc115-07b-resultados.md. Recursos DOC-115 conservados: planes 24–31, lotes 6–11 y staging; sin expedientes/documentos definitivos nuevos. Completadas 28/28 tareas; entrega pendiente de revisión/publicación.
