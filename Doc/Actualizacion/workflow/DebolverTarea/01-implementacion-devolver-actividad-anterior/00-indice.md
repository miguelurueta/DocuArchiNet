# DOC-32 — Implementación de Devolver a actividad anterior

Paquete técnico del corte backend seguro para devolver una tarea Workflow a una actividad anterior autorizada. La entrega contiene contratos ASMX, reglas de servidor, protección de concurrencia, integración encapsulada con el motor legacy y evidencia de pruebas; no entrega interfaz WebForms ni cambia configuración de ambiente.

- Ticket: DOC-32
- Cambio OpenSpec: `doc-32-backend-actividad-anterior`
- Clasificación: `cross_cutting`
- Estado: implementación, pruebas focales y E2E autorizada completadas.

- [Arquitectura y componentes](01-arquitectura.md)
- [Contratos, endpoints y códigos](02-contrato.md)
- [Flujo, seguridad, límites y relevo](03-flujo-y-seguridad.md)
- [Pruebas, evidencia y riesgos](04-pruebas-y-evidencia.md)
- [Inventario de componentes](05-inventario-funciones.md)
- [Diagramas](Diagramas/)

La siguiente etapa debe implementar la interfaz oficial consumiendo exclusivamente estos contratos. No debe reconstruir permisos, Ruta, Flujo, destinos, cursor ni invocar el motor legacy.

La etapa DOC-33 ya materializa esa interfaz oficial en un paquete separado, con el mismo criterio de aislamiento y sin modificar los contratos del servidor: [DOC-33 — Interfaz moderna](../02-interfaz-moderna-devolver-actividad-anterior/00-indice.md).

La compuerta posterior de evidencia se consolida en [DOC-34 — Verificación transversal](../03-verificacion-transversal-devolver-actividad-anterior/00-indice.md). No reejecuta ni reemplaza la E2E autorizada registrada en este paquete.
