<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
## Context

Fase03 del mismo módulo: DOC121 formalizó contratos/SQL y DOC122 entregó autorización, conexión y plantillas. Base integrada main36b10cc0; rama feature/DOC-123. Framework4.6.1 y MySQL5.1.37. Este cambio es una biblioteca interna de lectura/validación. No produce PREPARADA ni modifica la base.

La mención de cuenta dedicada en el prompt original está superada por la decisión del usuario aplicada en DOC122: resolver DA de la sesión del módulo con la cuenta compartida existente. No se amplían límites, permisos o conexiones por disponer del archivo de prueba.

## Goals / Non-Goals

Entregar lector común XLS/XLSX que preserve valores, posiciones y números físicos, aplique mapeos guardados y devuelva filas tipadas/incidencias. Preparación posterior solo consume resultados completos y válidos.

Recepción ASHX, UI, persistencia de cargas, comparación de claves según MySQL, INSERT/DELETE/DDL, pausa/reanudación y confirmación quedan para fases04-06. Ninguna adecuación de EGRESOS/CONTRATOS ni reescritura de SII/FastExcel. El lector no acredita que una tabla esté habilitada como fuente de índices.

## Inventario previo de rutas

| Ruta exacta | Estado inspeccionado | Responsabilidad prevista |
|---|---|---|
| Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb | Existente: PlantillaExcel/CampoPlantillaExcel/ColumnaDestinoExcel | Añadir contratos internos de lectura, coordenadas, valor normalizado e incidencias. Preservar campos/huella previos. |
| Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb | Nuevo | Mapeos, transformaciones, validación contra esquema y resultado global. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb | Nuevo | Adaptar stream, formato/hojas, celdas tipadas/formatos y coordenadas al servicio. |
| Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb | Existente | Reutilizar tipos/validaciones/huella; sin guardado ni cambio de contratos. |
| Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb | Existente: LeerDestino | Proporciona metadatos en fase02. No añadir consultas desde lector. |
| Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb | Existente: ValorCompatible | Reutilizar método estático, completando reglas en servicio nuevo sin alterar carpetas. |
| Modelo/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionModels.vb | Existente: OpcionPerfilImportacion | Descriptor para longitud, tipo físico, rango, precisión, escala y charset. |
| Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb | Existente: Limite/Resolver | Autoridad de parámetros/DA en consumidor posterior; no otra conexión. |
| DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb | Existente: DTO ASMX fase02 | Sin nuevo transporte público en fase03. Los resultados son internos. |
| webservice/WebServiceImportacionExcel.asmx.vb | Existente:4 operaciones fase02 | No añadir endpoint para forzar E2E. |
| packages.config; GestionDocumental-Docuarchi.net.vbproj | Existentes | Fijar paquetes, HintPath, CopyLocal y Compile de archivos nuevos; conservar target y referencias actuales. |
| tests/ImportacionExcelBehaviorTests.cs | Existente | Extender suite con lectura real de bytes sintéticos y valores esperados. |
| tests/doc122-importacion-excel-behavior.test.cjs | Existente: compile | Ampliar registro de fuentes/referencias y casos, reutilizando compileBehaviorHarness DOC110. |
| tests/Fixtures/ProduccionDocumental/ImportacionExcel/ | Nuevo | Libros sintéticos XLS/XLSX, expectativas y procedencia reproducible. |
| tools/e2e/tests/importacion-excel.spec.cjs | Existente | Se conserva para recorrido posterior; no autenticación nueva aquí. |
| Doc/Actualizacion/Importacion/Diseno/03-lectura-validacion-doc123.md | Nuevo durante implementación | Contrato vigente, estrategia de fidelidad y límites demostrados. |
| Doc/Actualizacion/Importacion/Validacion/ | Existente | Evidencia focal con comandos, SHA, ambiente y resultados reales. |
| Doc/Actualizacion/Importacion/Sql/ | Existente | No hay migración prevista; cualquier hallazgo exige archivo manual nuevo. |

