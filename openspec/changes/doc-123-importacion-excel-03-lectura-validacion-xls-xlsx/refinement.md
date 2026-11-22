<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento DOC-123

## Fuente y alcance

Revisión técnica Codex2026-11-22, solicitada con «y el refinamiento». Plan de fase03 sobre main36b10cc0 (cierre DOC122), sin implementación de producto ni pruebas reales. Perfil legacy-webforms-vb y .NET Framework4.6.1. approved registra revisión técnica del plan y criterios: no acredita fidelidad del paquete, implementación, despliegue o autorización de datos reales.

## Contexto inspeccionado

- DOC121: contratos de clave/tipos, huella v1, límites positivos y SQL manual; Doc/Actualizacion/Importacion/Diseno/01-contratos-configuracion-doc121.md.
- DOC122: PlantillaExcel, CampoPlantillaExcel, ColumnaDestinoExcel y ServicioPlantillasExcel.ValidarDestino/Huella. MySqlPlantillaExcelRepository.LeerDestino aporta capacidad física actual.
- ImportacionExcelConexionAdapter.Limite y ModuleSessionConnectionStringResolver: conservar DA del módulo/cuenta compartida; no exigir conexión nueva en Web.config.
- PerfilImportacionCatalogAdapter.ValorCompatible valida longitud UTF16, rangos enteros/decimales y ascii/latin1. Completar DATE/NULL y Unicode en lector, sin cambiar el adaptador compartido.
- FastExcel/Class_FastExcel.vb:53-59 elimina caracteres. No modificar ese consumidor SII ni reutilizar su saneamiento.
- Antecedente externo leído sin editar: D:/imagenesda/desarrollo/copiaservercamara/Proyectos-General/preproyectoimport/preproyectoimport/ClassCargarDatos.vb:255-282. Omite primera fila, elimina apóstrofes/n, reemplaza barras y compara el índice split al revés. Recuperar opciones, no defectos.
- tests/doc122-importacion-excel-behavior.test.cjs, tests/ImportacionExcelBehaviorTests.cs y tools/e2e/tests/importacion-excel.spec.cjs. Sin fixtures XLS/XLSX focales actuales.
- Fuentes oficiales versionadas y limitación de errores/caché documentadas en design.md. Framework declarado no sustituye compilación ni pruebas binarias.

## Decisiones aprobadas

