<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->
## Purpose

Definir la carga documental segura de Radicación Entrante, su estado de interfaz y la separación contractual respecto de Radicación Simplificada y los demás consumidores compartidos.

## ADDED Requirements

### Requirement: RQ-01 — Origen explícito de Radicación Entrante

El sistema SHALL identificar la carga iniciada en Radicación Entrante mediante `ADJUNTARADICACION_CLASICA`, conforme a D-01.

#### Scenario: Apertura del cargador clásico

- **WHEN** `ActivaAdjuntarDocumentoRadicacion` construye sus opciones
- **THEN** SHALL conservar `NameLoadProceso` como `ADJUNTARADICACION`
- **AND** SHALL enviar `evento_adjunta` como `ADJUNTARADICACION_CLASICA`
- **AND** SHALL NOT enviar un ID de estado como fuente de autorización

### Requirement: RQ-02 — Enrutamiento clásico de diez argumentos

El handler SHALL seleccionar una rama exclusiva para Radicación Entrante y conservar su respuesta vigente, conforme a D-02.

#### Scenario: Evento clásico reconocido

- **WHEN** el handler recibe `ADJUNTARADICACION_CLASICA`
- **THEN** SHALL invocar exactamente una vez `UploadSaveFile` con diez argumentos
- **AND** SHALL completar los campos existentes de `uploadFiles`

#### Scenario: Separación respecto de Simplificada

- **WHEN** el handler recibe `ADJUNTARADICACION`
- **THEN** SHALL continuar invocando exclusivamente la sobrecarga de doce argumentos

### Requirement: RQ-03 — Contexto clásico autorizado por sesión

La ruta clásica SHALL resolver el registro seleccionado desde `RA_ID_REGISTRO_RADICADO` y rechazar contextos inválidos antes del almacenamiento, conforme a D-03.

#### Scenario: Registro clásico válido

- **WHEN** la sesión corresponde a Radicación Entrante y contiene una plantilla activa y un registro positivo y resoluble
- **THEN** SHALL obtener desde servidor su radicado, tarea y trámite
- **AND** SHALL comprobar que el registro pertenece a la plantilla activa
- **AND** SHALL llamar una sola vez `PreAlmacenaDocumentosRadicacion`
- **AND** SHALL propagar la tarea resuelta al resultado

#### Scenario: Contexto clásico inválido

- **WHEN** el módulo no es Radicación Entrante, la plantilla no es válida o no coincide, la sesión no contiene registro, el ID es cero o la estructura no puede resolverse
- **THEN** SHALL devolver un error funcional
- **AND** SHALL NOT iniciar el prealmacenamiento

### Requirement: RQ-04 — Protección de Radicación Simplificada

La corrección SHALL mantener sin cambios funcionales el contrato autoritativo DOC-85, conforme a D-04.

#### Scenario: Regresión DOC-85

- **WHEN** se ejecutan los contratos de Radicación Simplificada
- **THEN** SHALL conservar ID explícito positivo, resolución por usuario y comparación del radicado informativo
- **AND** SHALL NOT usar `RA_ID_REGISTRO_RADICADO` ni `DAT_ADIC_TAR` como fallback

#### Scenario: Superficies protegidas

- **WHEN** se revisa el diff funcional
- **THEN** SHALL NOT contener cambios en el cliente o ASPX de Radicación Simplificada, `FileUploadHandler.js`, el servicio ni sus repositorios

### Requirement: RQ-05 — Proyección, caché y evidencia clásica

La carga clásica SHALL actualizar la interfaz una sola vez y usar una versión nueva de su script, conforme a D-05.

#### Scenario: Carga clásica exitosa

- **WHEN** el almacenamiento devuelve `YES`
- **THEN** SHALL devolver radicado, tarea, gabinete, tipología e imagen
- **AND** SHALL insertar una sola fila mediante `insert_row_documento_relacionado`
- **AND** SHALL NOT ejecutar postback ni recarga

#### Scenario: Validación controlada