La división servicio/adaptador separa reglas de plantilla del formato del libro. ServicioPreparacionExcel/ServicioConfirmacionExcel, MySqlCargaExcelRepository, Upload.ashx, JS y CSS del prompt general no se crean en fase03.

## Decisions

### D-01 / RQ-01 — Contrato de lector sin efectos externos

Un servicio de lectura recibe stream privado, snapshot de plantilla/metadatos y límites; devuelve un resultado interno para fase04. No crea endpoint, UI, staging persistente, carga ni SQL.

Evidencia inspeccionada: Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb; Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb.

### D-02 / RQ-02 — Dependencias y fidelidad verificadas antes de integrar

Fijar ExcelDataReader3.6.0 net45 y ExcelNumberFormat1.1.0 net20. Caracterizar errores/fórmulas con libros binarios antes de integrar; ningún error Excel puede convertirse silenciosamente en NULL válido.

Evidencia inspeccionada: GestionDocumental-Docuarchi.net.vbproj:16; packages.config; FastExcel/Class_FastExcel.vb:53; fuentes oficiales versionadas.

### D-03 / RQ-03 — Formato real y selección de hoja

Reconocer XLS/XLSX por contenido, verificar coincidencia con formato esperado y extensión descriptiva, enumerar hojas y seleccionar por nombre exacto. Rechazar formatos ajenos, libro inválido/cifrado y hoja ausente.

Evidencia inspeccionada: Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb:37; ServicioPlantillasExcel.FormatoHistorico.

### D-04 / RQ-04 — Coordenadas y encabezados estables

Fila física1 como encabezados; CAMPO_FUENTE por nombre exacto y único entre los encabezados utilizados; columnas vacías conservan posición. Incluir primera/última fila y omitir solo filas completamente vacías, conservando numeración física.

Evidencia inspeccionada: FastExcel/Class_FastExcel.vb:97; ClassCargarDatos.vb antiguo:255 (antecedente externo).

### D-05 / RQ-05 — Límites efectivos sin ampliaciones

Aplicar límites positivos del servidor:10000 filas de datos no vacías y10000000 bytes por defecto. La fila inválida cuenta; encabezado/vacías no. Detener en primera fila excedente y nunca ofrecer un prefijo como libro válido.

Evidencia inspeccionada: Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb:23; webservice/WebServiceImportacionExcel.asmx.vb:19.

### D-06 / RQ-06 — Mapeo y transformaciones explícitas

Aplicar CAMPO_FUENTE→CAMPO_DESTINO; eliminar coma, luego punto y luego segmento literal base0 solo cuando banderas=1. NULL/0 inactivas; segmento fuera de rango genera incidencia. Preservar apóstrofes, letras, barras, espacios y símbolos no afectados.

Evidencia inspeccionada: Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb:21; ServicioPlantillasExcel.Guardar; ClassCargarDatos.vb antiguo:267.

### D-07 / RQ-07 — Texto y capacidad física

Preservar texto original y formatos significativos de celdas numéricas destinadas a texto, incluido00123. Adaptar metadatos a OpcionPerfilImportacion y reutilizar ValorCompatible para longitud/charset, completando validación Unicode para utf8/utf8mb4.

Evidencia inspeccionada: Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb:253; Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb:74.

### D-08 / RQ-08 — Números sin redondeo implícito

Convertir ENTERO y DECIMAL de forma invariante, comprobar unsigned/rango y precisión/escala físicos. Representar números canónicos sin forzar Decimal CLR para DECIMAL de hasta65 dígitos; rechazar conversión ambigua, fracción entera, NaN/infinito y exceso de precisión.

Evidencia inspeccionada: PerfilImportacionCatalogAdapter.ValorCompatible; ServicioPlantillasExcel.TipoCreable; GestionDocumental-Docuarchi.net.vbproj:337.

### D-09 / RQ-09 — Fechas, nulos y clave según destino

