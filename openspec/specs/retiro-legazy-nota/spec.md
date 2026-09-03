# retiro-legazy-nota Specification

## Purpose

TBD - Define the purpose of the retiro-legazy-nota capability after archive.

## Requirements

### Requirement: Retiro atómico de la rutina duplicada

El sistema SHALL retirar `Class_anotacion_tarea.Eliminar_nota_tarea_workflow` únicamente cuando el inventario estático confirme que no tiene consumidores, SHALL conservar `Eliminar_nota_service_workflow` y SHALL mantener disponible el contrato moderno `EliminarNota`. Trazabilidad: D-01, RQ-01.

#### Scenario: El candidato no tiene consumidores

- **WHEN** se busca `Eliminar_nota_tarea_workflow` en código compilable, scripts, configuración de proyecto y pruebas
- **THEN** no existe ninguna invocación ni referencia diferente de la definición candidata antes de retirarla

#### Scenario: Las rutas de borrado activas permanecen

- **WHEN** se inspecciona el código después del retiro
- **THEN** `WebServiceWorkflow.asmx.vb` continúa invocando `Eliminar_nota_service_workflow`
- **AND** `WebServiceWorkflowNotesModern.asmx.vb` continúa exponiendo `EliminarNota`

#### Scenario: Se descubre un consumidor dinámico o textual

- **WHEN** el inventario, la compilación o la regresión detecta un consumidor del candidato
- **THEN** la rutina no se retira o se restaura mediante el rollback atómico

### Requirement: Conservación de legacy con referencias vivas

El sistema SHALL conservar los endpoints `Service_*_nota_tarea_workflow`, `WebFormAnotacion.aspx(.vb)` y los scripts compartidos mientras exista al menos un consumidor verificable fuera del Centro de Trabajo. El consumidor legacy exclusivo de `Webworkflow` SHALL retirarse una vez validada su sustitución moderna. Trazabilidad: D-02, RQ-02.

#### Scenario: Endpoint legacy consumido

- **WHEN** el inventario encuentra llamadas desde `js/workflow/Webworkflow.js`, radicación, gestión de correspondencia o consulta de tareas
- **THEN** el endpoint ASMX correspondiente y su implementación permanecen fuera del retiro de DOC-45

#### Scenario: Página incluida en el proyecto

- **WHEN** `WebFormAnotacion.aspx` continúa incluida en `GestionDocumental-Docuarchi.net.vbproj` o conserva un uso operativo no descartado
- **THEN** la página, su code-behind, designer y recursos asociados no se eliminan

#### Scenario: Revisión del diff

- **WHEN** se compara DOC-45 con su base
- **THEN** no hay cambios funcionales en módulos externos ni eliminaciones adicionales a la pieza aprobada

### Requirement: Evidencia integrada y protegida

El sistema SHALL reutilizar exclusivamente el arnés de `tools/e2e`, SHALL exigir autorización explícita antes de autenticación, mutación o activación temporal del gate y SHALL producir únicamente evidencia saneada. Trazabilidad: D-03, RQ-03.

#### Scenario: Validación sin autorización de ambiente

- **WHEN** no existe autorización literal para el ambiente, cuenta y tarea descartable
- **THEN** solo se ejecutan inventario, política, sintaxis, pruebas no autenticadas y compilación
- **AND** la E2E real queda registrada como bloqueo, sin mocks ni simulaciones sustitutivas

#### Scenario: E2E autorizada

- **WHEN** están presentes las autorizaciones exigidas por `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`
- **THEN** la sesión reutiliza `authenticated-workflow-session.cjs`
- **AND** las verificaciones de base de datos son sentencias `SELECT` parametrizadas permitidas
- **AND** se cubren autorización, tarea explícita, lectura no mutante, CRUD, idempotencia, versión/conflicto, auditoría y aislamiento aplicables

#### Scenario: Evidencia saneada

- **WHEN** finaliza una prueba
- **THEN** la evidencia contiene solo estados, códigos, conteos, latencias y huellas permitidas
- **AND** no contiene credenciales, cookies, tokens, cadenas de conexión, contenido de notas ni cuerpos HTTP

