<!-- opsxj:refinement-traceability version=1 artifact=spec decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
## ADDED Requirements

### Requirement: RQ-01 Contrato de lector sin efectos externos

El sistema SHALL cumplir D-01: Un servicio de lectura recibe stream privado, snapshot de plantilla/metadatos y límites; devuelve un resultado interno para fase04. No crea endpoint, UI, staging persistente, carga ni SQL.

#### Scenario: Lectura pura

- **WHEN** un consumidor entrega stream legible/seekable, plantilla copiada, esquema y límites válidos
- **THEN** el servicio devuelve el contrato interno sin abrir conexiones, crear archivos ni cambiar plantilla o estado persistido

#### Scenario: Propiedad del recurso

- **WHEN** termina una lectura correcta o fallida sobre un stream del consumidor
- **THEN** se liberan readers auxiliares, el stream permanece abierto y se restaura su posición inicial

### Requirement: RQ-02 Dependencias y fidelidad verificadas antes de integrar

El sistema SHALL cumplir D-02: Fijar ExcelDataReader3.6.0 net45 y ExcelNumberFormat1.1.0 net20. Caracterizar errores/fórmulas con libros binarios antes de integrar; ningún error Excel puede convertirse silenciosamente en NULL válido.

#### Scenario: Compilación de dependencias

- **WHEN** se restauran las versiones fijadas y se compila sobre net461
- **THEN** se usa ExcelDataReader/lib/net45 y ExcelNumberFormat/lib/net20 sin cambiar el framework ni sustituir FastExcel

#### Scenario: Error de celda y caché de fórmula

- **WHEN** un fixture XLS o XLSX contiene error Excel, fórmula con error o fórmula sin resultado utilizable
- **THEN** la caracterización prueba su detección y el resultado no los acepta como NULL/vacío; si la biblioteca los oculta se bloquea la integración hasta corregir el adaptador y registrar su estrategia

#### Scenario: Resultado guardado de fórmula

- **WHEN** una fórmula tiene valor guardado válido y detectable
- **THEN** se valida ese valor sin ejecutar fórmulas, macros o enlaces y sin afirmar que se recalculó

### Requirement: RQ-03 Formato real y selección de hoja

El sistema SHALL cumplir D-03: Reconocer XLS/XLSX por contenido, verificar coincidencia con formato esperado y extensión descriptiva, enumerar hojas y seleccionar por nombre exacto. Rechazar formatos ajenos, libro inválido/cifrado y hoja ausente.

#### Scenario: Hojas por contenido

- **WHEN** se entrega un libro válido de varias hojas
- **THEN** se devuelve el formato real y sus hojas; la lectura usa solo la hoja pedida por nombre exacto

#### Scenario: Formato o libro no permitido

- **WHEN** el contenido no coincide con XLS/XLSX esperado, está corrupto, cifrado o es CSV/XLSB/XLSM
- **THEN** se devuelve diagnóstico controlado sin intentar otro lector silenciosamente

#### Scenario: Hoja ausente

- **WHEN** el nombre solicitado no existe o no hay hoja utilizable
- **THEN** se informa HOJA_NO_ENCONTRADA o LIBRO_SIN_DATOS sin seleccionar otra hoja

### Requirement: RQ-04 Coordenadas y encabezados estables

El sistema SHALL cumplir D-04: Fila física1 como encabezados; CAMPO_FUENTE por nombre exacto y único entre los encabezados utilizados; columnas vacías conservan posición. Incluir primera/última fila y omitir solo filas completamente vacías, conservando numeración física.

#### Scenario: Columnas vacías y extremos

- **WHEN** el libro tiene A/C mapeadas, B vacía, primera fila de datos2 y última filaN
- **THEN** se preservan índices de columna y se incluyen ambas filas con números2/N

#### Scenario: Filas vacías y contenido no mapeado

- **WHEN** hay filas totalmente vacías, otra con espacios o con valor solo en una columna no mapeada
- **THEN** solo las totalmente vacías se omiten; las restantes cuentan y se validan sin compactar coordenadas

#### Scenario: Encabezado no resoluble

- **WHEN** un campo fuente falta, está vacío o corresponde a más de una columna
- **THEN** se devuelve incidencia de configuración/encabezado; no se inventan nombres ni se desplazan mapeos

