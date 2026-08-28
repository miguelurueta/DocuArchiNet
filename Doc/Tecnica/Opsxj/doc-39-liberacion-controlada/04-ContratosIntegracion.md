# Contratos e integraciones de la liberación

- Ticket: DOC-39
- Cambio OpenSpec: doc-39-liberacion-controlada
- Clasificacion: cross_cutting

## Contratos e integraciones

DOC-39 no crea ni modifica endpoints, payloads, autenticación, esquemas ni integraciones. La liberación candidata mantiene los dos contratos existentes:

| Operación | Entrada permitida | Garantía preservada |
| --- | --- | --- |
| `PreviewDevolverUsuarioAnterior` | Identidad de la tarea permitida por el contrato | Devuelve contexto mínimo y token opaco; no cambia tarea, estado ni auditoría. |
| `EjecutarDevolverUsuarioAnterior` | `idTarea` y `tokenVersion` vigentes | Revalida en servidor y ejecuta una sola transición si el lock y el historial continúan vigentes. |

El ASMX recupera el contexto autenticado en servidor y no manipula `Page`, controles ni handlers Web Forms. El token no se registra en la evidencia de liberación. La matriz de ambiente tampoco contiene credenciales, cookies, DSN, hosts internos, cadenas de conexión ni cuentas de usuario.

La operación no puede utilizar como alternativa los contratos de Devolver a actividad anterior, Continuar flujo, Enviar a usuario ni Enviar a grupo. La liberación no cambia sus destinos, confirmaciones, gates, payloads ni dependencias. Cualquier divergencia de contrato entre el artefacto revisado y el artefacto que se proponga liberar obliga a detener la ventana y solicitar revisión técnica.

## Reversión e interoperabilidad

La reversión corresponde al mecanismo de gestión de despliegue autorizado para el ambiente. Devuelve únicamente el artefacto de aplicación a una versión aprobada y solo condiciona intentos nuevos. No actualiza `estados_tarea_workflow`, no borra auditoría, no reasigna tareas y no reconstruye historial. La continuidad de las integraciones se verifica contra los contratos anteriores, sin introducir un postback ni una ruta de interfaz alternativa.