DATE admite fecha tipada o texto exacto yyyy-MM-dd/dd/MM/yyyy; conservar solo fecha. FECHA VARCHAR sigue texto. Celdas nulas requieren columna nullable y clave siempre no nula/no vacía; no suplir errores con defaults ni fechas cero.

Evidencia inspeccionada: ColumnaDestinoExcel.Nulable/TipoCompleto; PlantillaExcel.CAMPO_UNICO; PerfilImportacionCatalogAdapter.ValorCompatible.

### D-10 / RQ-10 — Resultado completo e incidencias localizadas

Resultado con formato/hoja, encabezados, filas normalizadas y coordenadas, conteos y lectura completa/válida. Incidencias con código, fila, columna y campos; muestra máxima1000 con total y truncamiento explícitos; cualquier incidencia impide uso para escritura.

Evidencia inspeccionada: ImportacionExcelException; CampoPlantillaExcel; patrón de DTO tipado de DOC122.

### D-11 / RQ-11 — Compatibilidad y autoridad heredadas

Mantener conexión DA de sesión/resolver, permiso RA y huella DOC122. El lector puro no abre DB ni usa ruta histórica como autoridad; no toca EGRESOS, consumidores SII o FastExcel. Unicidad MySQL, preparación persistente y recuperación pertenecen a fases04/05.

Evidencia inspeccionada: Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb; webservice/WebServiceImportacionExcel.asmx.vb; AGENTS.md.

### D-12 / RQ-12 — Verificación reutilizada y evidencia honesta

Extender harness nativo DOC122/DOC110 con fixtures sintéticos XLS/XLSX y build aislado net461. E2E real no aplica al lector sin recorrido; conservar tools/e2e para fases posteriores. SQL eventual solo en archivos para ejecución manual.

Evidencia inspeccionada: tests/doc122-importacion-excel-behavior.test.cjs; tests/ImportacionExcelBehaviorTests.cs; tools/e2e/tests/importacion-excel.spec.cjs; AGENTS.md.


## Contrato interno — D-01 / D-03 / D-10

El adaptador ofrece enumeración de hojas y lectura de la seleccionada sobre Stream legible/seekable controlado por servidor; nunca recibe una ruta del navegador. Extensión es descriptiva y se contrasta con contenido/PlantillaExcel.Formato. El consumidor entrega copia de plantilla, snapshot de esquema y límites efectivos positivos. El lector no consulta HttpContext, configuración global ni SQL.

ResultadoLecturaExcel contiene formato real, hoja, encabezados con ordinal físico, filas normalizadas y sus números físicos, filas de datos observadas, total de incidencias, muestra e IncidenciasTruncadas, LecturaCompleta y Valido. Los nombres exactos de símbolos se pueden ajustar conservando esta semántica. Fila normalizada contiene valores por CAMPO_DESTINO con campo fuente, ordinal base1 y tipo semántico TEXTO/ENTERO/DECIMAL/DATE. NULL es explícito; no se confunde con cadena vacía o valor inválido.

Valores TEXTO son String, DATE usa DateTime sin hora y números usan representación decimal canónica junto al tipo físico; para enteros se puede usar BigInteger internamente, ya referenciado. DECIMAL(65,30) no cabe en System.Decimal: no estrecharlo a ese CLR ni a Double como representación de salida. El escritor posterior parametriza el valor canónico conforme al destino; fase03 no genera SQL.

Incidencia: código estable, mensaje saneado, hoja, fila física nullable, ordinal físico base1 nullable, CAMPO_FUENTE y CAMPO_DESTINO cuando aplican. No se serializa excepción, ruta, cadena de conexión ni datos arbitrarios a logs. La muestra de incidencias se limita a1000; se conserva conteo total observado y señal de truncamiento. Se recorre el resto del libro dentro de límites para contar errores; fallo de parser/exceso detiene y marca LecturaCompleta=false, sin inventar total de filas no leídas. Si lectura falla, descartar filas parciales; si finaliza con incidencias, Valido=false y las filas no son autorizadas para escritura.

