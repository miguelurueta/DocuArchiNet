# Contratos e integraciones — Verificación transversal

- Ticket: DOC-38
- Cambio OpenSpec: doc-38-verificacion-transversal
- Clasificacion: cross_cutting

## Contratos e integraciones

DOC-38 verifica los contratos existentes `PreviewDevolverUsuarioAnterior` y `EjecutarDevolverUsuarioAnterior`; no agrega endpoints ni modifica sus payloads. El preview recibe únicamente la identidad de tarea permitida por el contrato y retorna un contexto mínimo junto con un token opaco. La ejecución acepta únicamente `idTarea` y `tokenVersion` vigentes.

El ASMX reconstruye el contexto autenticado en servidor y no manipula `Page`, controles Web Forms ni handlers. La verificación confirma que Ruta y Flujo se resuelven desde datos autorizados, que el usuario Workflow histórico se preserva cuando corresponde y que las respuestas funcionales no filtran detalles técnicos, tokens ni datos sensibles.

La comparación de integración revisa que Devolver actividad anterior, Continuar flujo, Enviar a usuario y Enviar a grupo preservan sus contratos. No se permite reutilizar sus destinos, gates, confirmaciones o flujos como una alternativa para usuario anterior.
