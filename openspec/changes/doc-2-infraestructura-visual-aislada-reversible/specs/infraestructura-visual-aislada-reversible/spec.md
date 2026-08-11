## ADDED Requirements
### Requirement: INFRAESTRUCTURA-VISUAL-AISLADA-REVERSIBLE
El sistema SHALL implementar el alcance definido para DOC-2.
#### Scenario: Flujo principal
- **WHEN** se ejecuta el caso de uso principal del ticket
- **THEN** el comportamiento coincide con las reglas funcionales esperadas
#### Scenario: No-regresion
- **WHEN** se valida el modulo afectado
- **THEN** no se rompen flujos existentes
### Requirement: Politica Frontend AppResponses
El sistema SHALL sembrar reglas de consumo seguro de `AppResponses<T>` en los artefactos iniciales cuando un ticket cree o modifique consumidores de API.

#### Scenario: No filtrado de mensajes tecnicos
- **WHEN** un endpoint `AppResponses<T>` retorna `errors[0].UserMessage` y un `response.message` con `code`, `requestId`, SQL, rutas, stack trace, tokens o detalle interno
- **THEN** la UI muestra el mensaje funcional resuelto por `getUserVisibleAppResponseMessage` y no muestra el detalle tecnico.

#### Scenario: Diagnostico tecnico controlado
- **WHEN** soporte activa `errorsDebugOn()` o `window.__APP_RESPONSE_DEBUG__ = true` desde la consola
- **THEN** el diagnostico completo puede registrarse solo con `logAppResponseErrorDiagnostic` y sin persistir ni transmitir payloads tecnicos.

### Requirement: Detalle funcional Jira
El sistema SHALL considerar las reglas detalladas del ticket.

#### Scenario: Reglas del ticket
- # JIRA-02 — Infraestructura visual aislada y reversible
- 
- ## Prompt para Jira
- 
- **Rol:** Actúa como arquitecto frontend senior para aplicaciones ASP.NET WebForms, experto en CSS incremental, carga de recursos legacy y mecanismos de feature flag reversibles.
- 
- Implementa la infraestructura opt-in de modernización del centro de trabajo Workflow. Debe cargar una capa visual aislada sin modificar el comportamiento existente y debe poder desactivarse sin despliegue de lógica de negocio. La implementación parte de la decisión de corte de JIRA-00: no se añade una segunda capa que coexista sin control con recursos visuales ya activos.
- 
- ### Alcance
- 
- - Crear `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js`.
- - Migrar, aislar o retirar de la ruta de carga los recursos preexistentes que JIRA-00 haya clasificado como incompatibles; ninguna capa moderna debe tener efectos cuando el modo esté apagado.
- - Cargarlos después de todos los recursos CSS y JavaScript legacy relevantes, en particular después de `Webworkflow.js`, con versión de caché explícita y orden verificado en el HTML entregado.
- - Definir una bandera de entorno/configuración evaluada en servidor: `WorkflowCentroTrabajoModernEnabled`, con valor predeterminado `false`, y una lista de perfiles piloto evaluada en servidor. La página emite la clase raíz `workflow-centro-trabajo-moderno` solo cuando ambas condiciones estén aprobadas.
- - Definir clases de subcapa emitidas por el mismo mecanismo: `ctw-layer-layout`, `ctw-layer-actions`, `ctw-layer-documents` y `ctw-layer-a11y`. Deben tener dependencias y valores predeterminados documentados para permitir rollback parcial sin retirar una hoja compartida.
- - Definir contrato de selectores, variables CSS, capas `z-index` y clases de estado.
- - Implementar los tokens y componentes de `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`; el resultado visual debe reproducir el HTML base, no una interpretación libre.
- 
- ### Restricciones no negociables
- 
- - Sin la clase de activación la interfaz debe ser idéntica a la actual.
- - No sustituir HTML ni alterar controles ASP.NET.
- - El adaptador no puede modificar DOM, clases, atributos ni foco fuera del contenedor moderno; una clase agregada manualmente desde cliente no constituye segmentación válida del piloto.
- - No habilitar por CSS una acción que el servidor haya ocultado por permisos.
- - El adaptador no puede lanzar errores que bloqueen scripts existentes.
- 
- ### Entregables técnicos
- 
- 1. `01-ArquitecturaActivacion.md`: bandera, perfiles piloto, punto de evaluación servidor, carga, selectores permitidos, subcapas y rollback.
- 2. `02-ContratoCSS.md`: variables, breakpoints y `z-index`.
- 3. `03-PlanCutoverCapasPrevias.md`: destino de cada recurso inventariado en JIRA-00 y prueba de ausencia de efectos con modo apagado.
- 4. `04-PruebasActivacion.md`: evidencia modo apagado/encendido y de cada rollback parcial.
- 5. Hoja CSS con componentes scoped: `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar`.
- 
- ### Criterios de aceptación
- 
- - Con el flag maestro apagado no hay diferencia visual, mutación DOM ni error respecto a la línea base aprobada de JIRA-01.
- - Activar/desactivar la clase raíz o una subcapa cambia solo la presentación de su alcance documentado.
- - No hay errores JavaScript en consola durante carga ni postback.
- - La reversión maestra o parcial no requiere revertir eventos, redeploy de lógica ni modificación de datos.
- 
- ### Pruebas requeridas
- 
- - Comparación visual y funcional con bandera en `0` y `1`.
- - Validar usuario fuera/dentro de perfil piloto; el modo moderno no se habilita mediante parámetro o manipulación de cliente.
- - Validar rollback maestro y de cada subcapa que haya sido habilitada.
- - Validar caché tras recarga forzada y navegación interna.
- 
- ### Reversión
- 
- Desactivar `WorkflowCentroTrabajoModernEnabled` para recuperación total o la subcapa documentada para recuperación parcial; mantener recursos publicados e inertes para no generar referencias rotas.
