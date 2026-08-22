# Contratos e integración — Liberación controlada de Enviar a usuario

- Ticket: DOC-31
- Cambio OpenSpec: doc-31-liberacion-controlada-enviar-usuario
- Clasificacion: cross_cutting

## Contratos e integraciones

Enviar a usuario conserva los contratos directos de preview y ejecución por tarea, usuario, actividad y token; no usa `IdConector`. Continuar flujo mantiene sus endpoints y payload por `IdConector`. La liberación no agrega endpoints, campos, esquema ni una configuración de habilitación, y una reversión restaura el paquete previo sin revertir transiciones confirmadas.
