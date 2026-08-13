## Why

Workflow es una pantalla ASP.NET WebForms operativa, con `UpdatePanel`, postbacks y permisos de servidor que no se pueden alterar durante la modernización visual. JIRA-02 crea una capa opt-in para modernizar el Centro de Trabajo sin abrir una segunda ruta visual no controlada ni obligar a revertir lógica de negocio para recuperar la apariencia anterior.

## What Changes

- Añadir los recursos aislados `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js`, entregados solo por `workflow/Webworkflow.aspx` a un piloto aprobado en servidor.
- Emitir `meta viewport` solo para el piloto DOC-2 tanto en el host superior `Defaul/WebFormInicioDocuarchiGestion.aspx` como en el acceso directo a `workflow/Webworkflow.aspx`, de modo que los breakpoints responsive usen el ancho real del dispositivo sin variar la cabecera ni el viewport de la línea base.
- Resolver en `workflow/Webworkflow.aspx.vb` la bandera `WorkflowCentroTrabajoModernEnabled`, el piloto cerrado `WorkflowCentroTrabajoModernPilotProfiles` y las subcapas `WorkflowCentroTrabajoModernLayers`; el valor seguro por defecto es apagado.
- Conservar los siete recursos visuales manuales previos como línea base aprobada. DOC-2 no los duplica, mueve ni reemplaza: su capa scoped se entrega después de esa base y de `Webworkflow.js`.
- Definir el contrato CSS reutilizable bajo `.workflow-centro-trabajo-moderno`, con tokens, breakpoints, `z-index`, estados y componentes reutilizables derivados del HTML base aprobado.
- Consolidar para pilotos el menú superior `Documentos` en `#ctw-document-actions-menu`: conserva las rutas legacy de adjunto, digitalización, eliminación y compartición, y muestra una sola vez las cuatro acciones masivas de copia o vinculación. Fuera del piloto permanece el menú baseline.
- Simplificar para pilotos el menú `Opciones`: conservar `Recuperar tarea` y dejar `Detalle de la sesión`, `Grupo relacionado` y `Estado de paginación` disponibles únicamente en la línea base.
- Trasladar para pilotos `Servicio default` desde `Servicios` a `Opciones`, conservando el id y handler legacy; el panel WebForms de servicios permanece para no romper dependencias de servidor.
- Preservar para pilotos `#pendiente_selec_tarea` como la acción operativa baseline: conserva id, visibilidad, icono, etiqueta y handler `E-ETP` de cierre o envío a pendientes que resuelva la lógica legacy. La modernización no cambia el estado de la tarea ni sustituye su transición.
- Permitir rollback total por flag y parcial retirando `actions`, `documents` o `a11y`, sin tocar datos, permisos, eventos, controles WebForms ni despliegue de lógica.
- Corregir la interacción de la lista de documentos relacionados: el clic sobre la zona no interactiva de una fila abre el documento mediante el handler legacy existente; el checkbox y el menú conservan sus acciones propias.
- Reubicar en el piloto el contexto que ya emiten los labels funcionales como única franja superior del área de trabajo, compactar las barras de comandos y recuperar altura útil para el visor.
- Separar visual y semánticamente el control que autoriza o revoca la tarea del acceso que solo consulta su historial; conservar el handler AJAX, permisos y postback existentes.
- Normalizar el contador de la lista a `Documentos (N)` solo para el piloto aprobado.
- Reducir el cromo persistente de herramientas: `Opciones`, `Detalle`, notas, historial y retorno a tareas se presentan como controles ghost; las decisiones secundarias conservan borde suave y el avance por flujo es la única acción sólida.
- Refinar el dropdown `Detalle` del piloto: estado activo ligero, ancho de 360–380 px adaptable, secciones de información/trazabilidad/documentos e iconos diferenciados, sin variar ninguna ruta legacy.

## Acceptance Scope

- Sin activación, la página conserva apariencia, DOM y comportamiento baseline; no entrega recursos DOC-2 ni obtiene efectos por parámetros, cookies o clases añadidas desde el navegador.
- En el piloto, el documento HTML declara `width=device-width, initial-scale=1`; fuera del piloto no se emite esa declaración y el baseline conserva su cabecera original.
- Con piloto aprobado, solo el contenedor `#div_content_general_wf` obtiene la clase raíz y las subcapas calculadas por servidor; el adaptador se limita a clases de presentación dentro de ese límite, incluso después de un postback parcial.
- El contexto de tarea del piloto se muestra una sola vez, antes de documentos, visor e índice. Reutiliza `Label_estado_tarea_selecion` y `Label_estado_selecion` para los datos funcionales y añade únicamente dos labels decorativos de servidor para título de trámite y estado; no crea consultas, postbacks ni estado paralelo.
- La autorización conserva su efecto legacy inmediato, pero su checkbox de estado y el menú de historial quedan inequívocamente diferenciados; el modo no piloto permanece sin variación.
- La validación debe cubrir flag `0`/`1`, usuario fuera/dentro de piloto, rollback maestro/parcial, recarga forzada, navegación interna y cuatro resoluciones. La aprobación manual requiere evidencia asociada al SHA desplegado.

