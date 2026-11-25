<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12,D-13,D-14 -->
# DOC-127 — Verificación integral, capacidad y cierre

Base inspeccionada: `d1d143954bcd0dc45198b862437d241e6dcc5643`; producto integrado desde `15ef5bb711fe5e3cdbf302efaeefd067cc51aa9d` (DOC126). Perfil `legacy-webforms-vb`, net461, MySQL5.1.37 y MySql.Data6.2.3. La fase acredita la capacidad existente y corrige brechas demostradas; no reconstruye importación, conexiones ni E2E.

## Context

Antecedente: `Doc/Actualizacion/Importacion/Exploracion/carga-excel-csv-tablas-imp-02.md`. El prompt conserva10000 iniciales; DOC124 elevó expresamente la configuración a20000 y DOC125/126 la preservaron. El fallback del lector sigue10000 y el tamaño predeterminado10MB equivale a10000000bytes. La ejecución utiliza la conexión DA del módulo y los recursos del repo, según la decisión DOC122; no se añade otra cadena en Web.config.

## Goals / Non-Goals

Acreditar integración, capacidad, límites y recuperación con matriz saneada; resolver brechas reproducibles y entregar operación/instalación verificables. Fuera de alcance: CSV, upsert, omisión de duplicados, migración entre gabinetes, pausa por fila, publicación automática de fuentes/proveedores, cambio de framework/conector y modificaciones a SII o AlmacenamientoDocumentoProduccionDocumental.

## Inventario exacto previo a cualquier cambio de producto

Las rutas siguientes existen. Su inventario y hashes no afirman que se hayan ejecutado de nuevo ni que requieran edición. Inspección focal de responsabilidades, símbolos y evidencias; una corrección futura exige leer completamente el archivo afectado antes de editarlo. Las clases adicionales a las previstas por Jira (recepción, análisis, writer y estructura) ya separan responsabilidades y se reutilizan.

### Modelos y contratos

- `Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb`
- `DTOs/ProduccionDocumental/ImportacionExcel/ImportacionExcelDtos.vb`

### Servicios

- `Services/ProduccionDocumental/ImportacionExcel/ServicioPlantillasExcel.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ServicioRecepcionExcel.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ServicioPreparacionExcel.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ServicioAnalisisPlantillaExcel.vb`
- `Services/ProduccionDocumental/ImportacionExcel/ReglasCargaExcel.vb`

### Infraestructura

- `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelContextAdapter.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelConexionAdapter.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/InspectorFidelidadExcel.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/AlmacenArchivoExcel.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ReferenciaRecepcionExcel.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/PoliticaDestinoCargaExcelAdapter.vb`
- `Infrastructure/ProduccionDocumental/ImportacionExcel/ComposicionCargaExcel.vb`

### Repositorios

- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPlantillaExcelRepository.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlCargaExcelRepository.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPreparacionExcelValidator.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlEstructuraExcel.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/SqlCargaExcel.vb`

### HTTP

- `webservice/WebServiceImportacionExcel.asmx`
- `webservice/WebServiceImportacionExcel.asmx.vb`
- `generic_control/ImportacionExcelUpload.ashx`
- `generic_control/ImportacionExcelUpload.ashx.vb`
- `generic_control/ImportacionExcelAnalisis.ashx`
- `generic_control/ImportacionExcelAnalisis.ashx.vb`

### Interfaz e integración

- `js/gestion/ImportacionExcel/ImportacionExcelApi.js`
- `js/gestion/ImportacionExcel/ImportacionExcelVista.js`
- `Styles/ImportacionExcel.css`
- `Gestion/WebFormProducionDocumental.aspx`
- `Gestion/WebFormProducionDocumental.aspx.vb`
- `Gestion/Class_remit_dest_interno_perfil_produccion.vb`
- `js/gestion/ImportacionCarpetas/PerfilImportacionEditor.js`

### Consumidores

- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb`
- `Infrastructure/ProduccionDocumental/ImportacionCarpetas/PoliticaFuentesProveedor.vb`
- `Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorIndicesRepository.vb`
- `Infrastructure/Workflow/ImportarServicioWeb/Sii/SiiImportProvider.vb`

