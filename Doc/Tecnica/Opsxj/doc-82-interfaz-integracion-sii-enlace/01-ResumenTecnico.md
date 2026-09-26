# INTERFAZ-INTEGRACION-SII-ENLACE

- Ticket: DOC-82
- Cambio OpenSpec: doc-82-interfaz-integracion-sii-enlace
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Integrar la experiencia moderna de consulta, preview, preparacion e importacion
de anexos SII en la preasignacion ENLASE, reutilizando los contratos DOC-80 y
DOC-81. La asignacion permanece como una accion posterior y explicita, con la
revalidacion autoritativa existente en `Buttonaceptar_Click`.

## Alcance y compatibilidad

- [x] Paginas, controles, servicios y scripts afectados identificados en el
  inventario tecnico y los diagramas de
  `Doc/Actualizacion/workflow/ImportarServiciWebEnlace/DOC-82-interfaz-integracion-asignacion/`.
- [x] Compatibilidad preservada: el mismo modal, proveedor, cliente API y
  contratos atienden constancias y anexos, aislados por capacidad.
- [x] Reversa verificada: al desactivar `WorkflowCentroTrabajoModernActive`, el
  disparador ENLASE conserva el recorrido legacy; la corrida E2E dejo el gate
  en `false`, sin usuarios ni grupos configurados.
