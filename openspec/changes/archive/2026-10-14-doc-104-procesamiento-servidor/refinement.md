<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-104-procesamiento-servidor

## Fuente y alcance

- Ticket: `DOC-104` — PROCESAMIENTO-SERVIDOR.
- Cambio OpenSpec: `doc-104-procesamiento-servidor`.
- Fuentes normativas: Jira, Prompt 06 y contratos implementados por DOC-102 y DOC-103.
- Perfil tecnológico confirmado: ASP.NET WebForms sobre .NET Framework 4.6.1, VB.NET, ASMX, iTextSharp, System.Drawing, PDF.js y filesystem legacy.
- Alcance aprobado: pegado servidor del grafo, temporales, transparencia, transacción del archivo, refresco inmediato y clasificación posterior. No cambia almacenamiento, esquema ni biblioteca PDF.

## Contexto inspeccionado

- `webservice/WebService_itext.asmx.vb:84-153`: `service_anotate_pdf_image(...)` consulta certificado, llama iText, registra rutas en `datos_add_grafo` y retorna excepciones crudas.
- `itextshare/Class_ItexShare.vb:129-289`: `add_anotate_image_pdf(...)` usa nombres por usuario, recursos con cierres parciales, tamaño efectivo 90 x 90, `File.Copy(..., True)` y extracción adicional de página.
- `itextshare/Class_ItexShare.vb:290-345`: `Solicita_ruta_imagen(...)` resuelve la entrada legacy y `Create_file_pagina(...)` crea el archivo compartido `{usuario}.pdf`.
- `workflow/Class_transparent.vb`: `TransparentAsync(...)` es síncrono; los ciclos descendentes carecen de `Step -1` y el diseño `GetPixel/SetPixel` no es apto para rendimiento.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1016-1023`: `_updatePage(...)` fuerza `stream` y espera render/caché, pero siempre solicita la página 1 del PDF temporal.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1413-1488,1596-1618`: DOC-103 implementa single-flight cliente, un POST, normalización ASMX y conservación del overlay hasta refresco.
- `Infrastructure/Documentos/Streaming/EstrategiaCargaPdf.vb`: DOC-102 clasifica de forma conservadora por tamaño y `/Linearized`, con fallback `stream`.
- `Web.config:28,65`: los límites HTTP globales son mucho mayores que una imagen de grafo; DOC-104 aplica un límite funcional de 10 MiB decodificados y 4096 x 4096.
- `tools/e2e/AGENT-RUNBOOK.md`: toda medición autenticada/mutadora exige autorización explícita, datos descartables, secretos efímeros y consultas de control solo `SELECT`.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Conservar ASMX/DTO y ejecutar validación más certificado antes de cualquier escritura; errores públicos cerrados. | `WebService_itext.asmx.vb:84-121` | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Validar tipos, dominios, página, base64 <= 10 MiB e imagen <= 4096 x 4096 antes del stamper. | DTO con campos `Object`; ausencia de límites funcionales | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Usar GUID por operación, lock por documento y limpieza exclusiva; mismo documento no se procesa simultáneamente. | nombres compartidos en `Class_ItexShare.vb:145,158,160,317` | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Sustituir la falsa asincronía y el recorrido por píxel por `LockBits`, preservando tolerancia estricta y alfa. | `Class_transparent.vb` completo | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Preservar cálculo legacy y tamaño efectivo 90 x 90; cerrar todos los recursos en cada salida. | `Class_ItexShare.vb:179-278` | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Escribir staged adyacente, validarlo, ejecutar `File.Replace` con backup y restaurar ante validación posterior fallida. | copia destructiva en `Class_ItexShare.vb:283` | D-06 | RQ-06 | Origen: D-06, RQ-06 |
| D-07 | Retirar `Create_file_pagina` del guardado y refrescar URL vigente con `getPage(i)` en modo stream. | backend `:279-282`; frontend `_updatePage` | D-07 | RQ-07 | Origen: D-07, RQ-07 |
| D-08 | Clasificar el PDF realmente persistido después del reemplazo, sin linearizar ni acoplar guardado y transporte. | `EstrategiaCargaPdf.Seleccionar(...)` | D-08 | RQ-08 | Origen: D-08, RQ-08 |
| D-09 | Eliminar rutas/base64/excepciones de respuesta y auditoría; conservar commit con advertencia de log. | `datos_add_grafo`; contrato DOC-103 | D-09 | RQ-09 | Origen: D-09, RQ-09 |
| D-10 | Tratar caracterización, código, pruebas, MSBuild, E2E autorizada y evidencia saneada como una entrega. | `tools/e2e`, AGENT-RUNBOOK y evidencia DOC-102/103 | D-10 | RQ-10 | Origen: D-10, RQ-10 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Ningún archivo se crea antes de validar payload y certificado. | Payload inválido o documento firmado conserva hash/fecha y no deja artefactos. | Preserva frontera y protección legal actuales. |
| RQ-02 | Solo datos tipados y acotados alcanzan iTextSharp. | Rotación fuera de 0/90/180/270, página inválida, base64 > 10 MiB o imagen > 4096 falla cerrada. | El límite funcional es más estricto que Web.config para evitar presión de memoria. |
| RQ-03 | Operaciones no comparten temporales ni pierden actualizaciones silenciosamente. | Igual documento produce conflicto controlado; documentos diferentes progresan con rutas disjuntas. | El lock requiere filesystem compartido con semántica de exclusión. |
| RQ-04 | La transparencia modifica alfa según tolerancia sin acceso píxel por píxel administrado. | Matriz RGBA confirma coincidencias/no coincidencias y ausencia de `GetPixel/SetPixel`. | Corrige un defecto observable; requiere aprobación visual de fixtures. |
| RQ-05 | El grafo conserva ubicación aprobada y tamaño 90 x 90; no quedan handles. | Rotaciones y fallos permiten comparar salida y manipular archivos inmediatamente. | El ancho/alto enviados siguen sin controlar el tamaño final por compatibilidad. |
| RQ-06 | El original solo cambia después de validar staged y puede recuperarse. | Fallos por etapa preservan/restauran huella; éxito deja PDF reabrible con igual número de páginas. | Proveedores sin `File.Replace` fallan sin mutar. |
| RQ-07 | El cliente refresca la página real sin PDF público de una página. | Guardar en página 7 llama `getPage(7)`, conserva navegación y no crea `{usuario}.pdf`. | El refresco completo stream puede transferir más bytes; se medirá. |
| RQ-08 | El modo posterior refleja la estructura final. | Pérdida de `/Linearized` selecciona `stream`; conservación válida puede seleccionar `range`. | No garantiza descarga parcial ni introduce linealización productiva. |
| RQ-09 | Respuestas y logs no contienen información sensible. | Matriz de errores rechaza rutas, URL, base64, secretos y excepciones; fallo de log no duplica el pegado. | Mantiene semántica `committed_with_warning` de DOC-103. |
| RQ-10 | La entrega contiene evidencia real y reproducible. | Pruebas, MSBuild, OpenSpec y E2E autorizada pasan con PDF descartable y controles `SELECT`. | Sin autorización se registra bloqueo y no se inventan resultados. |

## Reglas de compatibilidad aprobadas

1. El endpoint, el arreglo `Class_itext_anotate` y la colección `service_itex` se mantienen.
2. El tamaño efectivo 90 x 90 se conserva hasta una Jira funcional independiente.
3. La pérdida de `/Linearized` es válida si el PDF final abre y se clasifica `stream`.
4. No se agregan tablas, columnas, colas, almacenamiento externo, Ghostscript ni nueva biblioteca PDF.
5. No se modifican PDFs reales no descartables durante pruebas.
6. La experiencia moderna sigue siendo oficial y no se reintroducen mecanismos legacy de activación.

## Resultado del refinamiento

- Estado: aprobado por el usuario el 2026-10-14.
- Design, spec y tasks contienen trazabilidad completa D-01..D-10 / RQ-01..RQ-10.
- La implementación debe comenzar por caracterización y pruebas de fallo antes de modificar el camino productivo.
