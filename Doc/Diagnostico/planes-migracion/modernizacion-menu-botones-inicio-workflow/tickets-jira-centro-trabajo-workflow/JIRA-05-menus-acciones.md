# JIRA-05 — Normalización de menús y acciones Workflow

## Prompt para Jira

**Rol:** Actúa como diseñador senior de interacción empresarial y desarrollador WebForms, experto en permisos, acciones críticas, menús contextuales y prevención de errores operativos.

Moderniza la presentación de los menús y botones existentes sin cambiar eventos, permisos ni reglas de transición.

### Contrato visual obligatorio

Usar exclusivamente los componentes del archivo `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`. Los triggers `Opciones`, `Detalle` y `Servicios` usan `.ctw-btn`; sus paneles usan `.ctw-menu__panel`; los ítems usan `.ctw-menu__item`. `Enviar` usa `.ctw-btn--primary`; `Cerrar tarea` usa `.ctw-btn--danger`; `Notas` y `Autorizar` conservan `.ctw-btn`. No crear variantes por pantalla ni aplicar reglas directas a `.btn` o `.dropdown-menu` globales.

### Alcance

- Agrupar visualmente paneles existentes en `Opciones`, `Detalle`, `Servicios` y `Documentos`.
- Conservar acceso directo de `Notas` y `Autorizar` cuando estén permitidos.
- Mostrar `Devolver`, `Pendiente`, `Enviar` y `Cerrar tarea` como acciones de transición diferenciadas al extremo derecho.
- Agrupar acciones del documento en la barra del visor: cargar, metadatos, versiones y más acciones.
- Añadir nombres accesibles a iconos sin alterar el evento original.

### Restricciones no negociables

- Cada acción debe disparar el mismo evento/elemento existente; no crear acciones duplicadas.
- La visibilidad por permiso del servidor prevalece sobre el layout.
- Cerrar/eliminar conserva confirmación existente; no se añade bypass.

### Entregables técnicos

1. `01-MapaAcciones.md`: acción, control existente, permiso y ubicación visual.
2. `02-ContratoMenus.md`: foco, Escape, z-index y cierre tras postback.
3. Evidencia de cada grupo y de acciones deshabilitadas/no autorizadas.

### Criterios de aceptación

- Cada menú ejecuta exactamente el comportamiento legacy.
- No hay dos accesos equivalentes visibles para la misma operación.
- Acciones destructivas se distinguen por texto, no solo por color.

### Pruebas requeridas

- Usuario autorizado/no autorizado.
- Apertura, teclado, Escape y postback desde cada grupo.

### Reversión

Retirar reglas de agrupación; conservar el layout de JIRA-04 si está estable.