Usar Dispose para readers dejando abierto el stream del consumidor, restaurando su posición inicial incluso en fallo. Cada llamada tiene estado local, sin caché mutable compartida; mismo libro/configuración debe producir misma salida independientemente del historial o cultura del proceso.

## Reglas de lectura — D-03 / D-04 / D-05

Detectar contenido XLS (BIFF) o XLSX (OpenXML) usando el lector y estructura real; un ZIP cualquiera no acredita XLSX. Rechazar CSV/XLSB/XLSM, archivos cifrados/corruptos y discrepancia de formato. No invocar Office, ACE/OLEDB ni recalcular fórmulas. Enumerar hojas conservando nombre/orden; lectura de nombre exacto sin selección alternativa silenciosa. Hoja totalmente vacía o solo encabezado da LIBRO_SIN_DATOS.

Fila1 es encabezado. CAMPO_FUENTE se resuelve con StringComparer.Ordinal; nombre ausente, vacío utilizado o ambiguo es incidencia. No renombrar automáticamente duplicados ni compactar columnas. Columnas no mapeadas conservan su posición y no generan destinos inventados. Mapeos con destino repetido/inválido, clave ausente, tipos incompatibles o configuración de transformación inválida fallan antes de normalizar filas.

Iterar hasta fin del reader, sin Read adicional inicial. Coordenadas basadas en fila física, no contador de filas emitidas. Solo fila con todas las celdas realmente vacías en hoja se omite; espacios, cero, false o valor solo en columna no mapeada no son fila vacía. Errores Excel tampoco son vacíos: véase compuerta D-02.

Comprobar Long del stream antes de abrirlo, contra MaxArchivoMB*1000000 con aritmética comprobada. Valores iniciales10000 y10 vienen del adaptador DOC122; ausencia usa default allí, configuración inválida falla allí y consumidor no puede enviar infinito/cero. Contar cada fila de datos no vacía, válida o inválida; encabezado/vacías no cuentan. Exactamente máximo permitido, máximo+1 detiene con número físico excedente. El archivo local «RELACIÓN ORDENES2025.xlsx» ya observado tiene más de10000 filas de datos: no autoriza ampliar límite, ni forma parte de los fixtures o evidencias públicas.

API de bajo nivel Read/NextResult sin AsDataSet; no materializar todo el workbook ni todas las hojas en código propio. Límite de filas y muestra acota colecciones propias, pero no demuestra por sí mismo memoria máxima del parser, cachés/shared strings o expansión ZIP. No prometer capacidad por encima de10000 ni rendimiento real antes de medición; fase07 mide escenarios del lector nuevo. Reutilizar medidas del importador de carpetas solo como antecedente.

## Transformación y tipado — D-06 / D-07 / D-08 / D-09

