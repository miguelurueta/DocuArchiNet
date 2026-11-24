## Why

DOC124 recibe y prepara un Excel, pero todavía no importa sus filas. DOC125 completa la confirmación con resultado durable: un fallo revierte las filas y una respuesta perdida se resuelve por el mismo ID, sin duplicar efectos.

## What Changes

- Confirmar y ConsultarResultado sobre las cargas privadas PREPARADA existentes; ampliar el resultado seguro con filas confirmadas y fecha.
- Implementar CREAR, AGREGAR y REEMPLAZAR con autoridad vigente, snapshots, unicidad MySQL y transacción de filas/resultado.
- Recuperar CREATE mediante procedencia estructural y distinguir rollback conocido de commit incierto.
- Reutilizar recursos RA/DA, lector, harness y E2E; entregar SQL adicional como archivos manuales cuando sea necesario.
- Conservar el máximo configurado de 20000 filas y 10000000 bytes. La interfaz corresponde a fase06.

## Capabilities

### New Capabilities
- `importacion-excel-05-confirmacion-recuperacion`: confirmación durable, aislamiento por destino, recuperación de resultado y creación identificable.

### Modified Capabilities

Los contratos previos de recepción y preparación se conservan; el nuevo resultado extiende sus DTOs sin cambiar su semántica.

## Impact

Servicios VB, DTOs/modelos, repositorios MySQL, composición y ASMX existentes; tres archivos VB nuevos registrados en vbproj. No se cambia framework, conector, conexiones ni consumidores SII/carpetas. Se reutilizan las 22 columnas operativas DOC121. SQL18 es diagnóstico SELECT; una adecuación adicional requiere archivo propio y ejecución manual.

Dependencia DOC124 integrada por PR136 y Jira Listo. La fuente íntegra del ticket se conserva en `specs/importacion-excel-05-confirmacion-recuperacion/jira-context.md`; el prompt05 y sus antecedentes permanecen en Doc/Actualizacion/Importacion/Exploracion/. Este refinamiento aplica la instrucción de continuar; no acredita implementación, SQL aplicado ni autorización nueva para escrituras E2E.
