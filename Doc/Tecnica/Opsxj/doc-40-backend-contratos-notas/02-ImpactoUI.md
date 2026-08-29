# BACKEND-CONTRATOS-NOTAS

- Ticket: DOC-40
- Cambio OpenSpec: doc-40-backend-contratos-notas
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

DOC-40 no modifica ninguna superficie visual. `workflow/Webworkflow.aspx`, sus modales, controles, JavaScript, foco, selección, diseño responsive y accesibilidad permanecen sin cambio. El consumidor Centro de Trabajo Workflow continuará usando su flujo legacy hasta que una fase posterior disponga de contratos operativos y autorización expresa para migrarlo.

El diseño deja una condición de integración: las acciones futuras de Notas solo reflejarán la autorización ya calculada por el servidor; la interfaz no será fuente de identidad, grupo, permiso, tarea objetivo ni ruta Workflow. La ruta de negocio no se representa como URL ni como un valor oculto confiable.

## Validacion visual

No aplica una validación visual en este refinamiento porque no hay HTML, CSS, JavaScript ni endpoint expuesto. La futura fase de consumidor deberá validar modal o panel de Notas, navegación con teclado, foco, Escape, estados de carga/error, ancho reducido y ausencia de doble acción, con evidencia en el ambiente autorizado.
