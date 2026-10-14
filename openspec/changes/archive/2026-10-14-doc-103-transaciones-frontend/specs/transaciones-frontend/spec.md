<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->

## Purpose

Garantizar que el guardado de un grafo en el visor PDF sea una transacción cliente esperable, de envío único, recuperable y segura, sin alterar el contrato servidor existente.

## ADDED Requirements

### Requirement: Propietario único de la transacción
DOC-103 SHALL implementar la transacción de guardado únicamente en el script productivo cargado por el visor y SHALL NOT reactivar ni duplicar la lógica en el visor anterior comentado. <!-- RQ-01; D-01 -->

#### Scenario: Ruta productiva
- **WHEN** se abre `pdf_anotate_view/index.html`
- **THEN** `pdf_viewer_anotate.js` posee el flujo de guardado y `pdfannotate.js` no se ejecuta

### Requirement: Confirmación autoritativa antes de retirar el overlay
El visor SHALL mantener el overlay local hasta que carga de imagen, POST, respuesta funcional y refresco de página hayan terminado correctamente. <!-- RQ-02; D-02 -->

#### Scenario: Guardado y refresco exitosos
- **WHEN** el servidor confirma la mutación y la página afectada termina de actualizarse
- **THEN** el visor retira únicamente el overlay persistido y resuelve la operación como confirmada

#### Scenario: Fallo antes de confirmar
- **WHEN** falla validación, carga de imagen, transporte, contrato funcional o refresco
- **THEN** el overlay permanece disponible y la UI no anuncia éxito

### Requirement: Single-flight por grafo
El visor SHALL permitir como máximo una solicitud de guardado pendiente por combinación de documento, página y overlay. <!-- RQ-03; D-03 -->

#### Scenario: Doble clic o eventos repetidos
- **WHEN** se solicita guardar varias veces mientras la primera operación continúa pendiente
- **THEN** se emite un solo POST y las invocaciones posteriores no crean otra mutación

#### Scenario: Liberación determinista
- **WHEN** la operación termina con éxito, fallo o cancelación
- **THEN** el registro pendiente se elimina y los controles existentes se rehabilitan en `finally`

### Requirement: Timeout y cancelación controlados
El transporte SHALL declarar un timeout de 30 segundos, conservar una operación cancelable y abortarla cuando se abandona el visor, sin reintento automático. <!-- RQ-04; D-04 -->

#### Scenario: Timeout o abandono
- **WHEN** expira el timeout o ocurre `pagehide` con un POST pendiente
- **THEN** el transporte se aborta, el bloqueo local se libera, el overlay se conserva y no se genera otro POST

#### Scenario: Resultado incierto
- **WHEN** la conexión se pierde o el servidor falla después de recibir potencialmente la solicitud
- **THEN** la UI informa un resultado incierto sin afirmar rollback ni repetir la mutación

### Requirement: Normalización estricta de respuesta ASMX
El cliente SHALL aceptar éxito únicamente cuando `data.d` sea una colección válida cuyo resultado declare éxito funcional y proporcione la URL de la página actualizada. <!-- RQ-05; D-05 -->

#### Scenario: Commit confirmado
- **WHEN** el primer resultado contiene `error_sistema = "YES"`, `url_salida` válida y `error_log = "YES"`
- **THEN** la operación se clasifica `committed` y continúa al refresco

#### Scenario: Commit con advertencia
- **WHEN** el PDF fue confirmado pero `error_log` informa fallo posterior
- **THEN** la operación se clasifica `committed_with_warning`, refresca la página y no se reintenta automáticamente

#### Scenario: HTTP 200 inválido
- **WHEN** `data.d` falta, no es colección, está vacío, declara error funcional o no contiene URL de salida
- **THEN** el cliente no declara éxito y conserva el overlay

### Requirement: Errores públicos saneados
El visor SHALL presentar errores mediante su componente de aviso con textos públicos fijos y SHALL NOT mostrar respuestas, payloads, rutas, usuarios o excepciones crudas. <!-- RQ-06; D-06 -->

#### Scenario: Matriz de fallos
- **WHEN** ocurre 401/403, 404, 500, error de parser, timeout, aborto o respuesta incompleta
- **THEN** se muestra la categoría pública correspondiente sin `responseText`, `errorThrown` ni `ex.message`

### Requirement: Refresco esperado de una página
El visor SHALL esperar la carga, render y actualización de caché de la página persistida usando el transporte compatible de DOC-102 y SHALL preservar navegación y escala. <!-- RQ-07; D-07 -->

#### Scenario: Página actualizada
- **WHEN** el ASMX devuelve `url_salida` válida
- **THEN** solo la página afectada se descarga y renderiza antes de resolver, sin cambiar página activa, zoom o scroll

#### Scenario: Refresco fallido
- **WHEN** descarga, render o actualización de caché falla
- **THEN** la promesa rechaza, el overlay permanece y el visor no se reinicializa

### Requirement: Compatibilidad de fronteras existentes
DOC-103 SHALL conservar sin cambios el WebMethod, DTO, payload, algoritmo iText, temporales, firma, Wacom, impresión, descarga y permisos vigentes. <!-- RQ-08; D-08 -->

#### Scenario: Regresión transversal
- **WHEN** se valida el cambio
- **THEN** las capacidades no relacionadas mantienen sus contratos y `pdfannotate.js` continúa inactivo

### Requirement: Evidencia integral y protegida
DOC-103 SHALL probar la transacción con dobles sin escritura y con una E2E real únicamente cuando exista autorización específica y un PDF descartable. <!-- RQ-09; D-09 -->

#### Scenario: Prueba controlada sin escritura
- **WHEN** se ejecuta la suite local
- **THEN** un doble ASMX cubre éxito, advertencia, fallos, concurrencia, timeout, aborto y refresco sin modificar documentos

#### Scenario: E2E mutadora autorizada
- **WHEN** ambiente, cuenta y PDF descartable están expresamente autorizados
- **THEN** una acción genera un POST y un efecto, verificados con controles antes/después exclusivamente `SELECT` y evidencia saneada

#### Scenario: E2E sin autorización
- **WHEN** falta cualquiera de las autorizaciones o el documento descartable
- **THEN** el runner se detiene antes de autenticar o mutar y registra el bloqueo sin inventar resultados
