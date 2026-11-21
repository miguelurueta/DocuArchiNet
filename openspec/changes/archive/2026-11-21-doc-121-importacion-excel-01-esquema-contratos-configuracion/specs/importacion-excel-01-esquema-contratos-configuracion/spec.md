<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## Purpose

Formalizar la base contractual y el paquete SQL manual de la importación de Excel a tablas que alimentan índices. Esta fase define estructuras, autorización, configuración, validación previa e identidad recuperable de carga, conservando consumidores históricos. La presencia de estos artefactos no acredita endpoints, instalación o ejecución real. Las fases posteriores implementan el recorrido acordado de archivo completo, revisión integral y confirmación transaccional.

## ADDED Requirements

### Requirement: D-01 RQ-01 Catálogos y consumidores compatibles
La entrega SHALL conservar los tres catálogos históricos, identificar sus consumidores y extender la cabecera de plantilla de forma aditiva.

#### Scenario: Extensión de plantilla
- **WHEN** se revisa el SQL de cabecera
- **THEN** solo agrega CAMPO_UNICO nullable y REVISION_CARGA al final sin renombrar columnas históricas ni alterar mapeos

#### Scenario: Plantilla histórica sin clave
- **WHEN** un consumidor SII consulta una plantilla cuya nueva clave es NULL
- **THEN** el contrato preserva su lectura histórica y exige adecuación antes de permitir escritura web

#### Scenario: Longitud real de una fuente
- **WHEN** el preflight encuentra una longitud distinta de la observada el 11 de noviembre
- **THEN** se conserva la estructura actual y se impide sustituirla automáticamente por el antecedente, incluida BENEFICIARIO

### Requirement: D-02 RQ-02 SQL manual verificable
La entrega SHALL separar preflight, cambios RA/DA, adecuaciones por tabla, concesiones y comprobación posterior, sin aplicar los archivos automáticamente.

#### Scenario: Entrega sin instalación
- **WHEN** el agente genera y verifica los scripts localmente
- **THEN** la instalación permanece PENDIENTE_EJECUCION_MANUAL y no se declara aplicada

#### Scenario: Comprobación posterior
- **WHEN** el usuario confirma la ejecución manual
- **THEN** el agente comprueba lo instalado únicamente mediante SELECT y entrega otro archivo si una diferencia necesita modificación

#### Scenario: Aplicación parcial
- **WHEN** un objeto o columna ya existe
- **THEN** se contrasta su definición y se omite el paso compatible o se genera una corrección revisada sin reaplicación ciega

### Requirement: D-03 RQ-03 Registro operativo de carga
El esquema SHALL definir una sola tabla operativa nueva InnoDB en la DA del destino, con ID de carga, propietario, archivo, configuración, huellas y resultado.

#### Scenario: Archivo recibido
- **WHEN** una carga se registra como RECIBIDA
- **THEN** conserva ID, ámbito, usuario, plantilla, referencia privada, bytes y hash, permitiendo NULL en campos de preparación todavía no establecidos

#### Scenario: Preparación íntegra
- **WHEN** una carga pasa a PREPARADA
- **THEN** el contrato exige revisión y snapshot de configuración, huellas de archivo/esquema y conteos validados

#### Scenario: Esquema compatible
- **WHEN** se revisa el CREATE operativo para MySQL 5.1
- **THEN** usa tipos disponibles, índices acotados y ausencia de CHECK, JSON nativo, triggers o FK a bases/estructuras legacy

### Requirement: D-04 RQ-04 Autoridad propia y configuración
El contrato SHALL exigir ADMINISTRAR_CARGAS_IMPORTACION y vincular en servidor ámbito, DA, conexión histórica y cuenta dedicada, con límites positivos independientes de 10000 filas y 10 MB decimales.

#### Scenario: Permiso exclusivo
- **WHEN** el usuario tiene el permiso propio de carga y no administra perfiles
- **THEN** el contrato permite el recorrido de carga autorizado sin exigir permisos ajenos ni conceder administración de fuentes

#### Scenario: Límites efectivos
- **WHEN** se recibe, prepara o confirma una carga
- **THEN** se comprueban respectivamente bytes, filas y vigencia de los límites, contando 1000000 bytes por MB y excluyendo el encabezado