1. Obtener valor tipado/formato sin limpieza general. Texto conserva contenido y espacios. Para TEXTO numérico usar formato visual significativo, p.ej.123 con00000 produce00123; sin formato significativo usar representación invariante que no invente dígitos. Formato inválido debe diagnosticarse cuando impida preservar significado, sin aceptar el fallback silencioso como evidencia de fidelidad.
2. Ejecutar REMPLAZA_COMA=1 (quitar coma), REMPLAZA_PUNTO=1 (quitar punto), OPCION_SPLITER=1 (separador literal completo, comparación ordinal y selección base0, preservando segmentos vacíos). NULL/0 inactivas; bandera distinta de0/1 inválida. Si no hay separador, la lista tiene un solo segmento: índice0 devuelve original; otro índice es incidencia. No reproducir el límite invertido antiguo. No quitar apóstrofes/n ni convertir barras por conveniencia SQL.
3. Tipar/validar valor final usando metadatos físicos actuales. Destino existente prevalece, incluso VARCHAR histórico sin longitud; no reducir BENEFICIARIO ni usar nombre FECHA para adivinar DATE. CREAR admite descriptor proyectado explícito de tipos permitidos DOC121, sin inferir capacidad ilimitada ni crear tabla.
4. Reutilizar ValorCompatible con OpcionPerfilImportacion preparado: Tipo, Longitud, Requerida, TipoFisico, Precision, Escala, Charset. Complementar en servicio: fecha/nulabilidad y conteo Unicode/capacidad textual cuando su longitud UTF16 sea conservadora. ascii exigeASCII; latin1 usa conversión cp1252 sin sustitución; utf8 MySQL de3bytes rechaza suplementarios/surrogates inválidos y utf8mb4 admite pares válidos, siempre según charset físico. No normalizar letras, espacios, acentos o mayúsculas.
5. ENTERO admite signo y dígitos canónicos, sin fracción/grupos textuales; validar desde TINYINT hasta BIGINT signed/unsigned con rango exacto. DECIMAL textual admite signo/dígitos/punto decimal sin separadores de grupo ni ambigüedad cultural; transformaciones explícitas pueden quitar coma/punto primero. Del valor numérico Excel usar representación invariante y expandir exponente a canónico antes de validar; sin NaN/infinito ni redondeo implícito. Comprobar p/s físicos, hasta65/30. Un double ya perdió la precisión que Excel no almacenó: no prometer recuperar cifras; códigos largos deben venir como texto.
6. DATE acepta DateTime convertido por biblioteca o texto exacto yyyy-MM-dd/dd/MM/yyyy, calendario válido y rango1000-01-01..9999-12-31; salida fecha sin hora. No interpretar número arbitrario como fecha sin tipado/formato del libro, ni texto según cultura de máquina, ni fecha cero.
7. NULL solo permitido en nullable no clave. String vacío en TEXTO nullable se conserva; en requerido se diagnostica; no reemplazar celdas vacías por default. Clave CAMPO_UNICO siempre mapeada/no nula/no vacía/solo espacios; validación de duplicados/igualdad MySQL queda en fase04 y no se sustituye con HashSet ordinal.

## Fidelidad de dependencia y compuerta de integración — D-02

