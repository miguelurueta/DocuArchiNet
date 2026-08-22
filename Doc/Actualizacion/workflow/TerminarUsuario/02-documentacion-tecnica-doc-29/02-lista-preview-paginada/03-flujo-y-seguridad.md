# Flujo, seguridad y compatibilidad — Lista preview

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Flujo de interfaz

Al abrir el modal se consulta la primera página. Una búsqueda exige dos caracteres y espera 300 ms antes de llamar al endpoint. Cada término nuevo reinicia cursores y cada página conserva su cursor de origen. Vaciar el campo recupera la primera página sin término.

Antes de iniciar una consulta, el adaptador invalida la selección previa, aborta la solicitud anterior cuando es posible y aumenta la secuencia. Una respuesta cuyo contador no coincide no modifica la pantalla. Lista vacía, término corto y error controlado conservan el modal y muestran un mensaje acorde al estado.

## Seguridad y aislamiento

Preview es una operación de solo lectura: no toma el lock de ejecución, no escribe auditoría, no modifica tarea ni invoca el motor legacy. El servidor reconstruye el contexto autenticado y aplica permiso, tarea y ruta antes de consultar destinos. La selección resultante no autoriza la ejecución; se revalida en `EjecutarEnvioUsuario`.

Cerrar el modal, cambiar búsqueda, navegar o recibir una respuesta tardía impide confirmar un destino obsoleto. La interfaz no activa gates ni realiza postback legacy.
