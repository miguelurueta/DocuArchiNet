<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05 -->
# Diseño - DOC-12: lista moderna de destinos

## Contexto

La página `workflow/Webworkflow.aspx` usa un flujo Web Forms para abrir y poblar `GridView_envia_flujo`. El endpoint paralelo `PreviewEnviarTarea(idTarea)` ya resuelve sesión, autorización y destinos en servidor sin ejecutar una transición. DOC-12 agrega una representación progresiva que solo puede operar cuando el mismo gate de servidor permite al usuario.

## Arquitectura propuesta

```text
Continuar flujo
      |
      +-- gate inactivo o ausente --> UI legacy existente
      |
      +-- gate activo --> workflow-transition-ui.js
                              |
                              +--> PreviewEnviarTarea(idTarea)
                                        |
                                        +--> DTO de solo lectura
                              |
                              +--> modal moderno y callback de selección
```

La UI usa `fetch` con `credentials: 'same-origin'`, procesa el envoltorio ASMX `d` y crea todos los nodos con APIs DOM. El identificador de tarea obtenido desde el contexto visual es solo una solicitud: el ASMX vuelve a comprobar sesión, tarea y autorización.

## Decisiones

### D-01 — Convivencia con la interfaz legacy

Se crean `js/workflow/workflow-transition-ui.js` y `Styles/workflow-transition-modern.css`. La página los registra desde code-behind únicamente cuando el bootstrap moderno está activo. Esto evita bloques `<% ... %>` en la cabecera `runat="server"`, incompatibles con `AjaxControlToolkit.ToolkitResourceManager`, sin cambiar la condición de activación. El bootstrap se vuelve a registrar con `ScriptManager` después de cada postback parcial, porque Web Forms puede reemplazar el enlace al cambiar de tarea. Con el bootstrap desactivado no se agregan recursos ni listeners, no hay llamada ASMX y el enlace actual conserva su postback, grid y modal.

### D-02 — Paridad de feature gate

`WorkflowCentroTrabajoModernActive` de la página y `IWorkflowModernFeatureGate` no consultan las mismas claves ni segmentos piloto. El bootstrap de esta capacidad debe usar el segundo en servidor; usar el primero daría una autorización visual distinta a la que protege el ASMX. La integración necesita un punto de bootstrap compatible con esta regla antes de editar el comportamiento del enlace.

### D-03 — Límite del contrato de lectura

La UI admite el contrato actual: `TipoDecision`; `Contexto.Radicado`, `ActividadOrigen` y `GrupoActual`; destinos con `Id`, `Nombre`, `Destinatario`, `Grupo`, `Tipo` y `Orden`; `RequiereNotificacion`, `TokenVersion` y `Error`. La cabecera presenta radicado, tipo y grupo actual; omite trámite y nombre de actividad actual. No infiere datos desde identificadores ni HTML legacy.

DOC-12 no amplía el backend para trámite ni actividad actual legible. Un requisito futuro distinto deberá definir su propio contrato de lectura antes de incorporarlo a la UI.

### D-04 — Estados y selección

El modal tiene estados `cargando`, `sin-destinos`, `error-controlado`, `lista-disponible` y `destino-seleccionado`. Ofrece foco inicial, Escape, foco atrapado, navegación por teclado y retorno de foco al enlace. En móvil la cabecera y el cierre no se desplazan: solo el cuerpo de destinos usa scroll interno, de modo que controles de salida y selección no quedan cortados en viewports cortos. La selección emite un evento documentado y no llama a `EjecutarEnvioTarea` ni a controles Web Forms invisibles.

### D-05 — Verificación y reversa

La verificación incluye build de la solución afectada, pruebas focales del mapeo y de los estados, y QA de escritorio/móvil/accesibilidad. Una prueba autenticada solo se ejecuta bajo la autorización indicada por `AGENTS.md`. El rollback consiste en desactivar el gate; no requiere migración de datos.

## Riesgos y compatibilidad

- La página contiene una integración legacy sensible al postback. El enlace moderno no debe interferir cuando el gate no está activo.
- Un bootstrap basado en la propiedad de página existente puede habilitar clientes que el ASMX bloquea, o bloquear pilotos válidos; por eso la paridad del gate es requisito.
- El DTO actual no contiene trámite ni actividad actual legible. Por decisión de alcance, la UI los omite y muestra solo contexto verificable.

## Decisión de alcance

La representación limitada al DTO actual queda aprobada para DOC-12. La integración aún debe resolver el bootstrap de página con el mismo gate que el ASMX, sin alterar el comportamiento legacy.
