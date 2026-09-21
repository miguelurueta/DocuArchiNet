# DOC-67 — Creación y vinculación de expedientes SII

## Resultado arquitectónico

El flujo moderno conserva el agregado de inscripciones desde la respuesta SII autoritativa y ejecuta una saga persistente, secuencial e idempotente:

```text
CreateImportIntent
  → inscripción + items persistidos
ExecuteImportIntent
  → resolver/verificar o crear expediente
  → persistir plan lógico
  → almacenar cada item mediante el adaptador legacy conservado
  → consultar gabinete por ENLASE
  → persistir plan físico por IdImagen
  → precheck/vínculo/postcheck
  → caché documental verificada
  → NITCEDULA/RAZONSOCIAL/MATRICULA
  → verificar índice SQL y XML independientemente
  → reconciliar
  → Completada
```

La frontera productiva es `WebServiceImportarServicioWebModern.asmx.vb`. Los mutadores físicos complejos permanecen encapsulados en proceso detrás de puertos modernos; no se invocan ASMX legacy por HTTP interno y no se modificaron las funciones originales.

## Persistencia

- `workflow_import_inscription`: agregado y expediente por inscripción.
- `workflow_import_intent_item`: expediente y estados de almacenamiento, relación, índice y caché.
- `workflow_import_related_document`: diario de cada `IdImagen` descubierto por `ENLASE`.
- `workflow_import_document_link_cache`: unicidad por tarea, imagen y gabinete.

## Evidencia y trazabilidad

- Matriz individual de las 15 funciones: `Doc/Tecnica/Opsxj/doc-67-creacion-vinculacion-expediente/06-MatrizMigracionLegacy.md`.
- Línea base legacy: `Doc/Tecnica/Opsxj/doc-67-creacion-vinculacion-expediente/07-LineaBaseLegacy.md`.
- Evidencia local: `Doc/Tecnica/Opsxj/doc-67-creacion-vinculacion-expediente/05-PruebasEvidencia.md`.
- Flujo técnico end-to-end: [flujo-tecnico-end-to-end.md](flujo-tecnico-end-to-end.md).
- Despliegue y reversa: [deployment-and-rollback.md](deployment-and-rollback.md).
- Bitácora quirúrgica de fallos y correcciones E2E: [bitacora-diagnostico-e2e.md](bitacora-diagnostico-e2e.md).

## Riesgos residuales

- Los adaptadores de creación, vínculo e índices dependen temporalmente de contexto de sesión legacy; comparan tarea/ruta con el contexto persistido y fallan cerrados ante discrepancia.
- No existe transacción distribuida SQL/XML: una respuesta desconocida produce estado incierto y exige reconciliación.
- El gate debe habilitarse inicialmente solo para alcance controlado después de aplicar y validar las migraciones.
