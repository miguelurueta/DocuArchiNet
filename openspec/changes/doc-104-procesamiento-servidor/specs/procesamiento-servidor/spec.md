<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10 -->

## Purpose

Garantizar que el pegado servidor de un grafo produzca un PDF válido, compatible y recuperable sin colisiones de temporales, filtraciones ni expectativas falsas de descarga parcial.

## ADDED Requirements

### Requirement: Validación y certificado antes de escribir
DOC-104 SHALL conservar la firma del WebMethod y el DTO actuales, aceptar exactamente una anotación válida y comprobar el certificado digital antes de crear o modificar cualquier archivo. <!-- RQ-01; D-01 -->

#### Scenario: Documento firmado
- **WHEN** el registro de certificado indica que el documento está firmado
- **THEN** la operación se rechaza con mensaje público y no crea imagen, lock, staged, backup ni auditoría de mutación

#### Scenario: Payload inválido
- **WHEN** el cuerpo no contiene exactamente un `Class_itext_anotate` utilizable
- **THEN** el servidor rechaza sin invocar iTextSharp y sin incluir una excepción en la respuesta

### Requirement: Límites numéricos y binarios
DOC-104 SHALL convertir y validar explícitamente página, canvas, coordenadas, tamaño, escala, rotación, transparencia, tolerancia e imagen antes de crear `PdfStamper`. El base64 decodificado SHALL NOT superar 10 MiB y la imagen SHALL NOT superar 4096 x 4096. <!-- RQ-02; D-02 -->

#### Scenario: Geometría no finita o fuera de dominio
- **WHEN** un número es nulo, no finito, negativo donde debe ser positivo, o la rotación no pertenece a 0/90/180/270
- **THEN** la operación falla antes de escribir un PDF temporal

#### Scenario: Página superior al documento
- **WHEN** la página solicitada excede `PdfReader.NumberOfPages`
- **THEN** no se crea `PdfStamper`, el original queda idéntico y los recursos se liberan

### Requirement: Exclusión por documento y aislamiento por operación
DOC-104 SHALL usar un identificador GUID por invocación, rutas que pertenezcan exclusivamente a esa invocación y un lock de sistema de archivos por documento. SHALL NOT reutilizar nombres basados únicamente en usuario ni limpiar todo el directorio compartido. <!-- RQ-03; D-03 -->

#### Scenario: Dos operaciones sobre el mismo PDF
- **WHEN** una operación mantiene el lock y otra intenta modificar el mismo documento
- **THEN** la segunda recibe conflicto saneado sin tocar temporales, original o backup de la primera

#### Scenario: Documentos distintos en paralelo
- **WHEN** dos operaciones autorizadas modifican documentos diferentes, incluso para el mismo usuario
- **THEN** usan rutas distintas y ninguna elimina, lee o sobrescribe artefactos de la otra

### Requirement: Transparencia eficiente y determinista
DOC-104 SHALL aplicar transparencia de forma síncrona mediante acceso bloqueado al bitmap, sin `GetPixel/SetPixel`, haciendo transparente solo un píxel cuya diferencia en cada canal RGB sea estrictamente menor que la tolerancia. <!-- RQ-04; D-04 -->

#### Scenario: Píxel dentro de tolerancia
- **WHEN** las diferencias R, G y B cumplen el umbral
- **THEN** el alfa resultante es cero y el algoritmo no altera píxeles ajenos

#### Scenario: Transparencia desactivada
- **WHEN** `aplica_transparente` vale cero
- **THEN** la imagen se estampa sin ejecutar la transformación de transparencia

### Requirement: Compatibilidad visual y cierre de recursos
DOC-104 SHALL conservar el tamaño efectivo legacy 90 x 90 y el cálculo de ubicación vigente para los fixtures aprobados. Todo `Bitmap`, `PdfReader`, `PdfStamper`, stream e imagen temporal SHALL quedar liberado en éxito o fallo. <!-- RQ-05; D-05 -->

#### Scenario: Página normal o rotada
- **WHEN** se procesa un fixture aprobado con rotación 0, 90, 180 o 270
- **THEN** la comparación visual ubica el grafo en la posición esperada y con tamaño efectivo 90 x 90

