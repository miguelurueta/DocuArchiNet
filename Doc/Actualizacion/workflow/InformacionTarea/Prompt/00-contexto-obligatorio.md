# Contexto obligatorio — Modernización de Información de la tarea

## Uso

Este archivo no implementa y no debe convertirse en ticket Jira. Debe leerse junto al prompt numerado enlazado por la etapa.

- No recorrer automáticamente las etapas.
- Jira controla predecesores, decisiones, aprobaciones y cierre.
- Si falta etapa, DOC o una decisión obligatoria, registrar el bloqueo y detener el trabajo afectado.

## Objetivo

Modernizar exclusivamente `Detalle → Información de la tarea` en `workflow/Webworkflow.aspx`, sustituyendo el inspector técnico basado en `SELECT *` por una vista funcional, autorizada y de solo lectura.

## Fuentes obligatorias

- Documentos de `../Exploracion/`.
- `AGENTS.md`.
- `tools/e2e/AGENT-RUNBOOK.md`, antes de diseñar o ejecutar una prueba autenticada.
- `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
- `workflow/Class_DAT_ADIC_TAR.vb`.
- `js/workflow/Webworkflow.js`.
- Contratos, servicios y repositorios modernos existentes de Workflow.
- Referencias encontradas por búsqueda completa de `S-DTS`, `Panel_detalle_flujo`, `Table_detalle_flujo`, `Listar_datos_tarea_workflow` y `Genera_interface_detalle_tarea_workflow`.

## Alcance

- `IdTarea` explícito y contexto autenticado revalidado.
- Resumen funcional estable más campos variables configurados y publicables.
- SQL parametrizado y resolución interna permitida de la tabla de Ruta.
- Tipos, etiquetas, orden, formatos y sensibilidad explícitos.
- UI accesible con tamaño estable, scroll interno y estados controlados.
- Retiro legacy solo después de inventario, pruebas y referencias activas en cero.

## Restricciones críticas

- No ejecutar `SELECT *` ni materializar columnas desconocidas en la UI.
- No confiar en `WF_RUTAWORKFLOW`, `ID_TAREA_SELECCIONDA`, campos ocultos o ViewState como autorización.
- No aceptar nombres de tabla/columna, usuario, Ruta o permisos desde el navegador.
- No exponer identificadores técnicos, datos personales, SQL, sesión, rutas o `ex.Message` sin política aprobada.
- No agregar edición, actualización, descarga, copiado o mutaciones.
- No modificar otras opciones de `Detalle`, operaciones de tarea, tabla, colores, iconos, índice o scroll.
- No crear login, arnés E2E, proyecto Playwright, configuración o `.env` paralelo.

## No mutación

Este recorrido es de lectura. La consulta no cambia tarea, estado, auditoría, documentos, radicado, configuración ni datos de negocio. Los controles de base de datos para pruebas son exclusivamente `SELECT`.

## Documentación técnica

Cuando exista DOC, mantener bajo su carpeta propia:

- `00-indice.md`;
- `01-arquitectura.md`;
- `02-contrato-catalogo-y-datos.md`;
- `03-flujo-seguridad-y-privacidad.md`;
- `04-pruebas-y-evidencia.md`;
- `05-inventario-legacy-y-rollback.md`;
- diagramas profesionales de arquitectura, caso de uso, clases, secuencia y estados.

## Entregable común

Reportar ticket, archivos, decisiones, comandos y resultados, pruebas, evidencia saneada, riesgos, deuda, rollback y etapa desbloqueada. La implementación final debe coincidir con la versión validada.

