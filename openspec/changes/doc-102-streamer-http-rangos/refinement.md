<!-- opsxj:refinement version=1 state=approved -->

# Refinamiento - doc-102-streamer-http-rangos

## Fuente y alcance

- Ticket: `DOC-102` — STREAMER-HTTP-RANGOS
- Cambio OpenSpec: `doc-102-streamer-http-rangos`
- Fuente Jira: `specs/streamer-http-rangos/jira-context.md`
- Perfil tecnológico: ASP.NET WebForms/VB.NET 4.6.1, handler ASHX, PDF.js y pruebas Node/Playwright existentes.

El alcance sustituye la apertura enriquecida basada en ruta y materialización completa por una URL autorizada con streaming/rangos. Conserva temporalmente el adaptador `rut_image` para consumidores simples y no cambia persistencia, permisos funcionales ni guardado de grafos.

## Contexto inspeccionado

- `workflow/Handler_image_wf.ashx.vb`: acepta `rut_image`, implementa `IRequiresSessionState`, llama a `ReadFile`, usa `BinaryWrite` para PDF, `WriteFile/Response.End` para otros formatos y devuelve `ex.Message`.
- `radicador/ClassRaEnvioCorrespondencia.vb:1137`: `ReadFile` cambia atributos, reserva el archivo completo en `Byte()` y lo lee antes de cerrar el stream.
- `pdfjs/pdf_anotate_view/script.js:168-220`: ejecuta `fetch`, `Blob`, `FileReader` y `Uint8Array` antes de inicializar el visor.
- `pdfjs/pdf_anotate_view/pdf_viewer_anotate.js:1217-1241`: `loadDocument` ya entrega su argumento a `pdfjsLib.getDocument`, por lo que admite URL.
- `Docuarchi/ClassVisualisaDocumento.vb`, `workflow/ClassWorflowVisor.vb` y `Gestion/ClassGaProducionDocumental.vb`: construyen URLs enriquecidas con ruta física y metadatos/permisos del documento.
- Consumidores simples inventariados en `Docuarchi/WebForm*Visor*.aspx.vb`, `Gestion/Class_ra_ver_version_documento.vb`, `Gestion/WebFormGaVisorDocumentoExterno.aspx.vb` y `workflow/WebFormVisor_Otros_Formatos.aspx.vb`.
- `Web.config` y `GestionDocumental-Docuarchi.net.vbproj`: aplicación .NET Framework 4.6.1; el repositorio ya usa `MachineKey.Protect/Unprotect` e `IReadOnlySessionState`.
- `tools/e2e`: runner y pruebas de política/rendimiento del visor creados en DOC-100/DOC-101; deben extenderse, no duplicarse.

## Decisiones aprobadas

| ID | Decisión verificable | Evidencia de código | Design | Requirement | Tasks |
| --- | --- | --- | --- | --- | --- |
| D-01 | Emitir `doc_ref` protegido, expirable y ligado a sesión; rechazar alteración, expiración o sesión distinta sin filtrar ruta. | `MachineKey` ya usado bajo `Infrastructure/`; constructores resuelven la ruta en servidor. | D-01 | RQ-01 | 1.1-1.6, 2.5, 4.1, 5.2-5.3 |
| D-02 | Implementar `GET`/`HEAD` y un rango cerrado, abierto o sufijo; responder `416` a sintaxis/límites inválidos o múltiples. | `Handler_image_wf.ashx.vb`; soporte Range de PDF.js en `pdfjs/build/pdf.js`. | D-02 | RQ-02 | 2.1-2.2, 2.5-2.6, 4.2, 5.2-5.3 |
| D-03 | Usar caché privada revalidada, ETag opaco, Last-Modified y reglas explícitas de `If-None-Match`, `If-Modified-Since` e `If-Range`. | El handler actual no configura caché ni validadores. | D-03 | RQ-03 | 2.3-2.4, 2.6, 4.3, 5.2-5.3 |
| D-04 | Transmitir con `FileStream` y buffer fijo, longitud exacta y cierre determinista; retirar `ReadFile/BinaryWrite` del handler. | Handler y `ClassRaEnvioCorrespondencia.ReadFile`. | D-04 | RQ-04 | 2.7-2.8, 4.4, 5.2-5.3 |
| D-05 | Cambiar a `IReadOnlySessionState` y prohibir escrituras/efectos persistentes desde el streamer. | El handler actual solo lee datos de solicitud pero adquiere sesión exclusiva. | D-05 | RQ-05 | 2.9, 4.5, 4.7, 5.2-5.3 |
| D-06 | Pasar URL autorizada directamente a `pdfjsLib.getDocument`; conservar ruta formateada solo para acciones legacy. | `script.js:168-220`; `loadDocument` ya acepta URL. | D-06 | RQ-06 | 3.1-3.3, 4.5, 4.7, 5.3 |
| D-07 | Mantener `rut_image` como adaptador simple sin Range/caché nueva y transmitirlo por bloques; impedir nuevos usos enriquecidos. | Inventario de referencias a `Handler_image_wf.ashx?rut_image`. | D-07 | RQ-07 | 2.10, 3.4, 4.5, 5.1, 5.3 |
| D-08 | Integrar pruebas HTTP, políticas, MSBuild y E2E autorizada con bytes/primer canvas en la infraestructura existente. | `tools/e2e` y contrato de Jira/AGENTS. | D-08 | RQ-08 | 4.1-4.7, 5.1-5.4 |