### Pruebas

- `tests/ImportacionExcelBehaviorTests.cs`
- `tests/ImportacionExcelLecturaTests.cs`
- `tests/ImportacionExcelPreparacionTests.cs`
- `tests/ImportacionExcelConfirmacionTests.cs`
- `tests/doc122-importacion-excel-behavior.test.cjs`
- `tests/doc122-e2e-reuse.test.cjs`
- `tests/doc124-e2e-reuse.test.cjs`
- `tests/doc125-e2e-reuse.test.cjs`
- `tests/doc126-e2e-reuse.test.cjs`
- `tests/doc126-importacion-excel-ui.test.cjs`
- `tests/doc126-importacion-excel-client.test.cjs`
- `tools/e2e/tests/importacion-excel.spec.cjs`
- `tools/e2e/scripts/run-doc110-profile-session.cjs`
- `tools/e2e/tests/support/doc124-excel-scope.cjs`
- `tools/e2e/tests/support/doc125-excel-scope.cjs`
- `tools/e2e/tests/support/doc126-excel-scope.cjs`

### Proyecto y operación

- `GestionDocumental-Docuarchi.net.vbproj`
- `Web.config`
- `tools/e2e/AGENT-RUNBOOK.md`
- `Doc/Actualizacion/Importacion/Sql/README.md`

No se propone código de producto nuevo durante el refinamiento. Extensiones de scope/muestreo se incorporarán al runner y suite existentes, con su inventario antes de editar. Un nuevo archivo VB/content se registra en el vbproj sólo cuando pertenezca al producto compilado; documentación y evidencia no requieren Compile/Content artificiales.

## Corte de evidencia y brechas

| Evidencia previa | Resultado acreditado | Límite que conserva DOC127 |
| --- | --- | --- |
| DOC122/123 | autorización/plantillas y lector/harness | lector local; no E2E física de fase03 |
| DOC124 | CREAR12759, AGREGAR8000 y REEMPLAZAR8000 preparados | cero filas confirmadas en esos recorridos; no benchmark |
| DOC125 |1000 confirmadas por cada modo; rollback/carrera reales; tiempos por etapa y muestras IIS | no5000/10000 por formato; memoria agregada parcial; COMMIT/DDL adverso principalmente local |
| DOC126 | tres modos UI1000, asistente, rollback/carrera y lectura exclusiva/restringida v6 | ajuste final v7 sólo local; no atribuir lectura real v7 ni20000 confirmadas |

Cada resultado conserva SHA/runtime/hashes originales. Comparar las fuentes de evidencia con el corte actual; el backend histórico distinto no invalida automáticamente toda evidencia de un consumidor intacto ni acredita cambios nuevos. Conservar las cinco cargas y tablas DOC126 y sus78 perfiles; las tablas de prueba contienen claves consumidas. Toda reutilización futura exige nuevos IDs/rangos, huellas y consentimiento específico. Los datos EGRESOS/CONTRATOS/CRÉDITOS se protegen; no son descartables por su nombre.

## Decisions

### D-01 — Base y reutilización de evidencia (RQ-01)

