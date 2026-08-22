# Contratos e integración — Verificación transversal de Enviar a usuario

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificacion: cross_cutting

## Contratos e integraciones

`PreviewEnviarUsuario` recibe tarea, consulta, cursor y tamaño de página; devuelve solamente destinos autorizados, token y cursor público. `EjecutarEnvioUsuario` recibe tarea, usuario destino, actividad destino y token. Ninguno usa `IdConector`, controles Web Forms, `Page` o `Session` como contrato público. Continuar flujo mantiene su operación separada con `IdConector`; la verificación no agrega endpoints, campos ni cambios de esquema.
