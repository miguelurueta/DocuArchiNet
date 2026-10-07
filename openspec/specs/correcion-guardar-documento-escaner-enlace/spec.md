# Correccion de guardado de documento escaneado en Enlace Specification

## Purpose

Define el guardado seguro y visualmente estable de documentos digitalizados desde Enlace, preservando el escaner compartido y los recorridos existentes.

## Requirements

### Requirement: RQ-01 Async postback conservado

Trazabilidad: D-01.

El sistema SHALL conservar `ButtonAlmacenar` como disparador asincrono del `UpdatePanel_boton_tool`, sin navegacion completa ni recarga del iframe.

#### Scenario: Inicio desde digitalizacion

- **WHEN** `activa_document_save()` ejecuta `window.parent.ButtonAlmacenar.click()` para `Hidden21=1`
- **THEN** Microsoft AJAX inicia el async postback existente
- **AND** no se agrega `PostBackTrigger`, `RegisterPostBackControl`, `location.reload` ni navegacion alternativa

### Requirement: RQ-02 Indicador compacto sin pantalla blanca

Trazabilidad: D-01, D-04.

Durante el async postback de `ButtonAlmacenar`, el sistema SHALL mantener perceptibles el formulario, el arbol y el visor y SHALL mostrar `#progres_bar` como progreso compacto y accesible.

#### Scenario: Almacenamiento en progreso

- **WHEN** `InitializeRequest` recibe `ButtonAlmacenar`
- **THEN** muestra el indicador con su semantica `role=status` y `aria-live=polite`
- **AND** no agrega `overlay_`, ancho/alto completo ni una superficie blanca que cubra el viewport

#### Scenario: Fin del postback

- **WHEN** finaliza el async postback con exito o error
- **THEN** el indicador queda oculto
- **AND** no quedan clases, dimensiones, posiciones ni bloqueos transitorios

### Requirement: RQ-03 Guardado tradicional invariable

Trazabilidad: D-02.

El sistema SHALL conservar el comportamiento visual previo de `Button_guardar_desicion`.

#### Scenario: Guardado por Button_guardar_desicion

- **WHEN** `InitializeRequest` recibe `Button_guardar_desicion`
- **THEN** continua llamando `posicion_update_pogres_modal('progres_bar')`
- **AND** `CheckStatus` retira `overlay_` al finalizar

### Requirement: RQ-04 Escaner compartido invariable

Trazabilidad: D-03, D-06.

La correccion SHALL permanecer fuera del componente compartido de digitalizacion y SHALL preservar todas las continuaciones de `Hidden21`.

#### Scenario: Contratos de consumidores compartidos

- **WHEN** se verifican los valores `Hidden21` 1, 2, 3, 4 y 5
- **THEN** siguen activando respectivamente `ButtonAlmacenar`, `Button_añade_documento`, `save_document_scan`, `save_document_scan` y `Button_save_replace_dig`
- **AND** no cambian `Gurdar_documento_htpp_server`, callbacks Dynamsoft, sesion, temporales ni formatos

### Requirement: RQ-05 Persistencia y proyeccion unicas

Trazabilidad: D-03, D-06.

Una aceptacion SHALL producir como maximo una ejecucion efectiva de almacenamiento y una proyeccion incremental del documento.

#### Scenario: Almacenamiento exitoso

- **WHEN** `UploadSaveFileScan` devuelve `YES`
- **THEN** `Hidden_result_load_` y `Hidden_date_row_` transportan el resultado existente
- **AND** `insert_row_documento_relacionado` agrega exactamente un nodo
- **AND** no se ejecutan retry, temporizador, recarga, `DataBind` ni segunda escritura

#### Scenario: Doble interaccion

- **WHEN** el usuario intenta aceptar dos veces durante la misma operacion
- **THEN** se conserva una sola persistencia efectiva y un solo nodo nuevo

### Requirement: RQ-06 Error sin bloqueo visual residual

Trazabilidad: D-04.

El sistema SHALL mantener el tratamiento de error existente y SHALL retirar el progreso al completar una respuesta rechazada o excepcional.

#### Scenario: Almacenamiento rechazado

- **WHEN** `UploadSaveFileScan` devuelve un valor diferente de `YES` o el async postback termina con error
- **THEN** se presenta el mensaje existente
- **AND** no se inserta un nodo
- **AND** el indicador no permanece visible ni bloquea la interfaz

### Requirement: RQ-07 Experiencia moderna oficial

Trazabilidad: D-05.

La solucion SHALL operar en la presentacion moderna oficial sin gates, usuarios, grupos o rutas visuales paralelas.

#### Scenario: Inspeccion de compatibilidad moderna

- **WHEN** se revisa la implementacion
- **THEN** no se reintroduce `WorkflowCentroTrabajoModernActive`
- **AND** no se condiciona la correccion por listas de usuarios o grupos
- **AND** los selectores globales compartidos conservan su contrato

### Requirement: RQ-08 Evidencia integral y segura

Trazabilidad: D-07.

La entrega SHALL incluir caracterizacion, pruebas focales y de regresion, compilacion, validacion estricta, QA y E2E real autorizada con evidencia saneada.

#### Scenario: Validacion local

- **WHEN** se valida el cambio antes de entrega
- **THEN** pasan la prueba estructural DOC-89, suites relacionadas, MSBuild, `git diff --check` y OpenSpec estricto

#### Scenario: E2E autenticada

- **WHEN** existe autorizacion explicita para ambiente, cuenta y recursos descartables
- **THEN** se reutiliza `tools/e2e` y se comprueban async postback, interfaz visible, una escritura, un nodo y limpieza final
- **AND** los controles de datos son exclusivamente `SELECT` parametrizados
- **AND** no se imprimen ni persisten secretos, cookies, tokens, cadenas de conexion o contenido documental

#### Scenario: Falta de autorizacion E2E

- **WHEN** falta cualquier autorizacion o recurso descartable requerido
- **THEN** se registra el bloqueo explicito
- **AND** no se sustituye la prueba real por mocks, inspeccion estatica o evidencia ficticia
