# INTEGRAR-CENTRO-TRABAJO-NOTA

- Ticket: DOC-43
- Cambio OpenSpec: doc-43-integrar-centro-trabajo-nota
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- [x] Superficie afectada: `workflow/Webworkflow.aspx(.vb)`, panel moderno de Notas, editor, lista y estilos encapsulados; el modal y GridView legacy permanecen sin retiro.
- [x] QA visual estática en Chromium para 375x812, 768x1024, 1024x768, 1440x900 y móvil horizontal 812x375; no se observó desplazamiento horizontal ni superposición en el modelo aprobado.

## Validacion visual

### Identificadores de referencia del modelo

La integración conservará la semántica visual del modelo mediante los identificadores `task-title`, `notes-heading`, `notes-count`, `sort-notes`, `new-note`, `notes-list`, `saved-message`, `editor-backdrop`, `editor-title`, `note-text`, `character-count`, `save-note` y `confirm-backdrop`. En Web Forms se usarán equivalentes con prefijo del consumidor para evitar colisiones; el contenido se renderizará con APIs seguras de texto.

Capturas saneadas: `.opsxj/evidence/doc43-qa-*.png`.