## Jira Details

> # JIRA-02 — Infraestructura visual aislada y reversible
> 
> ## Prompt para Jira
> 
> **Rol:** Actúa como arquitecto frontend senior para aplicaciones ASP.NET WebForms, experto en CSS incremental, carga de recursos legacy y mecanismos de feature flag reversibles.
> 
> Implementa la infraestructura opt-in de modernización del centro de trabajo Workflow. Debe cargar una capa visual aislada sin modificar el comportamiento existente y debe poder desactivarse sin despliegue de lógica de negocio. La implementación parte de la decisión de corte de JIRA-00: no se añade una segunda capa que coexista sin control con recursos visuales ya activos.
> 
> ### Alcance
> 
> - Crear `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js`.
> - Migrar, aislar o retirar de la ruta de carga los recursos preexistentes que JIRA-00 haya clasificado como incompatibles; ninguna capa moderna debe tener efectos cuando el modo esté apagado.
> - Cargarlos después de todos los recursos CSS y JavaScript legacy relevantes, en particular después de `Webworkflow.js`, con versión de caché explícita y orden verificado en el HTML entregado.
> - Definir una bandera de entorno/configuración evaluada en servidor: `WorkflowCentroTrabajoModernEnabled`, con valor predeterminado `false`, y una lista de perfiles piloto evaluada en servidor. La página emite la clase raíz `workflow-centro-trabajo-moderno` solo cuando ambas condiciones estén aprobadas.
> - Definir clases de subcapa emitidas por el mismo mecanismo: `ctw-layer-layout`, `ctw-layer-actions`, `ctw-layer-documents` y `ctw-layer-a11y`. Deben tener dependencias y valores predeterminados documentados para permitir rollback parcial sin retirar una hoja compartida.
> - Definir contrato de selectores, variables CSS, capas `z-index` y clases de estado.
> - Implementar los tokens y componentes de `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`; el resultado visual debe reproducir el HTML base, no una interpretación libre.
> 
> ### Restricciones no negociables
> 
> - Sin la clase de activación la interfaz debe ser idéntica a la actual.
> - No sustituir HTML ni alterar controles ASP.NET.
> - El adaptador no puede modificar DOM, clases, atributos ni foco fuera del contenedor moderno; una clase agregada manualmente desde cliente no constituye segmentación válida del piloto.
> - No habilitar por CSS una acción que el servidor haya ocultado por permisos.
> - El adaptador no puede lanzar errores que bloqueen scripts existentes.
> 
> ### Entregables técnicos
> 
> 1. `01-ArquitecturaActivacion.md`: bandera, perfiles piloto, punto de evaluación servidor, carga, selectores permitidos, subcapas y rollback.
> 2. `02-ContratoCSS.md`: variables, breakpoints y `z-index`.
> 3. `03-PlanCutoverCapasPrevias.md`: destino de cada recurso inventariado en JIRA-00 y prueba de ausencia de efectos con modo apagado.
> 4. `04-PruebasActivacion.md`: evidencia modo apagado/encendido y de cada rollback parcial.
> 5. Hoja CSS con componentes scoped: `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar`.
> 
> ### Criterios de aceptación
> 
> - Con el flag maestro apagado no hay diferencia visual, mutación DOM ni error respecto a la línea base aprobada de JIRA-01.
> - Activar/desactivar la clase raíz o una subcapa cambia solo la presentación de su alcance documentado.
> - No hay errores JavaScript en consola durante carga ni postback.
> - La reversión maestra o parcial no requiere revertir eventos, redeploy de lógica ni modificación de datos.
> 
> ### Pruebas requeridas
> 
> - Comparación visual y funcional con bandera en `0` y `1`.
> - Validar usuario fuera/dentro de perfil piloto; el modo moderno no se habilita mediante parámetro o manipulación de cliente.
> - Validar rollback maestro y de cada subcapa que haya sido habilitada.
> - Validar caché tras recarga forzada y navegación interna.
> 
> ### Reversión
> 
> Desactivar `WorkflowCentroTrabajoModernEnabled` para recuperación total o la subcapa documentada para recuperación parcial; mantener recursos publicados e inertes para no generar referencias rotas.

## Jira Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: INFRAESTRUCTURA, REVERSIBLE, VISUAL

## Capabilities

### New Capabilities
- `infraestructura-visual-aislada-reversible`: Capacidad derivada del ticket Jira para continuar el refinamiento funcional en OpenSpec.

### Modified Capabilities
- 

## Impact

- Página y code-behind: `Defaul/WebFormInicioDocuarchiGestion.aspx`, `workflow/Webworkflow.aspx` y sus code-behind.
- Configuración: tres `appSettings` en `Web.config`, apagados de forma segura en la rama base.
- Presentación: hoja CSS y adaptador JavaScript nuevos, declarados como `Content` en el proyecto.
- Compatibilidad: los recursos pre-DOC-2 conservan su orden de carga y permanecen fuera del rollback de DOC-2.
- Operación: la promoción depende de perfiles piloto y la evidencia QA, no de controles de cliente.
