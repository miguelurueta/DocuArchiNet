# DOC-123 — Verificación local del lector

Fecha:2026-11-22. Revisión técnica:Codex. Rama:feature/DOC-123. Fuente anterior:0e7c9a54. El manifiesto JSON acompaña hashes SHA256 de los archivos exactos de implementación y pruebas; identifica el candidato validado sin confundir el SHA anterior con código nuevo.

## Resultado

- Suite reutilizada:7/7 PASS,0 fallos,0 omitidos;312 comprobaciones del lector y246 DOC122; ejecutable heredado DOC110/DOC120 PASS.
- MSBuild del proyecto real net461:PASS,0 errores,308 advertencias legacy; sin advertencias de los archivos nuevos del lector. Salidas bajo .tmp/doc123-build/, sin despliegue.
- Nueve libros sintéticos XLS/XLSX con hashes verificados; CFB FAT regular/miniFAT y OpenXML.
- DLL desplegada sin cambio:SHA256 79da3a8841e22d8c5835b2495ceb0179946c67b204effcf326c2d8030c722779. No se instaló SQL ni se importó negocio.

## Comandos y ambiente

Windows, compiladores Roslyn/MSBuild de Visual Studio; .NET Framework4.6.1. Paquetes ExcelDataReader3.6.0 net45/ExcelNumberFormat1.1.0 net20.

`node --test tests/doc122-importacion-excel-behavior.test.cjs tests/doc122-e2e-reuse.test.cjs`

`msbuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:Disable_CopyWebApplication=true /p:UseSharedCompilation=false /p:OutDir=<workspace>/.tmp/doc123-build/bin/ /p:BaseIntermediateOutputPath=<workspace>/.tmp/doc123-build/obj/ /p:IntermediateOutputPath=<workspace>/.tmp/doc123-build/obj/`

Logs completos locales excluidos:.tmp/doc123-tests-final.log y .tmp/doc123-build/msbuild-final.log. Generador reproducible y expectativas en tests/Fixtures/ProduccionDocumental/ImportacionExcel/README.md. El harness amplía el ejecutable y compilador DOC122/DOC110; no crea infraestructura E2E paralela.

## Matriz de revisión

| Origen | Resultado | Evidencia revisada |
|---|---|---|
| D-01/RQ-01 | Contrato puro:PASS | ServicioLecturaExcel.Leer y Read/ReadBytes: stream permanece abierto/posición restaurada; sin DB/HTTP. |
| D-02/RQ-02 | Dependencias/fidelidad:PASS | Fidelity/Boundaries/Reports: error/NULL/caché/miniFAT/FILEPASS/cicloFAT y build real. |
| D-03/RQ-03 | Formato/hoja:PASS | Valid/Boundaries: catálogo, nombre exacto, formato discrepante, CSV/corrupto/macro rechazados. |
| D-04/RQ-04 | Coordenadas/encabezados:PASS | Valid/Reports: filas2/4/5, columnaB vacía, C8, nombre ausente/ambiguo, no mapeadas/espacios. |
| D-05/RQ-05 | Límites:PASS | Boundaries/Reports:3 y4 filas,10000/10001,1000000/1000001 bytes; configuración inválida. |
| D-06/RQ-06 | Transformaciones:PASS | Valid/Types: coma→punto→segmento, índice0/1/invalidado, separador ausente, segmento vacío y preservación. |
| D-07/RQ-07 | Texto/capacidad:PASS | Valid/Types:00123, apóstrofe/n/barras/espacios, BENEFICIARIO1000, ascii/latin1/utf8/utf8mb4 y longitud escalar. |
| D-08/RQ-08 | Números:PASS | Types/Valid: fronteras signed/unsigned, DECIMAL65 y escala, NaN/exponente, culturas es-CO/en-US. |
| D-09/RQ-09 | DATE/NULL/clave:PASS | Types/Fidelity: DATE tipado/dos formatos, inválidos/rango, FECHA VARCHAR, NULL y clave vacía. |
| D-10/RQ-10 | Diagnóstico:PASS | Reports/Fidelity: C8,1005 incidencias/muestra1000, conteo observado y descarte ante interrupción. |
| D-11/RQ-11 | Compatibilidad:PASS | 246 controles DOC122, regresión DOC110/DOC120 y seis guardas E2E sintéticas; superficies compartidas sin diff. |
| D-12/RQ-12 | Evidencia:PASS | Build aislado,7/7 tests, nueve fixtures verificables y justificación explícita E2E no aplicable. |

Los33 escenarios se contrastaron con rutas de implementación y pruebas focales; esto no declara cobertura exhaustiva de todas las variantes de archivo.

## Fidelidad resuelta

El paquete original devolvía null para error, fórmula con error y caché ausente. El inspector complementario distingue los casos sin API privada/dependencia nueva. Fórmula sin caché corta para evitar que la búsqueda de STRING en XLS consuma el siguiente registro y se ofrezca un prefijo incompleto. Caché42, vacío textual y NULL real se acreditan por separado. No se recalculan fórmulas ni se garantiza vigencia de un resultado guardado.

## Límites y evidencia no aplicable

E2E real y QA visual no aplican al lector interno sin pantalla/endpoint. Las pruebas son locales con bytes sintéticos y no equivalen a importación real, verificación MySQL, collation, pausa/reanudación o aceptación de usuario. tools/e2e y su autorización específica se reutilizarán en fases posteriores. Sin SQL nuevo, autenticación, despliegue ni cambios a EGRESOS/CONTRATOS, FastExcel/SII, resolver DA o Web.config.

La clasificación automática transversal de inicio se precisa a backend_vb: solo lector/modelos internos y dependencias. Se exige evidencia unit/build; no se falsifica QA manual o E2E para satisfacer una plantilla de UI. Los cinco documentos se conservan, incluyendo justificación de no aplicabilidad visual.

La memoria total del parser/expansión de libros y rendimiento de fase07 no se acreditan en esta entrega. Los fixtures XLS cubren BIFF8/CFBv3; no prueban todas las versiones históricas. Archivo real aportado por el usuario no se utilizó ni se copió.

Huella conjunta del candidato:4df96bf7f0de5d9d2b88026905753a181cbca2e97ddb730343575112b42bef74.
