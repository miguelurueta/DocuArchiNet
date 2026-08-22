# Servicios y reglas — Liberación controlada de Enviar a usuario

- Ticket: DOC-31
- Cambio OpenSpec: doc-31-liberacion-controlada-enviar-usuario
- Clasificacion: cross_cutting

## Servicios y reglas

DOC-31 no modifica servicios. La liberación futura debe conservar `PreviewEnviarUsuario` como lectura y `EjecutarEnvioUsuario` con revalidación de autorización, tarea, usuario–actividad, token, requisitos y lock. La respuesta que requiere tratamiento se bloquea sin reasignación y la auditoría conserva el mecanismo sanitizado `ASMX_ENVIO_USUARIO`.