### Requirement: RQ-05 Límites efectivos sin ampliaciones

El sistema SHALL cumplir D-05: Aplicar límites positivos del servidor:10000 filas de datos no vacías y10000000 bytes por defecto. La fila inválida cuenta; encabezado/vacías no. Detener en primera fila excedente y nunca ofrecer un prefijo como libro válido.

#### Scenario: Frontera de filas

- **WHEN** la hoja contiene exactamente MaxFilasDatos filas no vacías y después otra adicional
- **THEN** el primer caso puede ser válido y el segundo informa LIMITE_FILAS_EXCEDIDO en la fila física excedente; ninguna salida parcial es confirmable

#### Scenario: Frontera de bytes

- **WHEN** el stream tiene exactamente MaxArchivoMB*1000000 bytes o un byte más
- **THEN** el límite inclusivo se respeta y el exceso se rechaza antes de parsear

#### Scenario: Límites inválidos

- **WHEN** el servidor aporta cero, negativo o multiplicación fuera de rango
- **THEN** se rechaza la configuración sin convertirla en ilimitada ni elevarla

### Requirement: RQ-06 Mapeo y transformaciones explícitas

El sistema SHALL cumplir D-06: Aplicar CAMPO_FUENTE→CAMPO_DESTINO; eliminar coma, luego punto y luego segmento literal base0 solo cuando banderas=1. NULL/0 inactivas; segmento fuera de rango genera incidencia. Preservar apóstrofes, letras, barras, espacios y símbolos no afectados.

#### Scenario: Reglas activas y orden

- **WHEN** la plantilla activa quitar coma, quitar punto y seleccionar segmento literal índice0
- **THEN** se aplican en ese orden y se valida el resultado; NULL/0 no transforman

#### Scenario: Segmento inválido

- **WHEN** el separador configurado no produce el segmento solicitado o su configuración es inválida
- **THEN** se emite TRANSFORMACION_INVALIDA sin excepción de índice ni reutilizar silenciosamente el valor original

#### Scenario: Contenido preservado

- **WHEN** el texto contiene apóstrofes, letra n, barras, símbolos y espacios sin transformación explícita
- **THEN** el valor normalizado conserva exactamente esos caracteres

### Requirement: RQ-07 Texto y capacidad física

El sistema SHALL cumplir D-07: Preservar texto original y formatos significativos de celdas numéricas destinadas a texto, incluido00123. Adaptar metadatos a OpcionPerfilImportacion y reutilizar ValorCompatible para longitud/charset, completando validación Unicode para utf8/utf8mb4.

#### Scenario: Código con ceros

- **WHEN** una celda textual00123 o numérica123 con formato00000 se asigna a VARCHAR
- **THEN** se obtiene00123 sin conversión a entero ni pérdida de ceros

#### Scenario: Longitud y charset

- **WHEN** los datos exceden la longitud o no son representables en ascii/latin1/utf8 del destino
- **THEN** se informa la columna y fila con LONGITUD_EXCEDIDA o CHARSET_INCOMPATIBLE sin truncar ni sustituir caracteres

#### Scenario: Longitud ampliada histórica

- **WHEN** una columna física BENEFICIARIO tiene longitud mayor que la descripción histórica del mapeo
- **THEN** la validación usa la capacidad física vigente y no reduce ni altera la tabla

### Requirement: RQ-08 Números sin redondeo implícito

El sistema SHALL cumplir D-08: Convertir ENTERO y DECIMAL de forma invariante, comprobar unsigned/rango y precisión/escala físicos. Representar números canónicos sin forzar Decimal CLR para DECIMAL de hasta65 dígitos; rechazar conversión ambigua, fracción entera, NaN/infinito y exceso de precisión.

#### Scenario: Enteros de frontera

- **WHEN** se validan valores signed/unsigned en límites de TINYINT a BIGINT
- **THEN** se aceptan fronteras válidas y se rechazan fracciones y valores fuera de rango sin overflow CLR

#### Scenario: Precisión decimal

- **WHEN** el destino es DECIMAL(p,s), incluido p65, y los valores están en frontera o la exceden
- **THEN** se conserva representación canónica y se rechaza exceso de enteros/escala sin redondeo o notación no validada

#### Scenario: Cultura y origen numérico

