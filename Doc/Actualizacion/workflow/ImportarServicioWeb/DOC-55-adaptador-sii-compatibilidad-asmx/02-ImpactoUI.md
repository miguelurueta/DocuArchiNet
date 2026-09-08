# Impacto UI

- Ticket: DOC-55
- Cambio OpenSpec: doc-55-adaptador-sii-asmx
- Clasificacion: cross_cutting

## Superficies UI

DOC-55 no modifica controles, páginas ni JavaScript vigentes. `JSProgresBar.js` continúa consumiendo las firmas históricas y no interpreta DTO modernos.

La integración moderna se publica en paralelo en `WebServiceImportarServicioWebModern.asmx`. Con el gate apagado responde `FEATURE_DISABLED`; no redirige ni reemplaza `WebService_integracion_sii` o `WebServiceGaExpediente`.

## Endpoints modernos

| Método | Resultado | Efectos |
| --- | --- | --- |
| `ResolveCapabilities` | Capacidades comunes del proveedor | Ninguno |
| `QueryItems` | Lista paginada de elementos externos | Ninguno |
| `GetPreview` | Descriptor temporal saneado | Ninguno; no descarga ni escribe |

## Validacion visual

No aplica una validación visual: DOC-55 no cambia HTML, CSS, controles WebForms ni JavaScript. Se verificó estructuralmente que el JavaScript legacy permanece fuera del diff.