## Requisitos verificables

| ID | Resultado observable | Escenario o criterio de aceptación | Riesgo/compatibilidad |
| --- | --- | --- | --- |
| RQ-01 | Solo una referencia protegida emitida para la sesión abre el contrato nuevo. | Válida abre; alterada, expirada o ajena falla cerrada sin ruta. | Afinidad/MachineKey entre nodos; no registrar secretos. |
| RQ-02 | `200`, `206`, `HEAD` y `416` tienen encabezados, límites y cuerpos correctos. | Completo, inicial, intermedio, abierto, sufijo, exacto, fuera de límite y múltiple. | Off-by-one y archivos vacíos/grandes. |
| RQ-03 | Revalidación privada cumple precedencia y no autoriza otra sesión. | `304`, `If-Range` coincidente y degradación a `200`. | Proxy/caché compartida; requiere `private` y `Vary: Cookie`. |
| RQ-04 | Memoria de copia es acotada y los streams siempre se cierran. | `Content-Length` coincide; desconexión no fuga handle ni excepción. | Excepciones tardías cuando ya se enviaron encabezados. |
| RQ-05 | Rangos de una sesión progresan sin bloqueo exclusivo ni mutaciones. | Concurrencia y controles `SELECT` estables. | Código incidental que escriba sesión volvería inválido `IReadOnlySessionState`. |
| RQ-06 | Primer canvas inicia desde URL sin precarga completa. | Ausencia de fetch/blob/reader/copia y acciones del visor preservadas. | Los servicios de edición aún requieren `urimage_format`. |
| RQ-07 | Consumidores simples conservan comportamiento y quedan separados del contrato nuevo. | MIME/disposición/cuerpo equivalentes; Range legacy no habilitado. | Deuda de ruta física existente requiere ticket posterior. |
| RQ-08 | Evidencia reproducible demuestra contrato, seguridad, build y mejora real. | Pruebas focales, MSBuild, OpenSpec y E2E autorizada saneada. | Sin autorización se registra bloqueo, nunca evidencia ficticia. |

## Reglas de trazabilidad obligatorias

1. Cada decisión `D-XX` debe permanecer desarrollada en `design.md`, reflejada en `spec.md` y vinculada a tareas mediante `Origen: D-XX, RQ-XX`.
2. Las tareas deben nombrar área/archivos, complejidad y una verificación primaria reproducible.
3. Archivos VB.NET nuevos deben registrarse en `GestionDocumental-Docuarchi.net.vbproj`.
4. La E2E real exige autorización explícita, lectura previa del runbook, secretos efímeros, evidencia saneada y controles únicamente `SELECT`.
5. El mapa aprobado conserva cobertura explícita D-01..D-08 y RQ-01..RQ-08; cualquier cambio de alcance exige actualizar diseño, spec y tareas conjuntamente.

## Resultado del refinamiento

- Estado: aprobado con diseño, requisitos y 31 tareas atómicas trazadas D-01..D-08 / RQ-01..RQ-08.
- Próximo paso: ejecutar `opsxj:refine -- DOC-102 --sync`, revisión técnica y luego implementación en orden de dependencias.
