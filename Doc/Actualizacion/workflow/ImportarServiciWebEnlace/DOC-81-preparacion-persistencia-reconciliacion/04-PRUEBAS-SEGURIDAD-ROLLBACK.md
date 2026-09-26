# Pruebas, seguridad y rollback

## Resultados locales 2026-09-26

- Suite focal DOC-81 y regresión: PASS.
- Suite completa `tests/importar-servicio-web-*.test.cjs`: 488/488 PASS antes de las correcciones finales; las suites focales posteriores también terminaron en PASS.
- Compilación `GestionDocumental-Docuarchi.net.vbproj`: PASS, 0 errores; permanecen advertencias históricas del proyecto.
- La primera E2E mutadora autorizada sobre la tarea descartable 220586 se bloqueó antes de crear intención. La telemetría saneada confirmó `ANNEX_SUCCESS` y `information_schema` confirmó ausentes `workflow_import_intent.capability` y `workflow_import_intent.provider_reference`.
- El responsable aplicó manualmente `Sql/001-add-intent-capability.sql`; la corrida siguiente superó `QueryItems`, confirmando funcionalmente que el esquema requerido quedó disponible.
- El preflight reveló que ENLASE abre el popup antes de asignar la tarea. Se corrigió la autorización para exigir una única tarea abierta en la misma ruta sin requerir `ID_USUARIO` ni `FECHA_SELECCION`; constancias conserva la regla estricta.
- La preparación reveló que `consultarRadicado` no entrega `Libro` y que ENLASE no consume los campos de constancia. Se separó esa preparación por capacidad y la corrida siguiente alcanzó almacenamiento y reconciliación.
- La última corrida completó el handler, cumplió las expectativas de los 7 controles y marcó el recurso local como `consumed` a las 12:06:03. La emisión del JSON falló después por IDs/códigos de aserción DOC-81 incompatibles con el validador común (`E2E_PLATFORM_EVIDENCE_INVALID`).
- Se corrigió el contrato a `DOC81-E2E-01..06` y códigos `ASSERTION_*`, con una prueba que construye la evidencia saneada completa. No se repite la mutación sobre 220586 porque el recurso ya fue consumido. El gate terminó en `false`, con usuarios y grupos vacíos.
- La primera corrida sobre 220587 aisló `INTENT_PERSISTENCE_ITEM_VALUE_LENGTH`: la descripción visible del anexo se enviaba como `file_name`. ENLASE ahora genera en servidor un nombre técnico corto y determinista; el preflight valida los límites persistibles restantes sin truncar valores.
- La corrida `import-sii-enlase-execution` sobre 220587 terminó correctamente con `sampleSize=1`: 7 controles verificados, 6 aserciones DOC-81 aprobadas, una evidencia física confirmada y recurso marcado `consumed`. Es evidencia de un solo anexo, no de RQ-01 multidocumento.
- Con autorización expresa para reutilizar la misma tarea, se preservó la reserva consumida anterior y se ejecutó `sampleSize=2`. La corrida terminó correctamente con una intención idempotente, una ejecución efectiva y evidencia física `2/2`; ausencia de efectos de expediente `0/0`, tarea sin transición y gate restaurado a `false` con alcance vacío.

## Cobertura automatizada

La matriz cubre selección/tipología, huella, concurrencia, referencia confiable, descarga segura, llamada legacy única, respuesta perdida, evidencia completa, archivo ausente, relación duplicada, agregado parcial, filtro por elemento, invariancia de constancias/ASMX y ausencia de asignación/cierre. Las pruebas estáticas de contrato complementan, pero no sustituyen, una E2E autorizada.

## Seguridad

- Autoridad reconstruida desde sesión/repositorios; no desde `localStorage`, hidden fields o payload libre.
- Gate versionado permanece `false` con usuarios/grupos vacíos al terminar pruebas.
- URLs temporales no se persisten como identidad.
- Descarga con allowlist, límites y validación de contenido compartida.
- SQL de valores parametrizado; nombre de gabinete validado por allowlist sintáctica antes de interpolarlo como identificador.
- Errores públicos saneados; evidencia no confirmada oculta el identificador documental.
- Reconciliación y controles E2E usan únicamente `SELECT`.

## Rollback

1. Desactivar la disponibilidad de la capacidad moderna mediante el gate, sin tocar la ruta legacy.
2. Retirar de `Compose` el adaptador/paso ENLASE y conservar el proveedor de lectura si se requiere.
3. No reintentar automáticamente intenciones inciertas; reconciliarlas antes de cualquier nueva ejecución.
4. Ejecutar `Sql/002-rollback-intent-capability.sql` solo si las columnas no contienen datos que deban conservarse y después de respaldo/aprobación DBA.
5. No eliminar documentos ya confirmados ni revertir asignación/estado de tarea: DOC-81 no los modifica.
6. Confirmar gate `false`, usuarios/grupos vacíos y ejecutar regresión de constancias.

## Pendientes de validación externa

- Verificar por `information_schema` el despliegue de `capability/provider_reference` en cada ambiente adicional. `QueryItems` clasifica MySQL 1054 como `IMPORT_SCHEMA_MIGRATION_REQUIRED`.
- Diagnóstico ODBC saneado con tabla calificada `DocuArchi.ra_ser_intento_serviciointegracion`: `ANNEX_SUCCESS`; no se consultaron cuerpos, secretos ni correlaciones.
- Verificación operativa de permisos físicos sobre todos los gabinetes productivos. No se declara cobertura completa sobre ambientes no inspeccionados.
