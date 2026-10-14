# Prompt 04 — Streaming, HTTP Range y caché autorizada del PDF

## Contexto de ejecución OPSXJ

Este prompt corresponde a una Jira independiente. Ejecuta `opsxj:new` al iniciar para crear su OpenSpec autónomo. Verifica primero el visor y las métricas vigentes después de los prompts anteriores, sin reutilizar sus cambios OpenSpec.

## Rol esperado

Actúa como arquitecto full stack senior especializado en HTTP, ASP.NET handlers, streaming, PDF.js, seguridad documental y compatibilidad WebForms.

## Fuentes obligatorias

- Exploración técnica completa.
- `workflow/Handler_image_wf.ashx` y `workflow/Handler_image_wf.ashx.vb`.
- `ClassRaEnvioCorrespondencia.ReadFile(...)` y sus efectos.
- `pdfjs/pdf_anotate_view/script.js` y `pdf_viewer_anotate.js`.
- Constructores de URL en `Docuarchi/ClassVisualisaDocumento.vb`, `workflow/ClassWorflowVisor.vb` y `Gestion/ClassGaProducionDocumental.vb`.
- Consumidores simples del handler inventariados en la exploración.

## Objetivo

Permitir que PDF.js consuma una URL autorizada sin la materialización cliente completa `fetch -> Blob -> FileReader -> Uint8Array` y seleccione de forma adaptativa entre `Range` y streaming completo. Usar rangos únicamente cuando la estructura y el tamaño del PDF permitan una descarga parcial efectiva; para documentos pequeños, no linealizados o indeterminados, priorizar un único stream `200` que evite solicitudes solapadas y mantenga rápida la primera visualización.

## Diseño obligatorio

Antes de implementar, documenta:

- autoridad para resolver el documento y evitar confiar en una ruta libre del navegador;
- contrato compatible para consumidores enriquecidos y simples;
- semántica de `GET`, `HEAD`, `Range`, `If-Range`, `ETag`/`Last-Modified`, `206`, `200`, `304` y `416`;
- criterio determinista para clasificar el documento como `RANGE` o `STREAM`, considerando como mínimo tamaño y presencia real del diccionario `/Linearized` al inicio del PDF;
- diferencia entre bytes únicos del documento alcanzados hasta el primer canvas y bytes físicos transferidos por HTTP, incluidos encabezados y rangos solapados;
- límites, tipos MIME, disposición, errores públicos y cierre de streams;
- efecto de `IRequiresSessionState` sobre concurrencia y alternativa compatible si el handler no escribe sesión.

## Implementación obligatoria

1. Servir el archivo mediante `FileStream`/streaming; no leer el PDF completo en `Byte()` ni usar `BinaryWrite` del cuerpo completo.
2. Implementar un único rango válido por solicitud con validación de límites, `Content-Range`, `Content-Length`, `Accept-Ranges: bytes` y `206`, sin convertir `Range` en la política obligatoria para todos los PDF.
3. Responder correctamente a rango inválido con `416` sin filtrar ruta.
4. Definir validadores de caché compatibles con autorización; nunca permitir reutilización entre usuarios o documentos no autorizados.
5. Hacer que PDF.js reciba una URL y gestione streaming/rangos directamente.
6. Eliminar del camino enriquecido `fetch`, `Blob`, `FileReader` y copia a `Uint8Array`.
7. Mantener adaptador temporal para consumidores simples y formatos no PDF.
8. No aceptar una ruta física nueva como autoridad. Si el contrato actual impide resolver el documento de forma segura, registrar la deuda y diseñar una referencia opaca/servidor sin inventar tablas.
9. Aplicar una política adaptativa y explícita: `RANGE` para PDF grandes y realmente linealizados; `STREAM` para PDF pequeños, no linealizados o cuya estructura no permita garantizar acceso incremental eficiente. El fallback nunca debe volver a materializar el archivo completo en JavaScript.
10. Mantener el almacenamiento actual. Cualquier prueba de Fast Web View/linealización debe ser una POC local, efímera y descartable, fuera del camino productivo y sin sobrescribir el documento fuente.

## Restricciones y antirregresión

- No agregar tablas, columnas, almacenamiento externo o CDN.
- No debilitar autenticación o autorización para habilitar rangos.
- No devolver rutas físicas en encabezados, errores o logs.
- No habilitar caché pública de documentos privados.
- No cambiar guardado de grafos en esta tarea.
- No incorporar Ghostscript, otro ejecutable externo ni una etapa de linealización en producción.
- No afirmar que `206` garantiza por sí solo menos bytes físicos o menor latencia: PDF.js puede emitir rangos solapados y algunos PDF necesitan gran parte del archivo antes del primer canvas.
- Preservar Workflow, Gestión de Correspondencia, Producción Documental, aprobación, versiones, migración y visores externos inventariados.

## Pruebas obligatorias

- Archivo completo, rango inicial, intermedio, sufijo, límite exacto, fuera de límite y header múltiple rechazado/controlado.
- `Content-Length`, `Content-Range`, MIME, `HEAD`, validadores y cierre ante desconexión.
- Acceso sin sesión, documento ajeno, referencia inválida y traversal.
- Regresión de consumidores simples.
- E2E local autorizada que demuestre primer canvas mediante la política seleccionada, sin exigir descarga parcial cuando el PDF no sea linealizado.
- Caso grande y linealizado que seleccione `RANGE` y demuestre cobertura única parcial antes del primer canvas; caso pequeño o no linealizado que seleccione `STREAM`, realice una sola respuesta `200` y no emita rangos redundantes.
- POC descartable que compare el mismo PDF antes/después de Fast Web View sin modificar almacenamiento: páginas, primer canvas y resultado visual equivalentes, temporal eliminado y fuente intacta.
- Medición remota o con perfil de red controlado; registrar por separado tiempo a primera página, bytes únicos del PDF, bytes físicos transferidos, cantidad de solicitudes y modo `RANGE`/`STREAM`.

## Criterios de aceptación

- PDF.js comienza con URL y no necesita `Blob`, `FileReader` ni copia completa a `Uint8Array` antes de inicializar.
- El handler no materializa el PDF completo en memoria.
- Las respuestas HTTP cumplen rangos y seguridad.
- La selección `RANGE`/`STREAM` refleja la estructura real del archivo: la descarga parcial solo se declara lograda cuando la evidencia prueba menor cobertura única antes del primer canvas; en archivos no linealizados se acepta `STREAM` si evita sobrecosto y mantiene o mejora la latencia.
- La POC de linealización informa beneficios y costos sin convertir el cambio de almacenamiento ni el ejecutable externo en dependencia productiva.
- Los módulos consumidores mantienen su comportamiento.

## Documentación y entrega

Documenta contrato HTTP, amenazas, compatibilidad, pruebas y métricas en `Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/`.

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
Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.

## Pruebas obligatorias
Ejecutar pruebas Node focales del contrato HTTP, E2E con Playwright y MSBuild del proyecto Web Forms afectado; registrar comandos y resultados.

## Documentacion tecnica
Actualizar el paquete documental canonico del ticket.

## Entregable final
Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.

Agregar regla para [FLOW_DETAIL_REQUIRED]: Flujo paso a paso, secuencia o comportamiento esperado.

Exigir MSBuild del proyecto Web Forms, pruebas Node focales del handler y validación E2E autorizada; registrar el resultado.

Registrar comandos ejecutados, resultados obtenidos y evidencia en `05-PruebasEvidencia.md`.

Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o `.env` paralelos.

Antes de autenticar, exigir lectura de `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.

Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo `SELECT` y conservar evidencia saneada.

Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.

Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.
