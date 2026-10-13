# PLANIFICADOR-VIEWPORT

- Ticket: DOC-100
- Cambio OpenSpec: doc-100-planificador-viewport
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

DOC-100 no modifica servicios VB.NET/C#, handler, WebMethod ni persistencia. La regla cliente es máximo un trabajo efectivo por `pageNumber:generation`; el trabajador continúa tras fallo y una terminación obsoleta no actualiza el canvas. Los errores se transforman en códigos estables sin mensajes, stacks, URL, queries, rutas ni datos documentales. Solo un recurso opcional conocido permite `PASSED_WITH_OBSERVATIONS`.
