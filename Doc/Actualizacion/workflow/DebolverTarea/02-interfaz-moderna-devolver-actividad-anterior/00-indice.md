# DOC-33 — Interfaz moderna para devolver a actividad anterior

Este paquete documenta la segunda etapa de Devolver tarea. DOC-32 entregó los contratos y las reglas de servidor; DOC-33 reemplaza únicamente el acceso Web Forms legado de actividad anterior por una experiencia moderna, aislada y accesible que consume esos contratos sin reinterpretarlos.

- Ticket: DOC-33
- Cambio OpenSpec: `doc-33-interfaz-moderna-devolver-tarea`
- Clasificación: `cross_cutting`
- Estado: implementación y verificación local completadas; una E2E autenticada requiere autorización independiente de ambiente y cuentas.

- [Arquitectura y componentes](01-arquitectura.md)
- [Contratos y estado de cliente](02-contrato.md)
- [Flujo, seguridad, accesibilidad y relevo](03-flujo-y-seguridad.md)
- [Pruebas, evidencia y límites](04-pruebas-y-evidencia.md)
- [Inventario de funciones implementadas y reutilizadas](05-inventario-funciones.md)
- [Diagramas](Diagramas/)

DOC-33 no habilita ni consulta `WorkflowCentroTrabajoModernActive`; tampoco agrega configuración, gates, esquemas ni cambios de base de datos.
