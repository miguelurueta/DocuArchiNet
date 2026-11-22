# DOC-123 — Verificación prevista

- Ticket: DOC-123
- Cambio OpenSpec: doc-123-importacion-excel-03-lectura-validacion-xls-xlsx
- Clasificacion: cross_cutting (Transversal)

## Evidencia requerida

En este refinamiento se revisan código/antecedentes, fuentes oficiales de paquetes y trazabilidad. Implementación deberá extender tests/ImportacionExcelBehaviorTests.cs y compile DOC122/DOC110 con fixtures binarios XLS/XLSX sintéticos. Comprobar errores/NULL/caché, coordenadas, transformaciones, tipos/charset y límites exactos/excedidos; build net461 aislado sin despliegue. Registrar comandos, SHA, ambiente y resultados al ejecutar. Esta revisión no acredita build o pruebas del lector todavía inexistente.

## QA/E2E WebForms

E2E funcional no aplica a fase03 por ausencia de recorrido o endpoint nuevo, conforme al prompt03. La planificación E2E DOC122 puede validarse sintéticamente sin login. Reutilizar tools/e2e y autenticación existente al integrar fases siguientes; autorización de DOC122 cerrada y sin transferencia. No usar datos de negocio, credenciales, SQL de instalación/limpieza o escritura real en fixtures. QA manual del contrato se documenta como tal, no como E2E.
