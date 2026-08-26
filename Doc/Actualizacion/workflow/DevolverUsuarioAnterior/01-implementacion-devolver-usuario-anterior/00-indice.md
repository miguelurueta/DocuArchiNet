# DOC-36 — Backend de devolución a usuario anterior

Este paquete registra las decisiones aprobadas de diseño que habilitan DOC-36 y la evidencia de su implementación. La capacidad es exclusiva de **Devolver → Usuario anterior**: no es un selector de actividades ni una devolución a grupo.

- Ticket: `DOC-36`
- Cambio OpenSpec: `doc-36-backend-devolucion-usuario-anterior`
- Alcance de esta etapa: ASMX, contratos, dominio, infraestructura, auditoría y pruebas focales.
- Fuera de alcance: interfaz, activación, configuración, E2E autenticada, carga y liberación.

| Documento | Contenido |
| --- | --- |
| [01-arquitectura.md](01-arquitectura.md) | Decisiones, capas y punto mutante. |
| [02-contrato.md](02-contrato.md) | Endpoints, DTOs y códigos públicos. |
| [03-flujo-y-seguridad.md](03-flujo-y-seguridad.md) | Historial, token, lock y parámetros del motor. |
| [04-pruebas-y-evidencia.md](04-pruebas-y-evidencia.md) | Matriz de pruebas y límites de evidencia. |

## Decisión de entrada aprobada

El antecedente es exclusivamente la segunda fila de `estados_tarea_workflow` de la misma tarea al ordenar `id_Estado DESC`: la primera es el estado actual y la segunda el usuario histórico inmediato anterior. No se buscan filas más antiguas ni se realiza fallback a actividad anterior.

La ejecución llamará al motor solo mediante un adaptador exclusivo, con `Page = Nothing`, notificación desactivada, actualización de interfaz legacy desactivada y eventos dinámicos desactivados. Esto preserva el límite de no tratar respuestas.
# Paquete técnico DOC-36

Estructura alineada con el paquete de referencia de devolución de actividad anterior.

- [Inventario de componentes](05-inventario-funciones.md)
- [Diagramas](Diagramas/)
