# Flujo, seguridad y límites

- Ticket: DOC-30
- Cambio OpenSpec: doc-30-verificacion-transversal-enviar-usuario
- Clasificación: cross_cutting

## Flujo de verificación

La revisión sigue la cadena preview, selección, confirmación, ejecución y actualización correlacionada. Se comprueba que el navegador solo expresa intención; el servidor conserva el cálculo de permiso, la validación del destino usuario–actividad, el token de versión y la exclusión mutua por `GET_LOCK`.

## Seguridad y aislamiento

Los adaptadores de usuario no invocan `After_envio_usuario_workflow` ni reasignan respuesta. Una respuesta que requiere tratamiento bloquea con resultado funcional. La auditoría usa `ASMX_ENVIO_USUARIO` y normaliza el mecanismo. El modal bloquea confirmar, cancelar, fondo, Escape, API y recarga mientras la respuesta está en curso; tras ella restaura el cierre normal.

## Límites operativos

DOC-30 no ejecutó E2E autenticado, carga, cambio de gate, despliegue ni consultas de ambiente. La etapa posterior debe mantener las consultas de control en solo lectura y no revertir transiciones ya confirmadas.
