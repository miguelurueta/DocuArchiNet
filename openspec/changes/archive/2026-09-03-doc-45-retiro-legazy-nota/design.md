<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->

# Diseño técnico — retiro legacy de Notas Workflow

## Context

DOC-45 cierra la modernización de Notas retirando el consumidor visual legacy del Centro de Trabajo y el código sin consumidores demostrables. DOC-42 aporta el contrato transaccional moderno, DOC-43 la interfaz moderna y DOC-44 la exclusión de doble presentación y su regresión E2E. Las rutas compartidas por radicación y gestión de correspondencia permanecen hasta que esos módulos migren; conservarlas no implica mantener el consumidor antiguo dentro de `Webworkflow`.

## Goals / Non-Goals

**Goals**

- Mantener un inventario reproducible de consumidores legacy de Notas.
- Retirar atómicamente la rutina duplicada de borrado que no tenga referencias.
- Retirar el botón, controles, handlers y llamadas JavaScript legacy exclusivos del Centro de Trabajo Workflow.
- Dejar el acceso con contador y el modal moderno como única presentación de Notas cuando el contexto Workflow sea válido.
- Probar que la ruta activa legacy y el contrato moderno conservan su comportamiento.
- Documentar evidencia, rollback y deuda residual sin exponer información sensible.

**Non-Goals**

- Eliminar `WebFormAnotacion`, los endpoints `Service_*_nota_tarea_workflow` o scripts compartidos con otros módulos.
- Cambiar tablas, datos, autorización, auditoría o semántica de borrado.
- Activar permanentemente `WorkflowCentroTrabajoModernActive`.
- Crear otro arnés E2E, login, `.env` o pruebas simuladas como sustituto de evidencia real.

## Decisions

### D-08 — Descubribilidad del estado vacío

El acceso principal conserva el contador, pero adapta su etiqueta y nombre accesible al resultado real de `ContarNotas`: con cero registros muestra `Nueva nota 0` y abre directamente el editor; con uno o más muestra `Notas N` y abre la gestión. La decisión espera la lectura autorizada incluso si el usuario pulsa antes de finalizarla y no infiere estado desde el DOM. Satisface RQ-08.

### D-07 — Propiedad y lectura ampliada de notas

El servidor expone en cada nota una capacidad booleana de gestión calculada exclusivamente con la identidad de la sesión y el autor persistido. El cliente no recibe ni envía identidad para decidir propiedad: muestra `Editar` y `Eliminar` únicamente cuando la capacidad es verdadera. Toda persona autorizada para consultar la tarea puede leer la nota; el contenido extenso se trunca solo visualmente y `Ver nota completa` abre un diálogo auxiliar de solo lectura, tamaño estable, scroll interno, cierre por botón/Escape y restitución de foco. El repositorio conserva la defensa en profundidad de `UPDATE`/`DELETE` por tarea, actividad, autor y versión, y diferencia `NotOwner` sin revelar contenido. La E2E usa una nota ajena existente de la misma tarea para demostrar propiedad y crea una nota propia extensa temporal para comprobar el visor, eliminándola al finalizar. Satisface RQ-07.

### D-01 — Retiro atómico de la rutina duplicada

`Class_anotacion_tarea.Eliminar_nota_tarea_workflow` se elimina únicamente después de repetir una búsqueda estática cuyo único resultado sea su definición. Se conserva `Eliminar_nota_service_workflow`, porque `WebServiceWorkflow.asmx.vb` la invoca desde `Service_delete_nota_tarea_workflow`. El cambio se limita a una función VB para que el rollback sea la restauración directa del bloque eliminado. Satisface RQ-01.

### D-02 — Frontera estricta de compatibilidad

Los endpoints legacy `Service_*_nota_tarea_workflow`, `WebFormAnotacion.aspx(.vb)`, `GridView_lista_notas` y los scripts de radicación/gestión de correspondencia permanecen intactos mientras tengan referencias de proyecto, JavaScript o code-behind. El inventario separa “candidato sin consumidores” de “legacy vivo”; DOC-45 no interpreta antigüedad como autorización de borrado. Satisface RQ-02.

### D-03 — Evidencia integrada y autorizada

Las verificaciones reutilizan `tools/e2e`, `authenticated-workflow-session.cjs`, la configuración Playwright y las suites de Notas existentes. Primero se ejecutan política, sintaxis y compilación; las E2E autenticadas o con mutación solo se ejecutan tras las autorizaciones literales del ambiente, cuenta y tarea descartable. La evidencia conserva únicamente resultados, conteos, latencias y huellas saneadas. Satisface RQ-03.

### D-04 — Rollback y estado seguro del gate

El rollback se conserva como reversión atómica del diff mediante Git, no como doble presentación activa. Los runners deben restaurar en `finally` el valor `false` y vaciar usuarios/grupos. El cierre se bloquea si ese estado no puede verificarse. No requiere reversión de datos ni DDL. Satisface RQ-04.

### D-05 — Retiro del consumidor legacy de Webworkflow

Se eliminan de `Webworkflow.aspx`, su code-behind, `Classselecciotarea.vb` y `js/workflow/Webworkflow.js` los elementos que solo abren o administran la interfaz antigua de Notas. No se eliminan `Service_*_nota_tarea_workflow`, `WebFormAnotacion` ni sus consumidores de Radicación y Correspondencia. Para un contexto Workflow válido, `Panel_notas_modernas` es la única superficie: se abre como diálogo superpuesto desde un acceso visible en la barra que presenta el contador actualizado. Su disponibilidad no depende del registro local inerte del gate. Satisface RQ-05.