Fijar SHA, hashes normalizados de fuentes, hashes binarios del runtime, configuración efectiva y alcance por caso. Reutilizar resultados anteriores sólo con correspondencia de componente y condiciones; conservar diferencias y fechas.

Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc126-verificacion-final-20261125.json; doc125-verificacion-final-20261124.json`. Resultado exigido: Una matriz identifica origen, caso, SHA, ambiente, tipo de evidencia, resultado y limitación. La aceptación histórica no equivale a una ejecución actual.

### D-02 — Infraestructura E2E compartida (RQ-02)

Extender el runner DOC110, la suite Excel y los validadores DOC124–126. Crear alcances DOC127 cerrados por ventana; no reabrir scopes, IDs ni carreras consumidas.

Evidencia: `tools/e2e/scripts/run-doc110-profile-session.cjs; tools/e2e/tests/importacion-excel.spec.cjs; tools/e2e/tests/support/doc126-excel-scope.cjs`. Resultado exigido: Cada ejecución real identifica autorización, recursos, efectos y presupuesto antes de autenticar. Las credenciales siguen efímeras; los controles externos son SELECT.

### D-03 — Autoridad y preparación (RQ-03)

Comprobar permiso exclusivo, denegado y revocado; empresa, propietario, CSRF y referencias. Preparar conserva tablas de negocio y configuración, aunque registra la ejecución y usa temporales propias.

Evidencia: `Services/ProduccionDocumental/ImportacionExcel/ReglasCargaExcel.vb:Autorizar/MismaAutoridad; Infrastructure/ProduccionDocumental/ImportacionExcel/ImportacionExcelRequestGuard.vb:Validar`. Resultado exigido: Se rechazan referencias ajenas y operaciones sin autoridad sin efectos funcionales. Revocación real necesita gestión manual autorizada; la cobertura sintética se distingue.

### D-04 — Instalación y SQL manual (RQ-04)

Revisar instalación existente sin reaplicar SQL00–27. Entregar un preflight DOC127 de SELECT y archivos nuevos específicos sólo para diferencias demostradas; el usuario aplica y confirma cada cambio antes del SELECT posterior.

Evidencia: `Doc/Actualizacion/Importacion/Sql/README.md; 03-ejecucion-da-doc121.sql; 22-preflight-doc126.sql`. Resultado exigido: El estado distingue ENTREGADO, EJECUTADO_CONFIRMADO y VERIFICADO_SELECT por archivo y destino RA/DA. Las excepciones históricas delegadas no autorizan otras aplicaciones.

### D-05 — Formatos, plantillas y modos (RQ-05)

Reutilizar el lector XLS/XLSX y las plantillas históricas/web. Verificar crear/agregar/reemplazar conservando revisión, mapeos y modo histórico; cada formato queda identificado.

Evidencia: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb:Leer; ServicioPlantillasExcel.vb:ModoHistorico/FormatoHistorico`. Resultado exigido: Los casos trazan formato, plantilla, hoja, clave, revisión, modo y filas efectivas. CSV, upsert y omisión de duplicados continúan excluidos.

### D-06 — Integridad del destino (RQ-06)

Verificar claves requeridas y comparación efectiva MySQL según tipo/collation, índice UNIQUE completo, motor, política empresarial y compatibilidad de columnas. Agregar falla íntegramente ante duplicados; reemplazar conserva aislamiento.

Evidencia: `Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlPreparacionExcelValidator.vb:Validar/Inspeccionar; MySqlConfirmacionExcelWriter.vb:Escribir`. Resultado exigido: Los rechazos conservan las filas de negocio y señalan su código y fase. No truncar datos ni alterar collation, índices históricos o BENEFICIARIO para adaptar la prueba.

### D-07 — Recuperación y concurrencia (RQ-07)

Conservar confirmación por ID, consulta histórica, locks de carga/destino y marca CREATE. Diferenciar replay/acuse ignorado de cortes físicos de COMMIT/red; comprobar rollback y carrera con observación del escritor activo.

Evidencia: `Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb:Confirmar/ConsultarResultado; Infrastructure/Repositories/ProduccionDocumental/ImportacionExcel/MySqlConfirmacionExcelWriter.vb:Ejecutar; MySqlEstructuraExcel.vb:Marca/Asegurar`. Resultado exigido: No se duplica una carga confirmada y la incertidumbre exige consulta antes de reintentar. CREATE DDL no es reversible como el DML; no borrar tablas para simular recuperación.

### D-08 — Límites efectivos (RQ-08)

Conservar MaxFilasDatos=20000 vigente y 10 MB decimales por defecto; el lector mantiene fallback10000. Cubrir 10000/10001 bajo configuración10000 y 20000/20001 bajo20000, además de 10000000/10000001 bytes y cambios entre fases.

