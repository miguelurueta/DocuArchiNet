# PROCESAMIENTO-SERVIDOR — PRUEBAS Y EVIDENCIA

Fecha: 2026-10-14.

| Prueba | Resultado |
| --- | --- |
| `MSBuild GestionDocumental-Docuarchi.net.vbproj` | PASS; advertencias legacy preexistentes, sin error de compilación |
| `npm.cmd --prefix tools/e2e run test:doc104:server-processing:policy` | PASS, 29/29 |
| `npm.cmd --prefix tools/e2e run test:doc104:server-processing -- --mode double` | PASS |
| `npm.cmd --prefix tools/e2e run test:doc104:transparency` | PASS; matriz RGBA real |
| `npm.cmd --prefix tools/e2e run test:doc104:pdf-transaction` | PASS; PDF sintético, staged, replace, backup y lock |
| `npm.cmd --prefix tools/e2e run test:doc104:characterization` | PASS reproducible; rotaciones 0/90/180/270, huellas visuales, matriz 90x90 y clasificación posterior `stream` |
| `npm.cmd --prefix tools/e2e run test:workflow:pdf-viewer-streaming:policy` | PASS, 11/11 |
| `openspec.cmd validate doc-104-procesamiento-servidor --strict` | PASS |
| `opsxj:refine -- DOC-104` | PASS, trazabilidad completa |
| `opsxj:technical-review -- 06-procesamiento-servidor-grafo.md --tech-profile legacy-webforms-vb` | PASS; 0 blocker, 0 major, 0 minor, 1 recomendación informativa de revisión manual |
| `git diff --check` | PASS |
| E2E mutadora DOC-104, escenario `normal` | PASS; 1 POST, efecto único y controles `SELECT` completos |

## Alcance de la evidencia local

- Ninguna prueba local leyó o modificó PDFs del sistema.
- La transacción focal crea PDFs sintéticos en un directorio temporal y los elimina en `finally`.
- La evidencia del modo doble tiene esquema cerrado y no conserva rutas, consultas o secretos.

## Validación real

La E2E mutadora del escenario normal se ejecutó con autorización explícita de ambiente, cuenta y PDF/grafo descartables. Produjo exactamente un POST, un efecto confirmado y controles antes/después exclusivamente `SELECT`; la evidencia saneada no conserva rutas, consultas, cuentas, credenciales ni contenido.

La primera tentativa fue rechazada antes de mutar porque el cliente enviaba simultáneamente ruta y base64. Se corrigió el contrato para seleccionar una única fuente y se agregó diagnóstico inmediato de respuestas ASMX. La validación real de un documento registrado como firmado permanece pendiente; las rotaciones y `/Linearized` se caracterizaron exclusivamente con fixtures sintéticos descartables.

## Caracterización sintética

- Se generaron cuatro PDFs descartables locales con rotaciones 0, 90, 180 y 270.
- Las huellas SHA-256 del contenido descomprimido de página fueron idénticas en dos ejecuciones consecutivas del arnés y distintas entre fixtures.
- Cada salida conservó una página y una matriz de imagen 90 x 90 en la posición calculada por el algoritmo legacy.
- Los archivos de entrada conservaron su huella física y cada salida pudo abrirse en modo exclusivo, moverse y restituirse inmediatamente.
- El fixture mayor de 1 MiB con marcador `/Linearized` clasificó `range` antes del estampado. La salida iText conservó el marcador pero quedó por debajo del umbral, por lo que clasificó correctamente `stream`; no se ejecutó linealización externa.
- La matriz transaccional comprobó staged inválido, fallo de reemplazo, revalidación fallida con rollback, exclusión sobre el mismo documento, paralelismo de locks distintos y liberación de recursos.

La barrera de certificado permanece pendiente de validación con un documento descartable registrado realmente como firmado; no se reemplazó por mocks ni por alterar datos de certificado.
