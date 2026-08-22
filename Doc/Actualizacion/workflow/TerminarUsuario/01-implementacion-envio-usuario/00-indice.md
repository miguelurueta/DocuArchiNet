# DOC-28 y DOC-29 — Implementación de Enviar a usuario

Paquete documental único de la implementación de Enviar a usuario. DOC-28 entrega backend, contratos y controles de servidor; DOC-29 entrega la interfaz moderna oficial, accesible y aislada de Continuar flujo.

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificación: cross_cutting

- [Arquitectura y componentes](01-arquitectura.md)
- [Contratos, endpoints y códigos](02-contrato.md)
- [Flujo, seguridad, límites y relevo](03-flujo-y-seguridad.md)
- [Pruebas, evidencia y riesgos](04-pruebas-y-evidencia.md)
- [Inventario de funciones implementadas y reutilizadas](05-inventario-funciones.md)
- [Diagramas](Diagramas/)
- [Documentación técnica y gobernanza DOC-29](../02-documentacion-tecnica-doc-29/00-indice.md)

Estado: backend e interfaz local validados. La experiencia de usuario no depende de gate; no hubo cambios de configuración, E2E autenticado ni carga. La verificación operativa queda para la etapa 03, con autorización explícita.