Evidencia: `Infrastructure/ProduccionDocumental/ImportacionExcel/ComposicionCargaExcel.vb:New; Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb:ValidarActual; Web.config:ImportacionExcel.MaxFilasDatos`. Resultado exigido: La frontera se interpreta según configuración del servidor y se informa el máximo efectivo. Los cambios de configuración se prueban localmente; cualquier ajuste real requiere alcance propio y restauración verificada.

### D-09 — Capacidad medible (RQ-09)

Completar seis celdas formato/tamaño: XLS y XLSX por1000/5000/10000, separadas por modo ejecutado. Medir preparación, confirmación y memoria con metodología, baseline, intervalo y cobertura; no extrapolar a20000.

Evidencia: `Doc/Actualizacion/Importacion/Validacion/doc125-ejecucion-seguimiento-20261124.md; tools/e2e/tests/importacion-excel.spec.cjs`. Resultado exigido: El informe presenta filas, bytes, tiempos, memoria, resultado y huecos por celda y modo. Los tiempos HTTP incluyen metadatos y transporte; memoria agregada IIS no es asignación exclusiva por carga.

### D-10 — Fuentes de índices y consumidores (RQ-10)

Verificar la lectura existente por clave y la regresión de perfiles/proveedores, SII y carpetas. Importar una tabla no publica una fuente ni un proveedor: siguen su configuración/autorización propias. Entregar EXPLAIN para ejecución manual sobre la consulta real.

Evidencia: `Infrastructure/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorDatosIndices.vb:ConsultarDatos; PoliticaFuentesProveedor.vb:PermiteOperativa; Infrastructure/Repositories/ProduccionDocumental/ImportacionCarpetas/MySqlProveedorIndicesRepository.vb:ObtenerVersion`. Resultado exigido: El plan y metadatos acreditan la consulta por igualdad con filtro empresarial cuando aplica y LIMIT2. No ejecutar EXPLAIN desde el agente; SELECT sólo para controles posteriores. No cambiar SII/almacenamiento para esta fase.

### D-11 — Interfaz vigente (RQ-11)

Preservar semáforo, asistente, errores explicativos, recuperación por ID y foco/Cerrar/Escape. Distinguir positivos reales v5, lectura real v6 y ajuste v7 cubierto localmente; evaluar una lectura v7 sólo en ventana autorizada.

Evidencia: `js/gestion/ImportacionExcel/ImportacionExcelVista.js; ImportacionExcelApi.js; tests/doc126-importacion-excel-ui.test.cjs`. Resultado exigido: El usuario ve prerrequisitos, acciones habilitadas y diagnóstico accionable sin perder cierre o consulta. La evidencia manual previa no cubre automáticamente toda corrección posterior.

### D-12 — Build y correcciones focales (RQ-12)

Compilar net461 con MSBuild en salida aislada y ejecutar los harness vigentes. Corregir únicamente brechas reproducibles con prueba de reproducción; vincular nueva evidencia a las huellas cambiadas sin desplegar automáticamente.

Evidencia: `GestionDocumental-Docuarchi.net.vbproj; tests/doc122-importacion-excel-behavior.test.cjs:compile; tests/ImportacionExcelConfirmacionTests.cs`. Resultado exigido: El build y la regresión relevante se ejecutan en el corte evaluado y sus logs son verificables. Conservar DLL IIS, fuentes ajenas y versiones MySQL5.1.37/MySql.Data6.2.3.

### D-13 — Guía de operación y despliegue (RQ-13)

Actualizar documentación existente con instalación RA/DA, confirmación manual, preflight, operación y recuperación. La carga confirma atómicamente; consultar/reintentar por ID no ofrece pausa por fila ni migración parcial.

Evidencia: `Doc/Actualizacion/Importacion/Sql/README.md; Services/ProduccionDocumental/ImportacionExcel/ServicioConfirmacionExcel.vb:Confirmar`. Resultado exigido: La guía permite instalar/verificar y operar con límites y procedimientos de recuperación explícitos. No retirar estructuras ni limpiar fixtures/resultados para preparar el cierre.

