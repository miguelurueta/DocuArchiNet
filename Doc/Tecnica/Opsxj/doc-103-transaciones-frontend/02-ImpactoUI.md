# TRANSACIONES-FRONTEND

- Ticket: DOC-103
- Cambio OpenSpec: doc-103-transaciones-frontend
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

Solo cambia el overlay de grafo del visor PDF. Al confirmar el guardado, los enlaces Guardar y Cerrar reciben `aria-disabled="true"` y se bloquean localmente; otros overlays, scroll, zoom y navegación continúan disponibles. En `finally` recuperan su estado si aún existen.

El overlay se conserva ante validación incompleta, fallo de imagen, rechazo funcional, error HTTP/parser, timeout, aborto o fallo de refresco. Solo se retira tras commit y refresco resuelto. Un commit con fallo posterior de auditoría sí lo retira para evitar duplicar el grafo y muestra una advertencia fija.

## Mensajes

Todos los fallos pasan por `PdfViwerAlert` con textos públicos. No se muestran `responseText`, excepción, URL, ruta, payload, identidad ni detalle de auditoría.

## Validacion visual

El arnés comprobó bloqueo, rehabilitación, retiro/conservación del overlay y preservación de página, zoom y scroll. La E2E real autorizada sobre PDF descartable confirmó un POST, commit, retiro del overlay y preservación de página, zoom y scroll.
