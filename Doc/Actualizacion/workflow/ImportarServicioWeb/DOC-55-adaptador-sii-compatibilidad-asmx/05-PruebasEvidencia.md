# Pruebas y evidencia

- Ticket: DOC-55
- Cambio OpenSpec: doc-55-adaptador-sii-asmx
- Clasificacion: cross_cutting
- Fecha de validación: 2026-09-07

## Evidencia requerida

- Pruebas focales: 66/66 PASS.
- Build VB.NET: código 0.
- Diff legacy protegido: sin cambios.
- OpenSpec estricto: válido.

## Cobertura automatizada

Las pruebas `tests/importar-servicio-web-*.test.cjs` se ejecutan con Node sin conexión a SII. Los seis fixtures de `sii-v1` son saneados y contienen datos de demostración.

La prueba de almacenamiento confirma que el orquestador solo conoce `IImportExecutionStep`, que no referencia `ClassAlmacenamiento` y que `LegacyImportDocumentStorageAdapter` contiene una única invocación a `AlmacenaDocumentoTareaWorkflow`.

## Evidencia de compilación

El proyecto se compila con MSBuild de Visual Studio 2022 en configuración Debug. Las advertencias observadas corresponden a conflictos de ensamblados y código legacy preexistente; DOC-55 no introduce errores de compilación.

## Exclusiones deliberadas

No se ejecutaron E2E autenticados, carga ni llamadas reales a SII. No se activó el gate.

## QA/E2E WebForms

No aplica E2E WebForms porque no existe impacto UI y el gate permanece apagado. Conforme al runbook, no se usaron ambientes ni cuentas autenticadas.