### Requirement: Rollback y gate seguro

El sistema SHALL conservar un rollback atómico por control de versiones sin mantener doble presentación activa y SHALL dejar `WorkflowCentroTrabajoModernActive=false`, con usuarios y grupos vacíos, al terminar cualquier corrida incluso si falla. Trazabilidad: D-04, RQ-04.

#### Scenario: Finalización exitosa

- **WHEN** una corrida autorizada concluye correctamente
- **THEN** el runner restaura el gate a `false`
- **AND** restaura usuarios y grupos a listas vacías
- **AND** confirma que no existe doble operación por acción

#### Scenario: Error durante la corrida

- **WHEN** ocurre una excepción, timeout o aserción fallida después de habilitar temporalmente el gate
- **THEN** el bloque de limpieza intenta restaurar la configuración segura
- **AND** una restauración no verificable bloquea el cierre del cambio

#### Scenario: Rollback del retiro

- **WHEN** la compilación o regresión posterior demuestra que la función retirada era necesaria
- **THEN** se restaura únicamente `Eliminar_nota_tarea_workflow` desde el commit anterior
- **AND** no se revierten datos ni se ejecuta DDL

### Requirement: Retiro de la presentación legacy del Centro de Trabajo

El sistema SHALL presentar Notas exclusivamente mediante un acceso moderno con contador y `Panel_notas_modernas` como diálogo superpuesto cuando exista un contexto Workflow válido, y SHALL retirar del consumidor `Webworkflow` los controles, eventos y llamadas JavaScript de la interfaz antigua. Los contratos compartidos con otros módulos SHALL permanecer disponibles. Trazabilidad: D-05, RQ-05.

#### Scenario: Tarea operable con contexto válido

- **WHEN** el usuario selecciona una tarea operable y el contexto Workflow es válido
- **THEN** la barra muestra la opción moderna Notas con su contador
- **AND** al activarla se abre y enfoca el diálogo moderno funcional sobre el Centro de Trabajo
- **AND** no existe un botón, modal, GridView ni acción legacy de Notas en el Centro de Trabajo

#### Scenario: Confirmación y estabilidad del modal moderno

- **WHEN** el usuario solicita eliminar una nota propia desde el diálogo moderno
- **THEN** el sistema presenta un diálogo auxiliar de confirmación y no un confirmador nativo JavaScript
- **AND** cancelar conserva la nota sin mutación
- **AND** confirmar ejecuta una sola eliminación y el mensaje exitoso se limpia automáticamente
- **AND** el diálogo principal conserva su tamaño mientras la lista utiliza scroll interno cuando es necesario

#### Scenario: Inventario posterior al retiro

- **WHEN** se buscan `Panel_Buttonanotacion`, `ImageButtonanotacion`, sus handlers y las llamadas `Service_*_nota_tarea_workflow` en el consumidor Workflow
- **THEN** no quedan referencias productivas en `Webworkflow.aspx`, `Webworkflow.aspx.vb`, `Classselecciotarea.vb` ni `js/workflow/Webworkflow.js`
- **AND** permanecen las referencias requeridas por Radicación y Correspondencia

### Requirement: E2E real integrada al retiro

El sistema SHALL tratar código, pruebas focales, compilación, E2E real autorizada y evidencia saneada como una única unidad de entrega. La E2E SHALL reutilizar exclusivamente la infraestructura existente de `tools/e2e`, SHALL ejecutarse sobre la implementación corregida y SHALL dejar la configuración de seguridad en su estado requerido. Trazabilidad: D-06, RQ-06.

#### Scenario: Cobertura final autorizada

- **WHEN** existen ambiente, cuenta, tarea descartable y autorización explícita
- **THEN** el ejecutor reutiliza la sesión autenticada y configuración existentes
- **AND** verifica que el panel moderno esté visible y los controles legacy no existan en el DOM
- **AND** cubre lecturas negativas, CRUD real y una sola mutación por acción
- **AND** registra únicamente resultados, conteos, latencias y huellas saneadas

