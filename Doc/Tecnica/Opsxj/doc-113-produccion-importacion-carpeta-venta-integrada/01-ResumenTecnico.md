# DOC-113 — Ventana integrada

- Ticket: DOC-113
- Cambio OpenSpec: doc-113-produccion-importacion-carpeta-venta-integrada
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Corregir F-01 de respuestas no consumibles y F-02 de foco, reutilizando la ventana integrada existente. UI-01/02/03 se implementan como pruebas focales de sus componentes, sin crear otra ventana o backend.

## Alcance y compatibilidad

Producto modificado: solo el controlador JavaScript. Pruebas, selector y registro usan infraestructura compartida; SQL, permisos, Web.config, SII y almacenamiento permanecen intactos. El JavaScript validado se aplicó al ambiente de pruebas con autorización específica; UI-01/02/03 reales PASS. Implementación técnica 12/12, pendiente revisión editorial/publicación. La reversa futura es el diff focal de DOC-113, sin deshacer recursos aprobados del ambiente.

Evidencia y límites: [Informe de implementación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md).