### D-14 — Cierre verificable (RQ-14)

Entregar matriz requisito-caso-prueba-SHA-ambiente-resultado, informe de capacidad y restricciones. Revisión, validación, publicación y archivo siguen OPSXJ por el SHA; los huecos críticos impiden cierre.

Evidencia: `tools/opsxj/README.md:Compuerta de refinement; openspec/changes/archive/2026-11-25-doc-126-importacion-excel-06-interfaz-menu/tasks.md`. Resultado exigido: Cada tarea cerrada tiene evidencia del resultado y los bloqueos conservan impacto y siguiente acción. No convertir aprobación técnica de planificación en aceptación humana, aplicación SQL o ejecución real.

## Protocolo de capacidad

Comandos locales previstos, sin ejecutar durante el refinamiento:

```powershell
node --test tests/doc122-importacion-excel-behavior.test.cjs
node --test tests/doc122-e2e-reuse.test.cjs tests/doc124-e2e-reuse.test.cjs tests/doc125-e2e-reuse.test.cjs tests/doc126-e2e-reuse.test.cjs
node --test tests/doc126-importacion-excel-client.test.cjs tests/doc126-importacion-excel-ui.test.cjs
```

La primera orden compila el harness VB/C# y ejecuta también su regresión heredada; no sustituye el build MSBuild del proyecto completo. Resolver MSBuild desde la instalación local y usar salidas absolutas bajo `.tmp/doc127-build/bin/` y `.tmp/doc127-build/obj/`, con despliegue desactivado. Registrar el comando real resuelto y la huella de IIS antes/después. La selección de regresiones de consumidores se obtiene del harness DOC110 y del contrato SII vigente, evitando ejecutar recorridos autenticados por una coincidencia de nombre.

La matriz tiene seis celdas formato/tamaño; dentro de cada una se registra cada modo CREAR/AGREGAR/REEMPLAZAR. Puede haber hasta18 recorridos si ninguno es reutilizable. Esa cifra es un plan de cobertura, no una autorización de18 escrituras. Cada celda/método tiene estado propio y el formato del archivo histórico se comprueba antes de reutilizarlo. No sustituir5000 o10000 por8000/12759, ni preparación por confirmación.

Generar archivos sintéticos mediante utilidades/fixtures vigentes, con SHA/bytes y claves nuevas. El scope declara cuenta, empresa, RA/DA, plantilla/revisión/huella, archivo, tabla, filas previas/previstas y efectos máximos. La elección concreta de recursos se resuelve por SELECT y autorización antes de ejecutar; no presumir que una tabla actual está vacía. Si necesita fixtures o políticas nuevos, entregar SQL manual primero. Los cambios de permisos tampoco se hacen desde tests.

Cronometrar preparar/confirmar individualmente; distinguir transporte, preflight y DML. Reutilizar muestreo IIS existente y ampliar su cobertura, con baseline/intervalo/procesos y picos observados. No atribuir memoria total compartida a una carga. El presupuesto se acuerda por ventana a partir de evidencia disponible; excederlo detiene nuevos efectos y deriva a consulta por ID. No aumentar HTTP30s ni SQL/locks/lotes sin evidencia, decisión explícita y regresión afectada.

Exactamente10MB exige un documento reconocido por el lector, con hash y contenedor verificados; no usar padding inválido para confundir tamaño con corrupción. Fronteras10000/20000 y cambio de configuración tienen primero cobertura sintética sin tocar IIS. La confirmación física de20000 sólo puede anunciarse si se ejecuta aparte con alcance autorizado.

## Consulta de índices y EXPLAIN

