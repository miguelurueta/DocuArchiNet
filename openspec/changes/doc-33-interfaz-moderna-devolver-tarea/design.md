<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Contexto

DOC-32 expone preview y ejecución de devolución con contexto servidor, validación Ruta/Flujo, cursor opaco, token y lock. La página aún deriva **Elegir actividad anterior** al postback `D-TASK-ANT`, botón oculto y handler Web Forms. DOC-33 elimina únicamente ese borde y conserva la página Web Forms como host de módulos JavaScript aislados.

## Decisiones

### D-01 — Registro moderno sin gate

`workflow/Webworkflow.aspx.vb` registrará estilo, diálogo común, módulos y bootstrap de devolución siempre que la presentación Workflow sea válida. El bootstrap toma únicamente los `ClientID` de los campos de tarea ya existentes; no consulta ni cambia `WorkflowCentroTrabajoModernActive` y no agrega una bandera nueva.

### D-02 — Contrato y estado exclusivos del cliente

El markup usará un trigger `workflow-return-activity-trigger`, un modal y atributos `data-workflow-return-activity-*`. `workflow-return-activity-ui.js` conservará su propio contador de solicitudes, selección, cursor, aborto y listeners `workflow:return-activity-*`; no importará ni emitirá eventos de envío, grupo o Usuario anterior.

### D-03 — Preview paginado no mutante

El módulo envía `idTarea`, `termino`, `cursor` y `tamanoPagina` a `PreviewDevolverActividad`. El término tiene mínimo de dos caracteres cuando no es vacío, debounce de 300 ms, límite de página del contrato y descarte de resultados que no correspondan a la solicitud actual. Solo materializa el JSON autorizado de `PrevisualizacionDevolverActividadDto`; trata `IdConector` como referencia contextual opaca.

### D-04 — Confirmación y ejecución

`workflow-return-activity-confirmation.js` recibe una única selección vigente y usa `ConfirmationDialog`. Envía a `EjecutarDevolverActividad` exclusivamente `idTarea`, `idConector` y `tokenVersion`. Mientras la promesa está pendiente no permite doble clic, confirmación adicional ni un cierre que abandone el resultado. Un bloqueo, error o timeout no inventa autorización ni usa fallback legacy.

### D-05 — Presentación posterior al resultado

Solo un resultado exitoso invoca `WorkflowTransitionPagePresentation.applySuccess` con la tarea afectada y un mensaje de devolución. El módulo conserva foco, Escape, cancelación, trampa de foco, estados ARIA y diseño móvil. Los resultados no exitosos mantienen el modal en estado seguro y no alteran bandeja, contador, visor ni scroll.

### D-06 — Retiro de la ruta sustituida

Se retiran el `onclick` `D-TASK-ANT`, `Button_tool_devolver_a_actividades_anterior`, su declaración de diseñador, handler y lógica de callback/postback asociada. No se tocan `Button_tool_devolver_a_usuario`, Continuar flujo, envío a usuario o envío a grupo.

### D-07 — Verificación y documentación

Pruebas CJS cargarán los módulos en VM y comprobarán bootstrap, payload, páginas, obsolescencia, selección, confirmación, accesibilidad, éxito y aislamiento. Se ejecutarán MSBuild y pruebas focales. La QA autenticada se documenta como no ejecutada hasta recibir autorización expresa.

## Riesgos y mitigación

- Un selector común contaminaría otra transición: se usan nombres, eventos y pruebas de ausencia exclusivos.
- Un preview viejo podría ejecutar una devolución incorrecta: la selección se invalida con cada búsqueda, página, tarea o cierre; servidor revalida token y conector.
- Retirar Web Forms podría afectar Usuario anterior: las pruebas estáticas aíslan exclusivamente el botón y handler de actividad anterior.

## Reversión

La reversión restaura solo el trigger y handler de actividad anterior desde el commit previo; no revierte transiciones ya confirmadas ni modifica datos, endpoints o configuración de ambiente.
