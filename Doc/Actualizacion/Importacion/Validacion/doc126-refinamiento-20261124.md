# DOC126: refinamiento técnico

Revisión de Codex del2026-11-24 por «continua» después del alta.15decisiones,15requisitos,45escenarios y44tareas (10S/34M/0L),0completadas de implementación. Perfil legacy-webforms-vb; revisión técnica sin atribuir aprobación humana.

Se inspeccionaron40rutas existentes y8nuevas previstas. Jira original conservado íntegro, SHA256 UTF-8/LF 1baa226f44e8f500fa1d71e1b9dde42030caa29cd0f8cf063d03e7aadc839575. El JSON identifica fuentes y hashes. Hallazgos resueltos en diseño: menú compartido independiente del host de carpetas, envelope Excel distinto del de perfiles, emisión CSRF sólo POST, datos de preparación disponibles con ok=false y recuperación contextual mínima sin repetir escrituras.

Preservar20000filas/10MB y aceptación anterior1000por modo; no confundirla con aceptación UI. La E2E amplía infraestructura y servicios existentes con autorización DOC126 propia; no se reabren ventanas ni claves anteriores. Los fixtures nuevos y cualquier adecuación se entregan como SQL manual fuera de tests, después del preflight SELECT; no se reaplican18–21.

Artefactos en openspec/changes/doc-126-importacion-excel-06-interfaz-menu/. Esta entrega modifica planificación/documentación: no implementa producto, ejecuta SQL, autentica E2E ni despliega. Validaciones del refinamiento y OpenSpec estricto se registran por separado del resultado funcional que aún debe ejecutarse.

Resultado: auditoría de refinamiento15/15 PASS; OpenSpec estricto97/97 PASS; formato y trazabilidad44/44tareas,15/15decisiones. Se verificaron los40hashes de fuentes existentes y el contexto Jira contra el commit inicial; git diff --check PASS. Implementación0/44; sin pruebas funcionales nuevas ni autorización real atribuida.
