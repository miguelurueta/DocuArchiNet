# procesamiento-servidor Specification

## Purpose

Garantizar que el pegado servidor de un grafo produzca un PDF válido, compatible y recuperable sin colisiones de temporales, filtraciones ni expectativas falsas de descarga parcial.

## Requirements

### Requirement: Validación y certificado antes de escribir
El sistema SHALL conservar el contrato público vigente, aceptar exactamente una anotación válida y comprobar el certificado digital antes de crear o modificar cualquier archivo.

#### Scenario: Documento firmado
- **WHEN** el registro de certificado indica que el documento está firmado
- **THEN** la operación se rechaza con mensaje público y no crea imagen, lock, staged, backup ni auditoría de mutación

#### Scenario: Payload inválido
- **WHEN** el cuerpo no contiene exactamente una anotación utilizable
- **THEN** el servidor rechaza sin procesar el PDF y sin incluir una excepción en la respuesta

### Requirement: Límites numéricos y binarios
El sistema SHALL convertir y validar explícitamente página, canvas, coordenadas, tamaño, escala, rotación, transparencia, tolerancia e imagen antes de escribir el PDF. El base64 decodificado SHALL NOT superar 10 MiB y la imagen SHALL NOT superar 4096 x 4096.

#### Scenario: Geometría no finita o fuera de dominio
- **WHEN** un número es nulo, no finito, negativo donde debe ser positivo, o la rotación no pertenece a 0/90/180/270
- **THEN** la operación falla antes de escribir un PDF temporal

#### Scenario: Página superior al documento
- **WHEN** la página solicitada excede el número de páginas del documento
- **THEN** no se escribe un PDF temporal, el original queda idéntico y los recursos se liberan

### Requirement: Exclusión por documento y aislamiento por operación
El sistema SHALL usar un identificador único por invocación, rutas que pertenezcan exclusivamente a esa invocación y un lock de sistema de archivos por documento. SHALL NOT reutilizar nombres basados únicamente en usuario ni limpiar todo el directorio compartido.

#### Scenario: Dos operaciones sobre el mismo PDF
- **WHEN** una operación mantiene el lock y otra intenta modificar el mismo documento
- **THEN** la segunda recibe conflicto saneado sin tocar temporales, original o backup de la primera

#### Scenario: Documentos distintos en paralelo
- **WHEN** dos operaciones autorizadas modifican documentos diferentes, incluso para el mismo usuario
- **THEN** usan rutas distintas y ninguna elimina, lee o sobrescribe artefactos de la otra

### Requirement: Transparencia eficiente y determinista
El sistema SHALL aplicar transparencia de forma síncrona mediante acceso bloqueado al bitmap, sin acceso individual por píxel, haciendo transparente solo un píxel cuya diferencia en cada canal RGB sea estrictamente menor que la tolerancia.

#### Scenario: Píxel dentro de tolerancia
- **WHEN** las diferencias R, G y B cumplen el umbral
- **THEN** el alfa resultante es cero y el algoritmo no altera píxeles ajenos

#### Scenario: Transparencia desactivada
- **WHEN** la opción de transparencia está desactivada
- **THEN** la imagen se estampa sin ejecutar la transformación de transparencia

### Requirement: Compatibilidad visual y cierre de recursos
El sistema SHALL conservar el tamaño efectivo legacy 90 x 90 y el cálculo de ubicación vigente para los fixtures aprobados. Todo bitmap, lector PDF, escritor PDF, stream e imagen temporal SHALL quedar liberado en éxito o fallo.

#### Scenario: Página normal o rotada
- **WHEN** se procesa un fixture aprobado con rotación 0, 90, 180 o 270
- **THEN** la comparación visual ubica el grafo en la posición esperada y con tamaño efectivo 90 x 90

#### Scenario: Excepción durante el estampado
- **WHEN** falla cualquier etapa después de abrir recursos
- **THEN** los archivos involucrados pueden reabrirse, renombrarse o eliminarse inmediatamente

### Requirement: Reemplazo validado y recuperable
El sistema SHALL escribir en un archivo staged adyacente al original, cerrarlo, validarlo y reemplazar el original con backup. SHALL restaurar el backup si la validación posterior falla y SHALL NOT degradar a copia destructiva cuando el reemplazo seguro no esté soportado.

#### Scenario: Fallo anterior al reemplazo
- **WHEN** falla carga, transparencia, escritura o validación del staged
- **THEN** el hash y la estructura del original permanecen iguales y se eliminan artefactos propios

#### Scenario: Resultado posterior inválido
- **WHEN** el archivo reemplazado no puede abrirse o no conserva el número de páginas
- **THEN** el backup se restaura y la operación no anuncia commit

#### Scenario: Commit válido
- **WHEN** staged y original reemplazado pasan todas las verificaciones
- **THEN** se elimina el backup residual y se responde éxito una sola vez

### Requirement: Refresco sin extracción de una página
El sistema SHALL retirar la extracción de una página del flujo de guardado y SHALL refrescar desde la URL vigente con invalidación de caché. El cliente SHALL solicitar la página afectada y mantener el transporte `stream` para ese refresco.

#### Scenario: Grafo guardado en página intermedia
- **WHEN** se confirma un grafo en la página 7
- **THEN** el visor carga la página 7 del documento actualizado y conserva página activa, zoom y scroll

#### Scenario: Refresco fallido
- **WHEN** el documento actualizado no puede descargarse o renderizarse
- **THEN** el visor conserva el overlay y clasifica el resultado como incierto sin repetir el POST

### Requirement: Clasificación de transporte posterior
El sistema SHALL clasificar el PDF realmente persistido después del reemplazo y SHALL mantener la mutación desacoplada de la lectura.

#### Scenario: iTextSharp elimina linearización
- **WHEN** el original era linearizado y el resultado ya no contiene `/Linearized` en la cabecera inspeccionada
- **THEN** la clasificación resultante es `stream` y no se afirma descarga parcial

#### Scenario: Resultado conserva linearización
- **WHEN** el PDF final tiene al menos 1 MiB y conserva `/Linearized` válido
- **THEN** una apertura posterior puede seleccionar `range` sin que el guardado ejecute una herramienta externa

### Requirement: Errores y auditoría sin datos sensibles
El sistema SHALL retornar mensajes públicos cerrados y registrar únicamente identificadores funcionales permitidos, página, operación, resultado y modo. SHALL NOT registrar rutas, URL completa, base64, cookies, tokens, cadenas de conexión o texto de excepción.

#### Scenario: Error previo al commit
- **WHEN** ocurre una excepción antes de reemplazar
- **THEN** la respuesta contiene un mensaje saneado y no revela el detalle interno

#### Scenario: Auditoría falla después del commit
- **WHEN** el PDF ya fue confirmado pero falla la auditoría posterior
- **THEN** la respuesta informa una advertencia y no repite ni revierte la mutación

### Requirement: Evidencia descartable y autorizada
El sistema SHALL caracterizar el comportamiento anterior y verificar el nuevo mediante fixtures descartables, pruebas focales, compilación y E2E con el arnés autorizado del repositorio.

#### Scenario: Ejecución sin autorización E2E
- **WHEN** falta autorización de ambiente, cuenta o PDF descartable
- **THEN** el runner se detiene antes de autenticar o mutar

#### Scenario: Validación integral autorizada
- **WHEN** las autorizaciones están vigentes
- **THEN** una acción produce un POST y una persistencia, los controles antes y después son solo `SELECT` y la evidencia saneada cubre concurrencia, rollback, equivalencia visual, `/Linearized` y tiempos por fase
