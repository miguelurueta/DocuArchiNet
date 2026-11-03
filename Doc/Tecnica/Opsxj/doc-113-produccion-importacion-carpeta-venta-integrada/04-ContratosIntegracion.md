# DOC-113 — Ventana integrada

- Ticket: DOC-113
- Cambio OpenSpec: doc-113-produccion-importacion-carpeta-venta-integrada
- Clasificacion: cross_cutting (Transversal)

## Contratos e integraciones

ASMX/ASHX conservan sus contratos. El frontend consume el mismo sobre y rechaza estructuras anidadas incompatibles antes de asignar el modelo. La suite usa openSession/authenticated-workflow-session, perfil base cerrado, consola interactiva y puente DSN registrados. El selector no crea .env ni pide URLs MySQL; credenciales efímeras solo en consola. Se compara el SHA del JavaScript servido con el candidato antes de operar y se conserva evidencia por caso, sin headers o contenido privado.

Evidencia y límites: [Informe de implementación](../../../Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md).
