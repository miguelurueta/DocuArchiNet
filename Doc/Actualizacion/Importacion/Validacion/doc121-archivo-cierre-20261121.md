# DOC-121 — OpenSpec archivado, PR integrado y Jira cerrado

**Jira: Listo**, categoría done, confirmado por lectura posterior el 2026-11-21T21:59:21.678Z. Se utilizó la transición del cliente existente sin enviar comentarios de issue. La continuación fue autorizada por el usuario después del estado que identificaba archivo, merge y cierre como pasos siguientes. [Registro estructurado](doc121-archivo-cierre-20261121.json).

## Integración verificada

El [PR133](https://github.com/miguelurueta/DocuArchiNet/pull/133) está integrado mediante 564a5600c0321eae1e0aa1465bdb00aba8c3c745. El árbol del merge coincide con el HEAD revisado 893ad681e278be8c924d6d8345dc963fea2a9437. [CI](https://github.com/miguelurueta/DocuArchiNet/actions/runs/44953907103) concluyó con 37/37 pasos aprobados sobre ese HEAD. Antes de cerrar Jira se comprobó main local/remoto y el gobierno del SHA integrado; la carpeta principal quedó en main por avance directo, conservando el trabajo local.

## Archivo y sincronización

Se conservan siete archivos y 16 tareas completas en openspec/changes/archive/2026-11-21-doc-121-importacion-excel-01-esquema-contratos-configuracion/. El directorio activo ya no existe. La especificación principal openspec/specs/importacion-excel-01-esquema-contratos-configuracion/spec.md contiene los 8 requisitos y 24 escenarios, comparados bloque por bloque y con Purpose preservado. El contexto Jira permanece archivado como contexto.

El snapshot de reglas se obtuvo antes de escribir la especificación. Se verificó la sincronización antes y después de mover. Los siete archivos se trasladaron sin cambiar su contenido. No había enlaces Markdown relativos que reparar; las rutas de JSON históricos siguen identificando sus entradas originales. El auditor local resuelve la ubicación activa o el archivo único.

Validación global estricta **92/92**, auditor local **27/27**, refinement archivado **15/15** y gobierno **72/72** aprobados. Las 195 entradas de ejecución DOC-120 mantienen sus hashes. El registro documental final vuelve a vincular revisión/evidencia al SHA comprometido mediante archivos OPSXJ locales excluidos de Git, sin repetir pruebas de negocio.

## Alcance de la entrega

Se cierra la fase 01 contractual: esquema, SQL revisable, configuración y request/response. Los 24 escenarios son contratos revisados, no 24 corridas reales. La implementación de conexión, cuenta dedicada, lector Excel, preparación, confirmación e interfaz pertenece a las fases posteriores, conservando sus verificaciones de ambiente y E2E autorizados.

EGRESOS sigue siendo una fuente de pruebas y permanece intacta. No condiciona esta entrega. El índice de CONTRATOS aplicado anteriormente por autorización expresa conserva su evidencia y las 625 filas; esta operación no repitió su ALTER. Archivo/cierre no ejecutaron nuevas consultas de base, DDL, DML, sesiones de aplicación, despliegues o limpieza. Los cortes anteriores conservan sus estados históricos.
