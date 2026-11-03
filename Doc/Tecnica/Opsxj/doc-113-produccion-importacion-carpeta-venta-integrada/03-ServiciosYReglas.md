# DOC-113 — Ventana integrada

- Ticket: DOC-113
- Cambio OpenSpec: doc-113-produccion-importacion-carpeta-venta-integrada
- Clasificacion: cross_cutting (Transversal)

## Servicios y reglas

No se modifican servicios VB, DTO, repositorios o reglas de negocio. El controlador valida solo la estructura de los datos que renderiza; la autoridad/elegibilidad permanecen en servidor. El soporte UI limita operaciones: permite lecturas, dos preparaciones en UI-02 y confirmación únicamente con aprobación expresa y el plan vigente; bloquea recepción/avance/administración. UI-03 verifica rechazo de una petición de apertura con referencia inválida sin efectos. Los controles reutilizan consultas SELECT y snapshots existentes.

Evidencia y límites: [Informe de implementación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md).
