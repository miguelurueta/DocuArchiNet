# DOC-102 — streamer HTTP por rangos

- Ticket: DOC-102
- Cambio OpenSpec: doc-102-streamer-http-rangos
- Clasificacion: cross_cutting
- Estado técnico: estrategia adaptativa implementada, compilada y validada mediante E2E autenticada (`PASSED`, mediana 624 ms)

## Objetivo

Reducir el tiempo y los bytes necesarios para mostrar la primera página del PDF. El visor ya no descarga el documento completo mediante `fetch`/`Blob`: entrega una URL autorizada a PDF.js, que puede solicitar los intervalos que necesita mediante HTTP Range.

## Alcance y compatibilidad

- Referencia opaca `doc_ref`, protegida con `MachineKey`, ligada a la sesión y con vencimiento de cinco minutos.
- `GET`/`HEAD`, rangos simples, validadores y respuestas `200`, `206`, `304` y `416`.
- Copia desde `FileStream` con buffer fijo de 64 KiB y sesión de solo lectura.
- Migración de los tres emisores enriquecidos de Workflow y Producción Documental.
- Adaptador `rut_image` conservado para consumidores simples, siempre con cuerpo completo y sin contrato Range.
- Selector de transporte de solo lectura: rangos para PDF grandes linearizados y streaming secuencial para PDF pequeños/no linearizados; refresco iText/grafo siempre compatible.
- Evidencia E2E saneada: estados, conteos Range, encabezados booleanos y bytes agregados; nunca URL, token, cookie o ruta.
- Prueba local descartable de Fast Web View: reduce cobertura única de primera página de 28 % a 9,12 % sobre fixture sintético, sin modificar fuente ni almacenamiento; no se integra Ghostscript al runtime.

## Compatibilidad y límite

Se preservan `urimage_format`, permisos de impresión/descarga/firma/sello y acciones existentes. DOC-102 no cambia tareas, documentos, auditoría ni esquema de base de datos. La deuda de autorización del adaptador legacy no se amplía y queda fuera del contrato nuevo.

La prueba de concepto no autoriza linearización productiva. Sus límites pendientes incluyen licencia y despliegue de la herramienta, cifrado, firmas, formularios, anotaciones, equivalencia integral y la posible pérdida de Fast Web View tras una reescritura iText.