#### Scenario: Conexión manipulada
- **WHEN** una referencia no corresponde a la DA configurada para el ámbito
- **THEN** se deniega la operación y nunca se usa un host, base o credencial enviados por el navegador

### Requirement: D-05 RQ-05 Clave única y fuente compatible
El contrato SHALL exigir clave requerida con índice UNIQUE completo y comparación MySQL real, respetando aislamiento y metadatos actuales del destino.

Este requisito aplica a la tabla efectivamente seleccionada para una carga o creada por el cargador. La entrega contractual de la fase 01 no exige adecuar tablas históricas de ejemplo como EGRESOS, declarada de pruebas por el usuario.

#### Scenario: Fuente exclusiva y compartida
- **WHEN** se adecua una fuente de conexión exclusiva o una compartida por columna empresarial
- **THEN** se revisa respectivamente la clave completa o empresa+clave, sin cambiar el aislamiento existente

#### Scenario: Datos inconsistentes
- **WHEN** hay NULL, vacíos, duplicados o una clave que excede la capacidad del índice
- **THEN** se bloquea su adecuación ordinaria y se entrega diagnóstico sin truncar, borrar o normalizar datos automáticamente

#### Scenario: Fuente nueva
- **WHEN** se confirma la carga de una tabla nueva imp_02_
- **THEN** su uso como índices requiere registro/activación de fuente y publicación de proveedor mediante los servicios vigentes

### Requirement: D-06 RQ-06 Contratos y validación integral
La entrega SHALL definir request/response de plantillas, recepción, preparación, confirmación y resultado, con multipart para el archivo y validación completa antes de escribir datos destino.

#### Scenario: Preparar una carga
- **WHEN** se prepara un archivo XLS o XLSX
- **THEN** se revisa el contenido completo, mapeos, tipos, nulabilidad, índice y duplicados del modo seleccionado, mostrando muestra acotada e incidencias por fila/columna

#### Scenario: Cambio después de preparar
- **WHEN** cambia archivo, plantilla, esquema o autoridad antes de confirmar
- **THEN** el contrato detecta conflicto incluso si el editor histórico no incrementó REVISION_CARGA

#### Scenario: Preparación sin mutación del destino
- **WHEN** la preparación termina o devuelve incidencias
- **THEN** conserva filas del destino y limita sus efectos al archivo, registro y objetos temporales de validación

### Requirement: D-07 RQ-07 Confirmación atómica y recuperable
El contrato SHALL confirmar datos y resultado en una transacción DML, resolver repetición por ID y separar la creación estructural de la carga de filas, sin pausa o commits por fila.

#### Scenario: Commit confirmado con acuse perdido
- **WHEN** se repite el mismo ID y contenido tras perder su respuesta
- **THEN** la autoridad vigente se comprueba y se devuelve el resultado persistido sin otra escritura ni invalidarlo por cambios posteriores de plantilla

#### Scenario: Reemplazo fallido
- **WHEN** falla de forma conocida DELETE o INSERT antes del commit
- **THEN** se revierte la carga completa, se conservan los datos anteriores y se registra el fallo sin sobrescribir una confirmación concurrente

#### Scenario: CREATE con respuesta incierta
- **WHEN** la tabla puede haber sido creada
- **THEN** se verifica el comentario del ID y la huella de estructura antes de continuar, conservando una tabla propia vacía y rechazando un destino ajeno

### Requirement: D-08 RQ-08 Evidencia proporcionada al alcance
La entrega SHALL acreditar revisión estática y trazabilidad sin presentar generación de archivos como instalación, compilación de producto o aceptación E2E.

#### Scenario: Fase sin código compilable
- **WHEN** el diff contiene únicamente Markdown, SQL y evidencia documental
- **THEN** se justifica la ausencia de build y E2E de producto y se ejecutan controles pertinentes de artefactos y gobierno

#### Scenario: Fases dependientes
- **WHEN** una fase posterior necesita el nuevo esquema
- **THEN** requiere confirmación manual y SELECT posterior antes de utilizarlo en pruebas dependientes

#### Scenario: Resultados locales
- **WHEN** se publica la evidencia de esta fase
- **THEN** identifica SHA, comandos, hashes y alcance local, conservando sin acreditar la capacidad real o las pruebas de usuario de fases 02–07
