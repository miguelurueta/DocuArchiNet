# DOC-115 — ImpactoUI

- Ticket: DOC-115
- Cambio OpenSpec: doc-115-produccion-importacion-carga-carpeta-lotes
- Clasificacion: cross_cutting

Estado: implementación y validación completas, entrega pendiente de revisión/publicación.

## Superficies UI

No hay cambio visual ni nueva ventana/árbol. Se reutiliza la sesión Producción y la fachada ASMX/ASHX existente. Los cambios JavaScript pertenecen exclusivamente a pruebas y utilidades.

## Validacion visual

No corresponde QA visual nueva por este delta sin UI. RECIB-01/02/03 ejecutaron el recorrido autenticado HTTP con controles SELECT. La evidencia histórica UI de DOC-113 se conserva como antecedente sin atribuirla a una nueva ejecución.