#### Scenario: Gate innecesario para la política oficial

- **WHEN** un contexto Workflow válido activa la experiencia moderna mediante la política oficial
- **THEN** la E2E no modifica `Web.config` para forzar visibilidad
- **AND** confirma al inicio y al final `WorkflowCentroTrabajoModernActive=false` y audiencias vacías

#### Scenario: Falta de autorización o datos

- **WHEN** falta una autorización, cuenta, ambiente o tarea descartable requerida
- **THEN** la prueba real no se ejecuta
- **AND** DOC-45 permanece abierto con un bloqueo explícito, sin mocks, simulaciones ni evidencia previa como sustituto

### Requirement: Lectura completa y gestión exclusiva por propiedad

El sistema SHALL permitir que un usuario autorizado para consultar una tarea visualice completas sus notas, incluidas las ajenas, y SHALL reservar la edición y eliminación exclusivamente al autor autenticado. La capacidad de gestión SHALL calcularse en servidor y las mutaciones SHALL conservar validación de propiedad en persistencia. Trazabilidad: D-07, RQ-07.

#### Scenario: Nota extensa en modo lectura y nota ajena protegida

- **GIVEN** una nota extensa consultable y una nota perteneciente a otro usuario dentro de la misma tarea
- **WHEN** el usuario abre la gestión moderna de notas
- **THEN** la lista ofrece `Ver nota completa` para el contenido extenso y no renderiza `Editar` ni `Eliminar` para la nota ajena
- **AND** el diálogo auxiliar muestra el texto como contenido seguro, mantiene tamaño estable, usa scroll interno y devuelve el foco al cerrarse

#### Scenario: Defensa en profundidad ante mutación ajena

- **WHEN** un usuario intenta actualizar o eliminar directamente una nota ajena de la misma tarea
- **THEN** el servidor responde `NotOwner`
- **AND** no cambia la nota, su versión ni la auditoría

#### Scenario: Nota propia

- **WHEN** el autor consulta una nota propia
- **THEN** la capacidad de gestión emitida por el servidor es verdadera
- **AND** la interfaz ofrece edición y eliminación, conservando control de versión y una sola mutación por acción

### Requirement: Creación descubrible en estado vacío

El sistema SHALL presentar una acción explícita de creación cuando la tarea no tenga notas. Trazabilidad: D-08, RQ-08.

#### Scenario: Tarea sin notas

- **WHEN** el contador de notas cargado es cero
- **THEN** el acceso visible muestra `Nueva nota` junto al contador `0` y su nombre accesible indica creación
- **AND** el primer clic abre el modal y enfoca directamente el editor de una nueva nota
- **AND** después de eliminar la única nota el acceso vuelve al estado `Nueva nota 0`

### Requirement: Compatibilidad visual de tareas e índice

El sistema SHALL conservar el color y el glifo reconocible de las acciones existentes de la tabla de tareas y de los controles mostrar/ocultar índice cuando la presentación moderna esté activa. Trazabilidad: D-09, RQ-09.

#### Scenario: Acciones no afectadas por el retiro de Notas

- **WHEN** el usuario consulta la tabla de tareas o alterna el panel de índice
- **THEN** las acciones Ver documentos, Detalle, Retomar y Asignar conservan fondos diferenciados y glifos blancos visibles
- **AND** los disparadores mostrar y ocultar índice conservan un glifo con contraste
- **AND** sus eventos, permisos y comportamiento funcional permanecen sin cambios

### Requirement: Acceso de Notas después de actualización parcial

El sistema SHALL mantener operativo el acceso moderno de Notas cuando ASP.NET Web Forms reemplace la barra de acciones durante un postback parcial. Trazabilidad: D-10, RQ-10.

#### Scenario: Selección asíncrona de otra tarea

- **WHEN** el usuario selecciona una tarea mediante la lista sin recargar la página completa
- **THEN** el botón de Notas recién renderizado abre el modal al primer clic
- **AND** el cliente consulta usando el identificador explícito de la tarea actual
- **AND** no restaura controles legacy ni provoca una mutación adicional
