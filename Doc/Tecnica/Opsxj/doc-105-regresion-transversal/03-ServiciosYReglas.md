# DOC-105 — Servicios y reglas

- Ticket: DOC-105
- Cambio OpenSpec: doc-105-regresion-transversal
- Clasificacion: cross_cutting

Cambio: `doc-105-regresion-transversal`. Impacto: `cross_cutting`.

## Servicios y reglas

Se incorpora preparación temporal de PDF y geometría afín del grafo, preservando original y contratos de negocio. La preparación se configura globalmente, no por usuarios/grupos; fallback al original ante ausencia/error. La prioridad del preview no habilita herramientas antes del render principal. Impresión conserva su servicio y espera callback de cierre de PrintJS, una heurística de foco, no acreditación física.

### Componentes observados

- `EstrategiaCargaPdf`: selecciona `RANGE` para PDF grande y linealizado; `STREAM` en los demás casos.
- `Handler_image_wf.ashx`: entrega autorizada con `200` o `206`, validadores y caché privada.
- Constructores enriquecidos de Docuarchi, Workflow y Producción.
- PDF.js: configura rango/stream y renderiza el primer canvas.
- Guardado DOC-103/DOC-104: single-flight, un POST y procesamiento transaccional.

## Reglas

- Parcial requiere `206`, `Content-Range` válido, total conocido y unión previa al canvas menor al total.
- Rangos solapados cuentan una vez como bytes únicos y todas las transferencias como bytes físicos.
- Cualquier consumidor obligatorio bloqueado impide declarar cobertura total.
- Solo Workflow puede ejecutar una mutación y debe producir exactamente un POST.
- Controles de base de datos: únicamente `SELECT` sobre `workflowdocument` con cuenta de lectura.
- Correspondencia conserva `anula_visor_espress = 1`: no requiere `UTIL_VISOR_EXPRESS` para esta ventana y su aceptación se basa en el handler autorizado, no en un canvas PDF.js.