La confirmación de borrado es un `alertdialog` auxiliar propio de la superficie moderna; no usa el confirmador nativo del navegador. El diálogo principal conserva una altura estable limitada por el viewport, delega el crecimiento a scroll interno de la lista y limpia automáticamente los mensajes exitosos después de anunciarlos. Esto evita saltos de tamaño y estados obsoletos sin cambiar la semántica transaccional. Satisface RQ-05.

### D-06 — E2E integrada como criterio inseparable de cierre

DOC-45 reutiliza `authenticated-workflow-session.cjs`, `playwright.config.cjs`, el runner interactivo y la suite real de DOC-44; no crea otro login, arnés, proyecto, configuración ni `.env`. La aserción legacy cambia de “oculto” a “inexistente” y la corrida debe comprobar el panel moderno, controles de acceso, lecturas negativas, CRUD y una mutación por acción. Como la política oficial moderna depende del contexto válido y no del registro local, el ejecutor no alterará `Web.config` únicamente para hacer visible la UI. Toda ejecución real requiere autorización literal y datos descartables; sin ellos, el cambio permanece abierto. Satisface RQ-06.

### D-09 — Compatibilidad cromática de acciones no afectadas

La presentación moderna fija de forma explícita, y bajo selectores locales, el fondo y contraste de los cuatro estados dinámicos de `GridView2`: documentos (`primary`), detalle (`info`), retomar (`warning`) y asignar (`success`), además de los disparadores mostrar/ocultar índice. La corrección no recrea controles, no cambia sus atributos `tip_event`, no modifica eventos y no amplía el retiro de Notas. Las URLs de las hojas afectadas se versionan para evitar que una caché anterior mantenga el estado sin color. Satisface RQ-09.

### D-10 — Ciclo de vida compatible con UpdatePanel

El acceso de Notas se enlaza mediante delegación en `document`, porque la selección de una tarea puede sustituir la barra de acciones y destruir el nodo que recibió el listener inicial. La tarea se obtiene nuevamente desde el control explícito configurado y `PageRequestManager.endRequest` dispara una recarga de solo lectura. La E2E selecciona la tarea por la UI y continúa sin `page.reload`, por lo que reproduce el recorrido manual que reveló la falla. Satisface RQ-10.

## Risks / Trade-offs

- Las invocaciones por reflexión o configuración pueden escapar a una búsqueda textual. Por eso el retiro requiere además compilación y regresión; cualquier indicio dinámico conserva la función.
- Mantener endpoints legacy vivos conserva deuda técnica, pero evita ampliar DOC-45 a módulos no autorizados.
- La E2E real depende de ambiente y datos descartables autorizados. Sin autorización, el cambio puede avanzar en análisis y pruebas estáticas, pero no puede cerrarse.
- La evidencia E2E obtenida antes del retiro ampliado queda histórica, pero no valida el código reabierto.
- El control de integridad del runbook debe aceptar únicamente los cambios a páginas Workflow trazados por DOC-45 y seguir bloqueando cambios inesperados.
- El merge produce un SHA distinto; la evidencia OPSXJ debe reanclarse al SHA fusionado antes del cierre.

## Migration Plan

1. Repetir inventario estático y guardar la matriz ruta–consumidor–decisión.
2. Agregar una prueba de política que exija ausencia del candidato y presencia de las rutas vivas.
3. Eliminar `Eliminar_nota_tarea_workflow` y el consumidor legacy exclusivo del Centro de Trabajo.
4. Confirmar que Radicación y Correspondencia conservan sus contratos compartidos.
5. Ejecutar pruebas focales, regresiones DOC-42/43/44 y compilación VB.NET.
6. Adaptar el ejecutor E2E existente para comprobar ausencia física del consumidor legacy sin crear infraestructura paralela ni alterar innecesariamente el gate.
7. Con autorización explícita, ejecutar nuevamente la E2E sobre el código corregido y comprobar el estado seguro antes/después.
8. Actualizar documentación bajo `Doc/Actualizacion/workflow/Notas/DOC-45-RETIRO-LEGAZY-NOTA/` con inventario, trazabilidad, evidencia y rollback.

## Rollback

- Revertir como una unidad el diff de DOC-45: rutina VB retirada, consumidor Workflow, contrato `PuedeGestionar`, defensa `NotOwner`, marcado, cliente, estilos, versiones de caché y cobertura E2E.
- No restaurar únicamente la presentación legacy sobre el backend moderno ni mantener ambas superficies activas.
- Mantener disponibles `Eliminar_nota_service_workflow`, los endpoints legacy y `WebFormAnotacion` durante toda la estabilización.
- Confirmar `WorkflowCentroTrabajoModernActive=false` y audiencias vacías; no hay DDL ni reversión de datos porque DOC-45 no modifica esquema y las notas temporales E2E se eliminan en la misma corrida.

## Open Questions

- Ninguna decisión funcional bloquea el diseño. La autorización y los identificadores descartables para E2E se solicitan únicamente al llegar a la etapa de ejecución real.
