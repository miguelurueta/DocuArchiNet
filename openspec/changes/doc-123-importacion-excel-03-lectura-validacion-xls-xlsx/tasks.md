<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09,D-10,D-11,D-12 -->
## Plan ejecutable DOC-123

Perfil legacy-webforms-vb. Solicitud actual: completar refinamiento y actualizar este plan genérico. Las tareas1.2 en adelante son implementación/verificación futura. La tarea1.2 es compuerta de integración por la pérdida de errores→NULL del paquete3.6.0; no marcar entrega completa sin resolverla.

## 1. Contrato y lector

- [x] 1.1 [S] Consolidar este refinamiento y sus contratos de fase03. Área/archivos: `openspec/changes/doc-123-importacion-excel-03-lectura-validacion-xls-xlsx/`. Origen: D-01, RQ-01. Verificación: Matriz de12 D/RQ y OpenSpec estricto.
- [ ] 1.2 [M] Caracterizar errores, NULL, fórmulas y formato00000 en XLS/XLSX y documentar estrategia de fidelidad antes de integrar. Área/archivos: `tests/Fixtures/ProduccionDocumental/ImportacionExcel/; tests/ImportacionExcelBehaviorTests.cs; design.md`. Origen: D-02, RQ-02. Verificación: Error y NULL distinguibles en los dos formatos; sin ello integración bloqueada.
- [ ] 1.3 [S] Fijar paquetes y referencias compatibles con net461 sin cambiar FastExcel. Área/archivos: `packages.config; GestionDocumental-Docuarchi.net.vbproj`. Origen: D-02, RQ-02. Verificación: ExcelDataReader3.6.0 net45 y ExcelNumberFormat1.1.0 net20 resueltos.
- [ ] 1.4 [M] Añadir contratos internos de celda, fila, límites y resultado. Área/archivos: `Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb`. Origen: D-01, RQ-01. Verificación: Consumidor sintético no requiere HttpContext ni DB y mantiene contratos DOC122.
- [ ] 1.5 [M] Implementar apertura por formato real, catálogo de hojas y selección exacta. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb`. Origen: D-03, RQ-03. Verificación: Libro incorrecto/hoja ausente rechazados y stream del consumidor preservado.
- [ ] 1.6 [M] Conservar posiciones físicas y resolver encabezados sin ambigüedad. Área/archivos: `Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb; Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb`. Origen: D-04, RQ-04. Verificación: Fixtures incluyen primera/última fila, huecos y columnas no mapeadas.
- [ ] 1.7 [M] Aplicar límites inclusivos de bytes y filas sin aceptar prefijos. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb; Infrastructure/ProduccionDocumental/ImportacionExcel/LectorArchivoExcel.vb`. Origen: D-05, RQ-05. Verificación: Máximo exacto aceptable y máximo+1 con lectura incompleta/diagnóstico.
- [ ] 1.8 [M] Aplicar transformaciones guardadas en orden y validar segmento base0. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb`. Origen: D-06, RQ-06. Verificación: Opciones NULL/0/1 y segmento inválido devuelven valores/errores esperados.
- [ ] 1.9 [M] Preservar texto/formato visual y validar longitud/charset físicos. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb`. Origen: D-07, RQ-07. Verificación: 00123, apóstrofe/n, BENEFICIARIO ampliado y Unicode según charset.
- [ ] 1.10 [M] Normalizar números con validación exacta de rango y precisión. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb`. Origen: D-08, RQ-08. Verificación: BIGINT unsigned y DECIMAL65 sin overflow/redondeo/cultura implícita.
- [ ] 1.11 [M] Validar DATE, nulabilidad y clave conforme al tipo físico. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb`. Origen: D-09, RQ-09. Verificación: Dos formatos DATE, fechas imposibles, FECHA VARCHAR y clave vacía.
- [ ] 1.12 [M] Entregar resultado completo e incidencias con coordenadas y muestra acotada. Área/archivos: `Services/ProduccionDocumental/ImportacionExcel/ServicioLecturaExcel.vb; Modelo/ProduccionDocumental/ImportacionExcel/ImportacionExcelModelos.vb`. Origen: D-10, RQ-10. Verificación: Error enC8 y reporte>1000 mantienen coordenadas, total y validez falsa.

## 2. Verificación sintética y compilación

