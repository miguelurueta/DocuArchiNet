<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Context

Radicación Entrante clásica configura `ADJUNTARADICACION` pero no envía `IdRegistroEstadoRadicacion` ni `RadicadoRadicacion`. Desde DOC-85, el handler usa ese mismo evento para la ruta exclusiva de Radicación Simplificada y llama la sobrecarga de doce argumentos, que rechaza correctamente el ID cero. Además, al separar esa sobrecarga, DOC-85 retiró de la firma legacy la rama que resolvía el registro clásico desde `RA_ID_REGISTRO_RADICADO`.

La corrección debe recuperar únicamente el recorrido clásico. Modificar el evento o el cliente de Radicación Simplificada ampliaría innecesariamente el riesgo sobre una ruta ya protegida.

## Goals / Non-Goals

**Goals**

- Restaurar la carga desde `WebFormRadicacionEntrante.aspx` usando el registro seleccionado en la sesión del módulo.
- Dar al origen clásico un evento inequívoco y una rama de servidor propia.
- Conservar respuesta, persistencia única y proyección sin postback.
- Demostrar que DOC-85 permanece intacto.
- Conservar las ramas y firmas de todos los demás consumidores del cargador compartido.

**Non-Goals**

- Cambiar el consumidor, evento, ASPX, servicio, repositorios o contexto de Radicación Simplificada.
- Modificar `generic_control/FileUploadHandler.js` para resolver esta regresión.
- Aceptar desde el navegador el ID que autoriza la carga clásica.
- Cambiar otros eventos, tablas, migraciones o el motor de almacenamiento.

## Decisions

### D-01 — Identidad exclusiva para Radicación Entrante

`ActivaAdjuntarDocumentoRadicacion` conservará `NameLoadProceso: "ADJUNTARADICACION"` para reutilizar la configuración de tipologías y extensiones, pero enviará `evento_adjunta: "ADJUNTARADICACION_CLASICA"`. No enviará los campos autoritativos exclusivos de Simplificada.

### D-02 — Rama clásica explícita en el handler

`FileUploadHandler_.ashx.vb` reconocerá `ADJUNTARADICACION_CLASICA`, establecerá el contexto temporal correspondiente y llamará la sobrecarga de diez argumentos. La rama existente `ADJUNTARADICACION` seguirá llamando exclusivamente la firma de doce argumentos. La nueva rama conservará el mapeo actual de `uploadFiles` requerido por `insert_row_documento_relacionado`.

### D-03 — Restauración cerrada de la ruta clásica

La sobrecarga de diez argumentos recuperará una rama exclusiva para `WF_TIPO_ADJUNTA = ADJUNTARADICACION_CLASICA`. Validará que `RA_MODULO_SELECCIONADO` corresponda a Radicación Entrante, que la plantilla activa sea positiva y coincida con el registro, y que `RA_ID_REGISTRO_RADICADO` sea positivo. Después resolverá `stru_registro_estado`, obtendrá radicado, tarea, trámite y tipología desde servidor y llamará `PreAlmacenaDocumentosRadicacion`. Esta preparación usará radicado y plantilla explícitos al construir los índices, antes de cualquier intento legacy de redescubrimiento desde `DAT_ADIC_TAR`. Tras el éxito propagará `IdTareaWorkflow` desde la estructura resuelta. Ningún ID del navegador participará como autoridad.

### D-04 — DOC-85 como frontera protegida

Permanecen sin cambios `js/RadicadorSimplificado/Web_form_radicacion_simpilificada.js`, su ASPX, `generic_control/FileUploadHandler.js`, la sobrecarga de doce argumentos, `ServicioAdjuntoRadicacion`, `ContextoAdjuntoRadicacion` y sus repositorios. Sus pruebas se ejecutarán como regresión y la huella del cliente Simplificada deberá permanecer estable.

### D-05 — Caché clásica y evidencia

`WebFormRadicacionEntrante.aspx` versionará su script modificado. Las pruebas verificarán selección de firma, rechazo previo, respuesta completa, fila única y ausencia de postback. `tools/e2e` registrará un escenario clásico con perfil no sensible, validación previa, runner interactivo, autenticación compartida, consultas de control `SELECT` y evidencia saneada. La ejecución real seguirá requiriendo autorización explícita para ambiente, cuenta, carga y recurso descartable.

