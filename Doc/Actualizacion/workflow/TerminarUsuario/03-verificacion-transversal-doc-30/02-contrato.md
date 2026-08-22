# Contratos y compatibilidad

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificación: cross_cutting

## Endpoints y contratos

`PreviewEnviarUsuario` usa `{ idTarea, consulta, cursor, tamanoPagina }` y ofrece una página de destinos autorizados, token y cursor. `EjecutarEnvioUsuario` usa `{ idTarea, idUsuarioWorkflowDestino, idActividadDestino, tokenVersion }`. El contrato público no expone permisos, sesión, SQL, controles de página ni `IdConector`.

## Compatibilidad de integración

La inspección y las pruebas confirman que preview no termina tareas ni produce auditoría. La ejecución conserva revalidación de autorización, tarea, destino, token, respuesta y lock. Grupo y Continuar flujo mantienen listeners y payloads propios; en particular, `IdConector` continúa siendo requisito de la transición por conector y no se acepta en la ruta de usuario.

## Resultado

No se detectó cambio de contrato, endpoint ni esquema atribuible a DOC-30.