- [ ] 2.1 [M] Añadir fixtures sintéticos reales de ambos formatos con expectativas y procedencia reproducibles. Área/archivos: `tests/Fixtures/ProduccionDocumental/ImportacionExcel/`. Origen: D-12, RQ-12. Verificación: BIFF/OpenXML válidos, sin datos de negocio, con extremos/hojas/huecos.
- [ ] 2.2 [M] Ampliar compilador/harness existente y verificar contenido esperado de libros. Área/archivos: `tests/doc122-importacion-excel-behavior.test.cjs; tests/ImportacionExcelBehaviorTests.cs`. Origen: D-12, RQ-12. Verificación: Suite nativa lee bytes reales XLS/XLSX con referencias y fuentes nuevas.
- [ ] 2.3 [M] Acreditar que errores y caché no utilizable impiden lectura válida. Área/archivos: `tests/ImportacionExcelBehaviorTests.cs`. Origen: D-02, RQ-02. Verificación: Casos de caracterización pasan contra adaptador final sin NULL falso.
- [ ] 2.4 [M] Verificar conversiones y transformaciones en culturas distintas. Área/archivos: `tests/ImportacionExcelBehaviorTests.cs`. Origen: D-08, RQ-08. Verificación: Fronteras signed/unsigned/65 dígitos, texto, fechas y charset conservan resultado.
- [ ] 2.5 [M] Verificar límites y resultados incompletos con libros sintéticos. Área/archivos: `tests/ImportacionExcelBehaviorTests.cs`. Origen: D-05, RQ-05. Verificación: Bytes/filas exactos y excedidos no producen prefijo confirmable.
- [ ] 2.6 [S] Ejecutar regresión heredada y comprobar ausencia de cambios en superficies compartidas. Área/archivos: `tests/doc122-importacion-excel-behavior.test.cjs; tests/doc122-e2e-reuse.test.cjs; FastExcel/Class_FastExcel.vb; webservice/WebServiceImportacionExcel.asmx.vb`. Origen: D-11, RQ-11. Verificación: DOC122/DOC110 pasan y planificación E2E sintética pasa sin autenticación.
- [ ] 2.7 [S] Compilar el proyecto real con salidas aisladas y sin despliegue. Área/archivos: `GestionDocumental-Docuarchi.net.vbproj; .tmp/doc123-build/`. Origen: D-02, RQ-02. Verificación: MSBuild net461 sin sustituir DLL IIS; referencias copiadas a salida aislada.

## 3. Documentación y aceptación

- [ ] 3.1 [S] Documentar contrato de lector, estrategia de fidelidad y evidencia focal. Área/archivos: `Doc/Actualizacion/Importacion/Diseno/03-lectura-validacion-doc123.md; Doc/Actualizacion/Importacion/Validacion/; Doc/Tecnica/Opsxj/doc-123-importacion-excel-03-lectura-validacion-xls-xlsx/`. Origen: D-12, RQ-12. Verificación: SHA/ambiente/comandos/resultados y E2E no aplicable al lector expresos.
- [ ] 3.2 [S] Verificar trazabilidad y criterios de aceptación de la entrega. Área/archivos: `openspec/changes/doc-123-importacion-excel-03-lectura-validacion-xls-xlsx/`. Origen: D-12, RQ-12. Verificación: Refinement, OpenSpec estricto y revisión contrastan evidencia real, sin afirmar E2E del lector.

## Dependencias y límites

1.2 precede integración de1.5/1.12 y aceptación2.3. 1.3/1.4 preceden lector;1.5/1.6 preceden normalización;1.8 precede validación de tipos. Los fixtures2.1 pueden prepararse junto a1.2. Primero ejecutar pruebas focales; luego regresión y build aislado.

D-01/RQ-01: contrato puro. D-02/RQ-02: paquetes/fidelidad/build. D-03/RQ-03: formato/hoja. D-04/RQ-04: coordenadas. D-05/RQ-05: límites. D-06/RQ-06: transformaciones. D-07/RQ-07: texto. D-08/RQ-08: números. D-09/RQ-09: fecha/NULL. D-10/RQ-10: resultado. D-11/RQ-11: compatibilidad/autoridad. D-12/RQ-12: evidencia.

Sin ASHX/ASMX nuevo, UI, SQL, autenticación, despliegue ni importación real. La verificación del refinamiento no marca build/pruebas futuras como realizados.
