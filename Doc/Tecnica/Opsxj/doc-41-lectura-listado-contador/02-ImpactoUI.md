# LECTURA-LISTADO-CONTADOR

- Ticket: DOC-41
- Cambio OpenSpec: doc-41-lectura-listado-contador
- Clasificacion: cross_cutting (Transversal)
## Superficies UI

- No se modifican páginas WebForms, UserControls, modales ni tablas existentes.
  DOC-41 incorpora exclusivamente operaciones ASMX modernas para la lectura de
  Notas Workflow.
- No hay cambios en foco, hover, selección, responsive ni accesibilidad. Los
  clientes actuales conservan su comportamiento porque los endpoints legacy y
  el gate de activación permanecen sin alteración.

## Validacion visual

No se requiere captura visual por no existir una superficie UI nueva. El
recorrido autorizado de lectura verificó que la sesión Workflow consulta el
ASMX especializado sin activar el centro de trabajo moderno ni modificar
páginas legacy.
