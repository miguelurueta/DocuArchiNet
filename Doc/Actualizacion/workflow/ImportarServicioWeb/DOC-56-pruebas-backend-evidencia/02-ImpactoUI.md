# Impacto UI

- Ticket: DOC-56
- Cambio OpenSpec: doc-56-pruebas-integracion
- Clasificacion: cross_cutting

## Alcance

DOC-56 valida el backend moderno de `ImportarServicioWeb`; no introduce una interfaz nueva ni modifica directamente páginas WebForms, controles, estilos o scripts del frontend.

La UI consumidora conserva el contrato v1 del servicio ASMX y debe seguir enviando el contexto de tarea, operación, correlación y proveedor requerido por cada operación. Los resultados parciales, errores tipados y estados de intención se presentan con la semántica documentada en [Contratos e integraciones](04-ContratosIntegracion.md).

## Comportamiento preservado

- La experiencia legacy continúa disponible mientras el gate moderno permanezca desactivado.
- La consulta y el preview no persisten intención, documento, estado ni auditoría.
- La ejecución moderna solo se habilita para usuarios y grupos autorizados por el gate.
- Los errores de proveedor, validación o reconciliación se devuelven como datos tipados; la UI no debe inferir éxito a partir de una respuesta HTTP aislada.

## Validación visual

No aplica una regresión visual específica para este cambio de backend. La validación E2E comprueba la interacción funcional desde la superficie consumidora y sus resultados saneados se registran en [`Evidencias/`](Evidencias/README.md).