#### Scenario: Excepción durante el estampado
- **WHEN** falla cualquier etapa después de abrir recursos
- **THEN** los archivos involucrados pueden reabrirse, renombrarse o eliminarse inmediatamente

### Requirement: Reemplazo validado y recuperable
DOC-104 SHALL escribir en un staged adyacente al original, cerrarlo, validarlo y usar `File.Replace` con backup. SHALL restaurar el backup si la validación posterior falla y SHALL NOT degradar a copia destructiva cuando el reemplazo seguro no esté soportado. <!-- RQ-06; D-06 -->

#### Scenario: Fallo anterior al reemplazo
- **WHEN** falla carga, transparencia, stamper o validación del staged
- **THEN** el hash y la estructura del original permanecen iguales y se eliminan artefactos propios

#### Scenario: Resultado posterior inválido
- **WHEN** el archivo reemplazado no puede abrirse o no conserva el número de páginas
- **THEN** el backup se restaura y la operación no anuncia commit

#### Scenario: Commit válido
- **WHEN** staged y original reemplazado pasan todas las verificaciones
- **THEN** se elimina el backup residual y se responde éxito una sola vez

### Requirement: Refresco sin extracción de una página
DOC-104 SHALL retirar `Create_file_pagina(...)` del flujo de guardado y SHALL refrescar desde la URL vigente con invalidación de caché. El cliente SHALL solicitar `getPage(i)` y mantener el transporte `stream` para ese refresco. <!-- RQ-07; D-07 -->

#### Scenario: Grafo guardado en página intermedia
- **WHEN** se confirma un grafo en la página 7
- **THEN** `_updatePage(7, url_salida)` carga la página 7 del documento actualizado y conserva página activa, zoom y scroll

#### Scenario: Refresco fallido
- **WHEN** el documento actualizado no puede descargarse o renderizarse
- **THEN** DOC-103 conserva el overlay y clasifica el resultado como incierto sin repetir el POST

### Requirement: Clasificación de transporte posterior
DOC-104 SHALL clasificar el PDF realmente persistido mediante `EstrategiaCargaPdf.Seleccionar(...)` después del reemplazo y SHALL mantener la mutación desacoplada de la lectura. <!-- RQ-08; D-08 -->

#### Scenario: iTextSharp elimina linearización
- **WHEN** el original era linearizado y el resultado ya no contiene `/Linearized` en la cabecera inspeccionada
- **THEN** la clasificación resultante es `stream` y no se afirma descarga parcial

#### Scenario: Resultado conserva linearización
- **WHEN** el PDF final tiene al menos 1 MiB y conserva `/Linearized` válido
- **THEN** una apertura posterior puede seleccionar `range` sin que el guardado ejecute una herramienta externa

### Requirement: Errores y auditoría sin datos sensibles
DOC-104 SHALL retornar mensajes públicos cerrados y registrar únicamente identificadores funcionales permitidos, página, operación, resultado y modo. SHALL NOT registrar rutas, URL completa, base64, cookies, tokens, cadenas de conexión o texto de excepción. <!-- RQ-09; D-09 -->

#### Scenario: Error previo al commit
- **WHEN** ocurre una excepción antes de reemplazar
- **THEN** `error_sistema` contiene un mensaje saneado y la respuesta no revela el detalle interno

#### Scenario: Auditoría falla después del commit
- **WHEN** el PDF ya fue confirmado pero falla `Registra_log_procesing_image`
- **THEN** la respuesta mantiene `error_sistema = "YES"`, informa advertencia mediante `error_log` y no repite ni revierte la mutación

### Requirement: Evidencia descartable y autorizada
DOC-104 SHALL caracterizar el comportamiento anterior y verificar el nuevo mediante fixtures descartables, pruebas focales, MSBuild y E2E reutilizando exclusivamente `tools/e2e`. <!-- RQ-10; D-10 -->

#### Scenario: Ejecución sin autorización E2E
- **WHEN** falta autorización de ambiente, cuenta o PDF descartable
- **THEN** el runner se detiene antes de autenticar o mutar

#### Scenario: Validación integral autorizada
- **WHEN** las autorizaciones están vigentes
- **THEN** una acción produce un POST y una persistencia, los controles antes/después son solo `SELECT` y la evidencia saneada cubre concurrencia, rollback, equivalencia visual, `/Linearized` y tiempos por fase