La consulta existente en MySqlProveedorDatosIndices.ConsultarDatos es SELECT de propiedades autorizadas FROM tabla WHERE clave=@clave, con AND empresa=@empresa si aplica y LIMIT2. La clave procede de la configuración del proveedor y del valor resuelto por el perfil; el importador no cambia ese contrato. Generar EXPLAIN de la misma proyección/predicados y tipo de valor, con identificadores elegidos en servidor y un valor sintético saneado. El usuario ejecuta el plan manualmente. Revisar key/possible_keys/type/rows y el índice completo con SELECT; no asegurar uso del índice sólo porque exista. No crear proveedor/fuente ni ejecutar una importación de carpetas/SII con efectos para acreditar una lectura.

## Risks / Trade-offs

La fase exige más capacidad que la evidencia anterior: constituye trabajo de aceptación, no un defecto probado. Las pruebas de revocación, COMMIT incierto y recuperación DDL reales requieren una ventana y mecanismo concreto adicionales; hasta entonces conservan su cobertura local y limitación explícita. El ajuste v7 no se anuncia real por tener HTTP200 o49 tests locales. DDL CREATE puede sobrevivir al rollback DML y sólo su marca/esquema permite recuperación segura.

## Migration Plan

No se necesita una migración nueva para refinar. Inventariar aplicación previa y revisar metadatos; nunca aplicar en lote00–27. Entregar preflight SELECT y EXPLAIN manual en archivos DOC127 nuevos. Si hay diferencias, producir un archivo específico y guía RA/DA con precondiciones, efectos, confirmación y SELECT posterior. No automatizar limpieza o reversión de esquema; preservar cambios ajenos y evidencia previa. Compilar/suite en salida aislada no despliega la DLL. Cualquier despliegue futuro deberá identificar hashes y preservar el binario del usuario mediante el procedimiento autorizado.

## Open Questions

### Inventario concreto de la ampliación local DOC127

Se amplían `tools/e2e/scripts/run-doc110-profile-session.cjs`, `tools/e2e/tests/importacion-excel.spec.cjs` y `tools/e2e/tests/support/doc125-excel-scope.cjs` con el modo `--excel-capacity`: una combinación formato/tamaño/modo por ventana, validada y consumida antes de autenticar. El soporte DOC126 sigue verificando runtime y cuentas; DOC32 conserva consultas y huellas. Los scopes DOC125/126 mantienen sus condiciones anteriores.

Nuevo archivo `tools/e2e/scripts/support/doc127-iis-memory.ps1`: generaliza el muestreo Get-Process utilizado en DOC125; baseline, fases e intervalo solicitado de 2 segundos, con totales compartidos de IIS. No consulta base ni instala nada. Un resultado funcional con medición insuficiente conserva su ID y queda incompleto, sin repetir escrituras.

Se amplían `tests/ImportacionExcelLecturaTests.cs` con ZIP/XML válidos de tamaño exacto y `tests/Fixtures/ProduccionDocumental/ImportacionExcel/generar.py` con generación opcional de las 18 combinaciones, conservando los nueve libros anteriores. Nuevo `tests/doc127-e2e-reuse.test.cjs`: guardas y transporte sintético, sin resultados reales persistidos. Estos archivos de pruebas/herramientas no requieren entradas Compile/Content en el vbproj del producto.

Nuevos `tests/doc127-capacity-fixtures.test.cjs` y `tests/support/Doc127CapacityFixtureReader.cs`: compilación mediante el harness DOC122 y validación reproducible de las 18 combinaciones con el lector de producto, sobre archivos locales sintéticos y sin MySQL/HTTP. La regeneración de los nueve libros históricos conserva sus hashes.

Entregables nuevos: `Sql/28-preflight-doc127.sql`, `Sql/29-explain-proveedor-doc127.sql`, `Doc/Actualizacion/Importacion/Operacion-Excel.md` y evidencia local DOC127. Los dos SQL son de inspección; EXPLAIN se ejecuta manualmente. No se altera código de producto, instalación, configuración o runtime de IIS.

No hay decisión de producto sin resolver para iniciar la aceptación local. La autorización de ventanas reales, la elección verificable de recursos y la aplicación/plan SQL son dependencias operativas explícitas de las tareas, no autorizaciones inferidas del refinamiento.
