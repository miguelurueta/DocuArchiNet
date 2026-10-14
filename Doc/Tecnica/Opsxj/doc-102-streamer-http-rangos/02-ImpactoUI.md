# DOC-102 — impacto en el visor

## Recorrido de apertura

```text
[Usuario selecciona documento]
              |
              v
[WebForms autoriza y emite doc_ref]
              |
              v
[iframe PDF.js recibe ash + doc_ref]
              |
              v
       ¿Parámetros válidos?
          | sí        | no
          v           v
[getDocument(URL)] [Error localizado; no solicita archivo]
          |
          v
[Servidor recomienda Range o streaming]
          |
          v
[PDF.js resuelve los objetos requeridos]
          |
          v
[Primer canvas visible]
          |
          +------> scroll/zoom/navegación reutilizan bytes o piden rangos adicionales
```

No se precarga el PDF completo mediante código JavaScript propio. Un PDF grande linearizado usa rangos; un PDF no linearizado o pequeño usa una única respuesta secuencial para evitar decenas de rangos que finalmente sumarían el archivo completo. Scroll, zoom y navegación reutilizan los bytes disponibles. Los botones de guardar, imprimir, descargar, firma y sello siguen usando `urimage_format`; no se cambió su contrato. El pegado de grafo conserva el servicio iText y refresca solo la página modificada por streaming completo.

## Superficies UI

- `pdfjs/pdf_anotate_view/script.js`: construye la URL autorizada y controla errores.
- `pdf_viewer_anotate.js`: conserva por separado URL de lectura y URL de acciones.
- Emisores enriquecidos: `ClassVisualisaDocumento`, `ClassWorflowVisor` y `ClassGaProducionDocumental`.

## Validacion visual

No se cambiaron DOM, foco, estilos, responsive, accesibilidad ni controles de navegación. La E2E autenticada adaptativa confirmó selección, tres primeros canvas, scroll y zoom con modo `stream` y resultado `PASSED`.