### D-06 — Compatibilidad transversal del componente compartido

La corrección será aditiva: solo se agregará `ADJUNTARADICACION_CLASICA` y su recorrido. Una prueba de inventario congelará los eventos existentes del handler, la aridad de las ocho llamadas actuales a `UploadSaveFile` y las fronteras de sesión/respuesta de cada rama. Deben permanecer equivalentes PQRS, adjuntar/reemplazar versión, SII real/virtual, migración, gestión de respuestas, Workflow selección/enlace, Radicación Simplificada, Producción, respuestas/anexos de correspondencia y enlace de radicados. También quedan protegidos los consumidores directos de `workflow/Webworkflow.aspx.vb` y `webservice/WebServiceRadicacion.asmx.vb`.

### D-07 — Soporte documental condicionado a una asignación vigente

`Hidden_numero_rad_pend` es únicamente un contador informativo y no representa la asignación activa. En carga completa, `inicio_tab_radicador` decidirá el estado mediante `Hidden_radicado_seleccion`; `nuevo_radicado_tab` y `terminar_radicado_tab` volverán a Recepción. En postback parcial, `CheckStatus` ya verifica el resultado autoritativo del servidor y solo invoca `asig_radicado_tab` cuando `Hidden_result_boton_tool = "YES"`; por eso esa transición habilitará Soporte directamente, sin volver a validar valores de DOM potencialmente anteriores a la respuesta. `tab_sow`, `tab_disable` y `tab_enabled` usarán operaciones idempotentes.

El selector `Panel_pendiente_radicado` no es una acción documental: permite elegir la tarea que dará contexto al módulo. Por ello se ubicará en el encabezado común, fuera de `soporte_envio`, conservando `A1`, `Label_numero_item`, el mismo control servidor y el mismo evento de asignación. Su panel de actualización será independiente y visible; contendrá también `Hidden_numero_rad_pend`. `Hidden_radicado_seleccion` se ubicará dentro de `UpdatePanel_boton_tool`, que `Asignar_radicado` ya actualiza explícitamente, para mantener coherencia tras el postback parcial sin cambiar la lógica servidor.

## Risks / Trade-offs

- Restaurar código histórico sin controles reproduciría ambigüedad; se mitiga con un evento exclusivo y validación explícita del ID de sesión.
- El handler compartido es un archivo protegido por DOC-85; se limita el cambio a una rama aditiva y se actualiza su huella solo después de revisar el diff.
- El recurso clásico no tiene versión; se agrega una para impedir que el navegador conserve el evento anterior.
- Una edición accidental alrededor de la rama nueva podría alterar bloques vecinos del handler; se mitiga con delimitación estructural, inventario de eventos y regresión de Producción, Workflow, versiones, migración, SII y radicación.
- La reversa debe retirar conjuntamente el evento clásico, su rama de handler y la rama de almacenamiento restaurada.
- Usar el contador de pendientes como autorización visual expone acciones sin contexto; se elimina esa dependencia y se conserva el contador únicamente para informar y abrir la selección de tareas.
- Validar una transición exitosa con controles fuera del `UpdatePanel` puede leer valores obsoletos; los controles de estado se alinean con los paneles ya actualizados por cada operación.

## Migration Plan

1. Crear contratos que reproduzcan el rechazo clásico y protejan el recorrido Simplificada actual.
2. Introducir el evento clásico y versionar únicamente el script de Radicación Entrante.
3. Agregar la rama clásica en el handler y restaurar la resolución por sesión en la firma de diez argumentos.
4. Ejecutar contratos focales, regresión DOC-85, compilación y revisión del diff.
5. Ejecutar la matriz de no regresión de los demás consumidores del componente compartido.
6. Ejecutar E2E real solo con autorización; registrar evidencia saneada o bloqueo.
7. Validar el estado inicial y las transiciones de pestañas con y sin asignación, incluso cuando existan pendientes.

## Open Questions

- No hay preguntas de producto abiertas. Si el contexto clásico de sesión no contiene tarea, trámite, radicado o gabinete coherentes, la implementación debe rechazar antes del almacenamiento y volver a refinamiento antes de ampliar el contrato.
