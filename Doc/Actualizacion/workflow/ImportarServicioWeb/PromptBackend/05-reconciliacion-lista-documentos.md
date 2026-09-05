# Prompt backend 05 — Reconciliación y lista de documentos

Implementa la consulta autoritativa del resultado de una intención y de cada elemento ejecutado por el Prompt backend 04.

Publica `ReconcileImportIntent` y completa `GetImportIntent` con el `ImportItemResult` versionado definido en el contrato compartido.

## Objetivo

Resolver resultados inciertos y devolver al frontend únicamente documentos confirmados, autorizados y relacionados con la tarea original.

## Rutas canónicas de implementación

```txt
Services/Workflow/ImportarServicioWeb/
├── ServicioReconciliacionImportacion.vb
└── ImportItemResultMapper.vb

Infrastructure/Repositories/Workflow/ImportarServicioWeb/
└── MySqlImportReconciliationRepository.vb

Tests/
├── importar-servicio-web-reconciliation.test.cjs
├── importar-servicio-web-document-list-contract.test.cjs
└── importar-servicio-web-reconciliation-authorization.test.cjs

Tests/Fixtures/Workflow/ImportarServicioWeb/reconciliation-v1/
├── completed.json
├── partial.json
├── uncertain.json
├── wrong-task.json
└── duplicated-document.json
```

- Extender `ImportItemResult` solo en los DTO/modelos canónicos de Backend 01.
- Toda lectura MySQL reside en `MySqlImportReconciliationRepository.vb` y utiliza infraestructura de datos compartida parametrizada.
- El mapeo a lista documental pertenece al servicio/mapper moderno; `dato_lista` solo se maneja en el adaptador de compatibilidad de Backend 06.
- Agregar archivos `.vb` al `.vbproj` sin editar repositorios o endpoints actuales.

No implementar reconciliación dentro de `insert_row_documento_relacionado(...)`, ASMX legacy, `ClassAlmacenamiento` o cachés SII existentes.

## Ruta documental obligatoria

```txt
docs/Architecture/Workflow/ImportarServicioWeb/SCRUMCORE-000-reconciliacion-lista-documentos/
```

Sustituir `SCRUMCORE-000` por el ticket real. Crear el paquete `00-Indice.md` a `07-Metadata.md` y `Diagramas/`, incluyendo fuentes de verdad, joins/lecturas, autorización, duplicados, mapeo de estados y resultados inciertos.

## Implementa

- Consulta de reconciliación por intención y consulta focal por intención más identidad externa.
- Composición de intención, fase, tarea original, identidad externa, documento, relación documento-tarea, expediente, índices y caché aplicables.
- `ImportItemResult` estructurado y versionado con estado, código, mensaje seguro, fase alcanzada, conocimiento de persistencia, documento y correlación.
- Mapeo contractual probado entre cada estado/fase backend y su único estado visible frontend.
- Identificador documental interno y datos mínimos necesarios para refrescar la lista sin depender de `dato_lista` delimitado.
- Detección de relación faltante, duplicada, parcial o inconsistente.
- Autorización de lectura que valide propietario o alcance permitido sobre intención y tarea.
- Compatibilidad temporal para traducir resultados confirmados hacia `insert_row_documento_relacionado(...)`, confinada al adaptador legacy.

## Restricciones

- Implementar consultas nuevas en paralelo; no modificar la escritura vigente ni `AlmacenaDocumentoTareaWorkflow(...)`.
- No declarar importado un elemento solo porque el endpoint mutador respondió correctamente.
- No insertar un documento en una vista correspondiente a otra tarea.
- Timeout o ausencia de confirmación produce ResultadoIncierto o Verificando, nunca Disponible.
- No reconstruir autoridad a partir de datos enviados por el navegador.
- No devolver rutas físicas, secretos, metadatos internos innecesarios ni mensajes de excepción.

## Aceptación

- Cada documento confirmado aparece una sola vez y en la tarea correcta.
- Recarga o pérdida de respuesta permite reconstruir el estado desde persistencia.
- La reconciliación distingue completado, parcial, detenido, fallido e incierto.
- Una inconsistencia queda visible para soporte mediante correlación y no se oculta como éxito.
- Las pruebas cubren duplicados, tarea distinta, relación ausente y fallo entre fases.

## Trazabilidad

Exploración backend: secciones 13, 14, 17 y 19; decisión recomendada 7.

## Correcciones opsxj:prompt-review

Estas reglas fueron agregadas desde `opsxj:prompt-review` para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.

## Rol esperado
Definir el rol tecnico esperado para ejecutar el ticket.

## Objetivo
Describir el objetivo funcional y tecnico verificable.

## Restricciones criticas
- No introducir cambios fuera del alcance declarado.
- No romper comportamiento existente ni contratos publicos.

## Criterios de aceptacion
- El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.

## Contexto obligatorio
Leer contratos B01, intención B03, orquestación B04, `DTOs/Workflow/Terminar/TransicionWorkflowDtos.vb` y repositorios de lectura de `Infrastructure/Repositories/Workflow/` como convenciones. Inspeccionar `insert_row_documento_relacionado(...)` y `dato_lista` solo para compatibilidad; no modificarlos.

## Pruebas obligatorias
Ejecutar pruebas unitarias/focales, build/tsc segun impacto y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.

## Documentacion tecnica
Actualizar exclusivamente el paquete de **Ruta documental obligatoria**, con contrato de reconciliación, consultas parametrizadas, mapa de estados, diagramas y evidencia reproducible.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

## Requisitos positivos
- Implementar el comportamiento esperado con contratos tipados y responsabilidades claras.
- Mantener la integracion sobre los puntos de extension existentes del repo.
- Dejar evidencia de pruebas y documentacion tecnica actualizada.

Exigir `npm run build` o `tsc` segun impacto y registrar el resultado.