Revisadas el2026-11-22:
- [Proyecto ExcelDataReader3.6.0](https://raw.githubusercontent.com/ExcelDataReader/ExcelDataReader/v3.6.0/src/ExcelDataReader/ExcelDataReader.csproj): targets net45, con referencias de plataforma System.Data/System.IO.Compression.
- [Paquete3.6.0](https://www.nuget.org/packages/ExcelDataReader/3.6.0) y [API versionada](https://raw.githubusercontent.com/ExcelDataReader/ExcelDataReader/v3.6.0/README.md): paquete base para Read/NextResult, valores y formatos; no se requiere DataSet ni CodePages adicional del mundo .NET Core.
- [ExcelNumberFormat1.1.0](https://www.nuget.org/packages/ExcelNumberFormat/1.1.0): ensamblado net20 apto para net461. La compilación/ejecución en proyecto real aún debe acreditarse.
- [Conversión XLSX3.6.0](https://raw.githubusercontent.com/ExcelDataReader/ExcelDataReader/v3.6.0/src/ExcelDataReader/Core/OpenXmlFormat/XlsxWorksheet.cs): ConvertCellValue caso e retorna null.
- [Conversión XLS3.6.0](https://raw.githubusercontent.com/ExcelDataReader/ExcelDataReader/v3.6.0/src/ExcelDataReader/Core/BinaryFormat/XlsWorksheet.cs): BOOLERR y FormulaValueType.Error pueden quedar null; valores de fórmula son resultados guardados.

El paquete por sí solo no acredita diagnóstico fiel de errores. La primera tarea ejecutable crea/caracteriza fixtures XLS/XLSX con error, NULL real, fórmula con valor/error/sin valor y formato00000, dejando evidencia de lo observado. Antes de integrar debe existir estrategia de detección complementaria en adaptador, revisable y probada en los dos formatos, que distinga errores de NULL sin reflejar APIs privadas ni cambiar silenciosamente versiones/arquitectura. Mientras no exista, la aceptación RQ-02 y la entrega de lector completo quedan bloqueadas; no marcar la prueba como pasada usando solo columnas NOT NULL. Si exige otra dependencia/fork o parser de formato adicional, revisar explícitamente diseño y contrato antes de incorporarlo. Es una dependencia técnica del plan, no una aprobación pendiente del usuario para investigar.

Las fórmulas no se recalculan; un resultado guardado válido puede usarse si la caracterización demuestra su observabilidad. Se explicita que podría estar desactualizado. Errores y ausencia de resultado utilizable nunca se publicitan como valores válidos conservados.

## Autoridad, SQL y continuidad — D-11 / D-12

El consumidor fase04 resolverá usuario/empresa/permiso, DA, plantilla/revisión/huella y esquema del servidor antes de llamar al lector, y repetirá comprobaciones al preparar/confirmar. La función pura no acredita autorización vigente por aceptar un objeto Contexto. No cambiar resolución DA ni cabeceras/DTO/huella v1 fase02.

Sin DDL/DML, consultas reales, despliegue o SQL nuevo necesario para esta fase. Si surge una adecuación, entregar archivo nuevo bajo Sql/ y guía manual; esperar confirmación y verificar solo SELECT. No ejecutar instalación/limpieza desde tests. EGRESOS es solo pruebas y permanece intacto.

Lectura completa requerida antes de consumir filas. Repetir la lectura del mismo archivo inmutable es determinista, pero no crea checkpoints ni garantiza pausa/reanudación de migraciones. Recepción/ejecución persistida, ID/hash, recuperación de respuesta y confirmación transaccional son responsabilidad fase04/05. No trasladar autorizaciones de ventanas E2E ya cerradas.

## Verificación y entrega — D-12

Extender compile DOC122/DOC110 y suite C# existentes; no infraestructura paralela. Fixture XLS debe ser BIFF real, no CSV/HTML renombrado, y XLSX OpenXML real. Incluir generación/procedencia sin información de negocio ni biblioteca nueva de producción solo para escribir fixtures. Expectativas independientes para posiciones,00000, apóstrofe/n/acentos, varias hojas, DECIMAL65, fecha/nullable, transformaciones, errores y fronteras.

Ejecutar pruebas nativas y regresión focal DOC122/DOC110, node --test tests/doc122-e2e-reuse.test.cjs (planificación sintética, sin login) y build MSBuild aislado con DeployOnBuild=false, Disable_CopyWebApplication=true, UseSharedCompilation=false y rutas .tmp propias. No sustituir DLL desplegada para probar un lector interno.

Registrar por separado diseño revisado, pruebas ejecutadas y pruebas aún abiertas. E2E funcional no aplica porque fase03 no añade recorrido/endpoint; guardas reales/autenticación y datos descartables se reutilizarán en fases siguientes con alcance explícito. Documentación técnica DOC123 y evidencia de build/pruebas al implementar. El refinamiento por sí mismo no acredita esos resultados.

## Riesgos y rollback

Riesgo principal: errores/caché ocultos de3.6.0; compuerta de caracterización anterior a integración. Otros: límites CLR frente DECIMAL65, cultura, formatos visuales y uso de longitud UTF16 conservadora; pruebas de fronteras y validación adicional. Compactación de celdas/filas y cambio accidental de consumidores compartidos se cubren con fixtures/regresión. Las medidas actuales de carpetas no prueban el lector nuevo.

Rollback de producto futuro: revertir solo referencias/Compile y archivos nuevos del lector, preservando DOC122 y datos. No hay migración de esquema ni limpieza de negocio en este cambio. Esta entrega solo modifica documentación y planificación.