| ID | Decision verificable | Evidencia de codigo | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Un servicio de lectura recibe stream privado, snapshot de plantilla/metadatos y límites; devuelve un resultado interno para fase04. No crea endpoint, UI, staging persistente, carga ni SQL. | Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb; Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Fijar ExcelDataReader3.6.0 net45 y ExcelNumberFormat1.1.0 net20. Caracterizar errores/fórmulas con libros binarios antes de integrar; ningún error Excel puede convertirse silenciosamente en NULL válido. | GestionDocumental-Docuarchi.net.vbproj:16; packages.config; FastExcel/Class_FastExcel.vb:53; fuentes oficiales versionadas | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Reconocer XLS/XLSX por contenido, verificar coincidencia con formato esperado y extensión descriptiva, enumerar hojas y seleccionar por nombre exacto. Rechazar formatos ajenos, libro inválido/cifrado y hoja ausente. | Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb:37; ServicioPlantillasExcel.FormatoHistorico | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Fila física1 como encabezados; CAMPO_FUENTE por nombre exacto y único entre los encabezados utilizados; columnas vacías conservan posición. Incluir primera/última fila y omitir solo filas completamente vacías, conservando numeración física. | FastExcel/Class_FastExcel.vb:97; ClassCargarDatos.vb antiguo:255 (antecedente externo) | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Aplicar límites positivos del servidor:10000 filas de datos no vacías y10000000 bytes por defecto. La fila inválida cuenta; encabezado/vacías no. Detener en primera fila excedente y nunca ofrecer un prefijo como libro válido. | Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb:23; webservice/WebServiceImportacionExcel.asmx.vb:19 | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Aplicar CAMPO_FUENTE→CAMPO_DESTINO; eliminar coma, luego punto y luego segmento literal base0 solo cuando banderas=1. NULL/0 inactivas; segmento fuera de rango genera incidencia. Preservar apóstrofes, letras, barras, espacios y símbolos no afectados. | Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb:21; ServicioPlantillasExcel.Guardar; ClassCargarDatos.vb antiguo:267 | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Preservar texto original y formatos significativos de celdas numéricas destinadas a texto, incluido00123. Adaptar metadatos a OpcionPerfilImportacion y reutilizar ValorCompatible para longitud/charset, completando validación Unicode para utf8/utf8mb4. | Infrastructure/ProduccionDocumental/ImportacionCarpetas/PerfilImportacionCatalogAdapter.vb:253; Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb:74 | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Convertir ENTERO y DECIMAL de forma invariante, comprobar unsigned/rango y precisión/escala físicos. Representar números canónicos sin forzar Decimal CLR para DECIMAL de hasta65 dígitos; rechazar conversión ambigua, fracción entera, NaN/infinito y exceso de precisión. | PerfilImportacionCatalogAdapter.ValorCompatible; ServicioPlantillasExcel.TipoCreable; GestionDocumental-Docuarchi.net.vbproj:337 | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | DATE admite fecha tipada o texto exacto yyyy-MM-dd/dd/MM/yyyy; conservar solo fecha. FECHA VARCHAR sigue texto. Celdas nulas requieren columna nullable y clave siempre no nula/no vacía; no suplir errores con defaults ni fechas cero. | ColumnaDestinoExcel.Nulable/TipoCompleto; PlantillaExcel.CAMPO_UNICO; PerfilImportacionCatalogAdapter.ValorCompatible | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Resultado con formato/hoja, encabezados, filas normalizadas y coordenadas, conteos y lectura completa/válida. Incidencias con código, fila, columna y campos; muestra máxima1000 con total y truncamiento explícitos; cualquier incidencia impide uso para escritura. | ImportacionExcelException; CampoPlantillaExcel; patrón de DTO tipado de DOC122 | D-10 | RQ-10 | Origen: D-10, RQ-10 |
| D-11 | Mantener conexión DA de sesión/resolver, permiso RA y huella DOC122. El lector puro no abre DB ni usa ruta histórica como autoridad; no toca EGRESOS, consumidores SII o FastExcel. Unicidad MySQL, preparación persistente y recuperación pertenecen a fases04/05. | Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb; webservice/WebServiceImportacionExcel.asmx.vb; AGENTS.md | D-11 | RQ-11 | Origen: D-11, RQ-11 |
| D-12 | Extender harness nativo DOC122/DOC110 con fixtures sintéticos XLS/XLSX y build aislado net461. E2E real no aplica al lector sin recorrido; conservar tools/e2e para fases posteriores. SQL eventual solo en archivos para ejecución manual. | tests/doc122-importacion-excel-behavior.test.cjs; tests/ImportacionExcelBehaviorTests.cs; tools/e2e/tests/importacion-excel.spec.cjs; AGENTS.md | D-12 | RQ-12 | Origen: D-12, RQ-12 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptacion | Riesgo/compatibilidad |
|---|---|---|---|
| RQ-01 | Contrato de lector sin efectos externos | Lectura pura; Propiedad del recurso | D-01; ver spec y tareas focales |
| RQ-02 | Dependencias y fidelidad verificadas antes de integrar | Compilación de dependencias; Error de celda y caché de fórmula; Resultado guardado de fórmula | D-02; ver spec y tareas focales |
| RQ-03 | Formato real y selección de hoja | Hojas por contenido; Formato o libro no permitido; Hoja ausente | D-03; ver spec y tareas focales |
| RQ-04 | Coordenadas y encabezados estables | Columnas vacías y extremos; Filas vacías y contenido no mapeado; Encabezado no resoluble | D-04; ver spec y tareas focales |
| RQ-05 | Límites efectivos sin ampliaciones | Frontera de filas; Frontera de bytes; Límites inválidos | D-05; ver spec y tareas focales |
| RQ-06 | Mapeo y transformaciones explícitas | Reglas activas y orden; Segmento inválido; Contenido preservado | D-06; ver spec y tareas focales |
| RQ-07 | Texto y capacidad física | Código con ceros; Longitud y charset; Longitud ampliada histórica | D-07; ver spec y tareas focales |
| RQ-08 | Números sin redondeo implícito | Enteros de frontera; Precisión decimal; Cultura y origen numérico | D-08; ver spec y tareas focales |
| RQ-09 | Fechas, nulos y clave según destino | DATE válido e inválido; FECHA histórica; Nulos y clave | D-09; ver spec y tareas focales |
| RQ-10 | Resultado completo e incidencias localizadas | Incidencia con coordenadas; Reporte acotado; Lectura interrumpida | D-10; ver spec y tareas focales |
| RQ-11 | Compatibilidad y autoridad heredadas | Metadatos como snapshot; Unicidad y recuperación separadas | D-11; ver spec y tareas focales |
| RQ-12 | Verificación reutilizada y evidencia honesta | Verificación sintética reutilizada; Entrega de lector | D-12; ver spec y tareas focales |

## Resultado del refinamiento

Doce decisiones/requisitos definidos. Primera actividad de producto: caracterizar fidelidad de errores/fórmulas en ambos formatos y resolver su observabilidad antes de integrar. Si3.6.0 oculta errores, el adaptador debe complementarlo con estrategia documentada y probada; no puede afirmar validación completa aceptando errores como vacíos. No se autoriza silenciosamente otra biblioteca, un fork o reflexión de APIs privadas.

La solicitud de refinamiento incluye concretar y sincronizar design/spec/tasks. Implementación permanece abierta; esta revisión no autoriza DDL/DML, nuevos privilegios, autenticación, despliegue o importación de negocio.
