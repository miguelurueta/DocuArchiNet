# DOC-84 — Corrección de proyección de sellos SII en Workflow

## Resultado

Los documentos de sellos/constancias importados desde SII se incorporan a `GridView_list_documento_relacion_wf` mediante JavaScript, con el mismo contrato visual que produce el servidor: gabinete, identificador, radicado, tipo físico, tipología, tarea, firma e icono. La ventana solo se cierra cuando la fila queda comprobada en el DOM.

No se usa recarga completa, postback, `PageRequestManager` ni el botón `Button_actualiza_trevie_seleccion` para proyectar la importación.

Durante la preparación se predetermina la única tipología autorizada equivalente a `Constancia de Inscripción`, incluso si otra opción figura como obligatoria. La comparación admite mayúsculas, tildes y errores menores como `Contancia de Inscrpcion`; ante dos coincidencias posibles conserva la selección manual.

## Causa

La proyección incorporada en `0e8199c8` resolvió correctamente ENLASE, pero el fallback Workflow fabricaba una cadena parcial: dejaba vacíos gabinete, radicado, formato y firma, usaba el nombre general como tipología y fijaba `fa-file`. Esa fila no equivalía a la renderizada por servidor y podía perder formato, tipología y posición del menú.

## Fronteras conservadas

| Capacidad | Proyección | Grid | Destino | Resultado |
|---|---|---|---|---|
| `ANEXOS_RADICADO_ENLASE` | `EnlaseProjection` | `GridView_list_documento_relacion` | `rad` | Implementación DOC-81/DOC-83 intacta |
| Sellos/constancias Workflow | `WorkflowProjection` | `GridView_list_documento_relacion_wf` | `wf` | Adaptador DOC-84 exclusivo |

El almacenamiento Workflow construye una proyección efímera después de `AlmacenaDocumentoTareaWorkflow`. El resultado la transporta por ejecución y reconciliación con coincidencia estricta de capacidad, ítem, identidad externa, documento y tarea. El navegador valida los ocho campos, escapa el contrato delimitado, evita duplicados, invoca `insert_row_documento_relacionado(..., "wf", 1)` y verifica la fila `id_wf`.

## Archivos principales

- Backend: modelos y DTO, `LegacyImportDocumentStorageAdapter`, pasos de ejecución, orquestador y reconciliación.
- Frontend: adaptadores de progreso/reconciliación/lista, nuevo `importar-servicio-web-workflow-document-list-adapter.js` y despacho por capacidad en la UI.
- Preparación: selección predeterminada segura de Constancia de Inscripción y versión de caché `20260929-doc84defaulttype1`.
- Registro: proyecto WebForms y versiones `20260929-doc84projection1`.
- Pruebas: contrato backend, appender Workflow, separación ENLASE, ausencia de recarga y regresión DOC-83.

## Verificación del 29 de septiembre de 2026

- Suite completa `importar-servicio-web-*`: 534/534 PASS.
- Regresión determinística DOC-83, incluida DOC-81/DOC-82: 191/191 PASS.
- E2E local de la fila Workflow con scripts productivos en Chromium: 1/1 PASS.
- Compilación `GestionDocumental-Docuarchi.net.vbproj`: PASS, cero errores.
- `openspec validate doc-84-fix-sellos-sii --strict`: PASS.

La E2E local valida `id_wf`, `idd_wf`, tipología, icono y clic en las seis acciones sin red. No se ejecutó E2E autenticada, carga ni activación de gate; esas corridas requieren autorización explícita de ambiente y cuentas según `tools/e2e/AGENT-RUNBOOK.md`.

## Reversa

Revertir conjuntamente el DTO/modelo, la propagación backend, el módulo Workflow, el despacho UI, el registro del asset y sus pruebas. No se requiere reversa de datos ni esquema: la proyección es efímera y no modifica la persistencia legacy.