- **WHEN** la cultura del proceso cambia o el texto contiene separadores ambiguos
- **THEN** el resultado sigue las reglas invariantes y transformaciones guardadas; no se interpreta por cultura de máquina ni se reconstruyen dígitos que Excel ya perdió

### Requirement: RQ-09 Fechas, nulos y clave según destino

El sistema SHALL cumplir D-09: DATE admite fecha tipada o texto exacto yyyy-MM-dd/dd/MM/yyyy; conservar solo fecha. FECHA VARCHAR sigue texto. Celdas nulas requieren columna nullable y clave siempre no nula/no vacía; no suplir errores con defaults ni fechas cero.

#### Scenario: DATE válido e inválido

- **WHEN** el destino es DATE y llega fecha tipada o texto en los dos formatos permitidos
- **THEN** se acepta fecha de calendario en rango1000-01-01 a9999-12-31; se rechazan fechas cero, imposibles y textos ambiguos

#### Scenario: FECHA histórica

- **WHEN** una columna llamada FECHA es VARCHAR
- **THEN** se aplica la regla de texto sin imponer conversión DATE

#### Scenario: Nulos y clave

- **WHEN** una celda está vacía o la clave mapeada queda vacía tras transformar
- **THEN** solo un destino nullable no clave puede aceptar NULL; la clave nula/vacía/solo espacios genera incidencia, sin suplir default

### Requirement: RQ-10 Resultado completo e incidencias localizadas

El sistema SHALL cumplir D-10: Resultado con formato/hoja, encabezados, filas normalizadas y coordenadas, conteos y lectura completa/válida. Incidencias con código, fila, columna y campos; muestra máxima1000 con total y truncamiento explícitos; cualquier incidencia impide uso para escritura.

#### Scenario: Incidencia con coordenadas

- **WHEN** una conversión falla en columnaC de fila física8
- **THEN** se devuelve código estable, fila8, columna3, encabezado y campo destino, sin ruta física ni excepción interna

#### Scenario: Reporte acotado

- **WHEN** el libro tiene más de1000 incidencias
- **THEN** se conserva total exacto de incidencias detectadas al recorrerlo, primera muestra1000 y señal de truncamiento; Resultado.Valido permanece falso

#### Scenario: Lectura interrumpida

- **WHEN** se supera límite o falla el parser tras algunas filas
- **THEN** LecturaCompleta es falso y no se entrega un conjunto parcial utilizable; se informa lo observado sin inventar total del libro

### Requirement: RQ-11 Compatibilidad y autoridad heredadas

El sistema SHALL cumplir D-11: Mantener conexión DA de sesión/resolver, permiso RA y huella DOC122. El lector puro no abre DB ni usa ruta histórica como autoridad; no toca EGRESOS, consumidores SII o FastExcel. Unicidad MySQL, preparación persistente y recuperación pertenecen a fases04/05.

#### Scenario: Metadatos como snapshot

- **WHEN** se llama al lector con metadatos ya resueltos por el módulo
- **THEN** no se leen credenciales/rutas del cliente, no se resuelve otra DA ni se altera hash v1, permisos o plantilla

#### Scenario: Unicidad y recuperación separadas

- **WHEN** el lector finaliza sin errores de celda
- **THEN** ello no acredita unicidad por collation, ausencia de claves existentes, PREPARADA, confirmación ni recuperación; fases04/05 revalidan archivo/plantilla/esquema y autoridad

### Requirement: RQ-12 Verificación reutilizada y evidencia honesta

El sistema SHALL cumplir D-12: Extender harness nativo DOC122/DOC110 con fixtures sintéticos XLS/XLSX y build aislado net461. E2E real no aplica al lector sin recorrido; conservar tools/e2e para fases posteriores. SQL eventual solo en archivos para ejecución manual.

#### Scenario: Verificación sintética reutilizada

- **WHEN** se ejecuta la suite focal de libros y la regresión DOC122/DOC110
- **THEN** los datos son sintéticos, se prueba contenido esperado de ambos formatos y no hay conexión real, autenticación, instalación SQL o escritura de negocio

#### Scenario: Entrega de lector

- **WHEN** se documentan build y pruebas del lector sin endpoint nuevo
- **THEN** la evidencia identifica SHA/ambiente/comandos/resultados y E2E no aplicable a esta fase; no presenta simulación como E2E real