- **WHEN** termina la implementación
- **THEN** SHALL pasar contratos focales, regresión DOC-85, compilación y `git diff --check`
- **AND** SHALL existir un escenario E2E clásico ejecutable mediante perfil no sensible, runner interactivo, autenticación compartida y consultas de control exclusivamente `SELECT`
- **AND** SHALL verificar selección del registro clásico, evento `ADJUNTARADICACION_CLASICA`, ausencia de los campos autoritativos de Simplificada, persistencia única y una sola fila visible sin postback
- **AND** la E2E real SHALL ejecutarse solo con autorización explícita o registrar bloqueo

### Requirement: RQ-06 — No regresión de consumidores compartidos

La corrección SHALL preservar todos los eventos, firmas y recorridos existentes del componente compartido fuera de la nueva rama clásica, conforme a D-06.

#### Scenario: Inventario estable de eventos

- **WHEN** se compara el handler antes y después del cambio
- **THEN** SHALL conservar `GESTION_PQRS`, `ADJUNTAVERSION`, `REMPLAZAVERSION`, `INTRUESII`, `INTVIRTUALSII`, `MIGRACION`, `GESTION_RESPUESTA`, `WORKFLOWSELECCION`, `WORKFLOWENLACE`, `ADJUNTARADICACION`, `PRODUCCION`, `SUBE_RESPUESTA`, `SUBE_ANEXO` y `RADICA_WORKFLOW`
- **AND** SHALL agregar únicamente `ADJUNTARADICACION_CLASICA`

#### Scenario: Firmas y contratos estables

- **WHEN** se inventarían las llamadas a `UploadSaveFile`
- **THEN** las llamadas existentes SHALL conservar su aridad y rama propietaria
- **AND** la única llamada nueva SHALL ser la firma clásica de diez argumentos
- **AND** `workflow/Webworkflow.aspx.vb` y `webservice/WebServiceRadicacion.asmx.vb` SHALL permanecer sin cambios funcionales

#### Scenario: Regresión de módulos

- **WHEN** se ejecuta la matriz automatizada del cargador compartido
- **THEN** SHALL pasar la cobertura de Radicación Simplificada, Producción, Workflow, versiones, migración, SII, PQRS, gestión de respuestas, correspondencia y enlace de radicados

### Requirement: RQ-07 — Estado seguro de pestañas sin asignación

Radicación Entrante SHALL mantener inaccesible el soporte documental mientras no exista un radicado asignado, independientemente de la cantidad de tareas pendientes, conforme a D-07.

#### Scenario: Inicio sin radicado asignado y con pendientes

- **WHEN** `Hidden_radicado_seleccion` está vacío al iniciar o reinicializar la pantalla completa
- **THEN** SHALL seleccionar y habilitar `home-radicador`
- **AND** SHALL desactivar `soporte-envio_nav` y ocultar su contenido y acciones documentales
- **AND** SHALL mantener visible y operativo el acceso `A1` a la lista de radicados pendientes con su total
- **AND** `Hidden_numero_rad_pend` SHALL NOT habilitar ni seleccionar el soporte documental

#### Scenario: Transiciones de asignación

- **WHEN** el postback parcial de asignación devuelve `Hidden_result_boton_tool = "YES"`
- **THEN** SHALL seleccionar y habilitar `soporte-envio_nav`
- **AND** SHALL desactivar `home-radicador`
- **AND** SHALL NOT rechazar la transición leyendo un valor de DOM anterior a la respuesta parcial
- **WHEN** se inicia un nuevo radicado o termina la tarea asignada
- **THEN** SHALL volver a `home-radicador` y desactivar `soporte-envio_nav`

#### Scenario: Reinicialización repetida

- **WHEN** la inicialización o una transición se ejecuta más de una vez
- **THEN** SHALL conservar una sola pestaña activa y los atributos `aria-selected` y `aria-disabled` coherentes

#### Scenario: Estado sincronizado en postback parcial

- **WHEN** una asignación actualiza el registro seleccionado y el total de pendientes
- **THEN** `Hidden_radicado_seleccion` SHALL pertenecer al `UpdatePanel_boton_tool` actualizado por `Asignar_radicado`
- **AND** `Panel_pendiente_radicado`, `Label_numero_item` y `Hidden_numero_rad_pend` SHALL pertenecer a un `UpdatePanel` visible que se actualice en postbacks parciales
