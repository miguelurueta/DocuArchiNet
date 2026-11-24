## Why

El backend Excel de DOC122–125 ya recibe, prepara y confirma cargas con resultado durable, pero Producción Documental aún no ofrece su recorrido de interfaz. DOC126 permite usar esa capacidad desde la navegación habitual con su propio permiso, revisión previa y recuperación por ID.

## What Changes

- Acceso Cargar datos desde Excel en el menú común, independiente de los permisos de carpetas/perfiles/fuentes/proveedores.
- Administración de plantillas con los servicios existentes, selección de XLS/XLSX y hoja, validación completa, preview/incidencias acotadas y confirmación explícita de reemplazo.
- Estados operativos, protección contra doble envío/respuestas tardías y recuperación del resultado sin reimportar automáticamente.
- Interacción accesible y E2E integrada que reutiliza autenticación, conexiones, verificadores y actor existentes, con autorización DOC126 propia.

## Capabilities

### New Capabilities
- `importacion-excel-06-interfaz-menu`: interacción autorizada para administrar plantillas y cargar/consultar Excel desde Producción Documental.

### Modified Capabilities
Sin cambios de requisitos de backend: se reutilizan los contratos y la semántica DOC122–125.

## Impact

Página de Producción Documental, cliente/vista/CSS de Excel y referencia contextual de recuperación; reutilización del menú, servicios y persistencia. Tres nuevos Content en vbproj. Se conservan net461/MySQL5.1.37/MySql.Data6.2.3 y límites20000filas/10MB. El diseño incluye inventario exacto de rutas y aceptación con recursos existentes. No se ejecutan instalación, E2E real ni despliegue durante el refinamiento.

## Fuente y trazabilidad

Ticket DOC126 IMPORTACION-EXCEL-06-INTERFAZ-MENU, fase6/7. Fuente íntegra e inmutable en specs/importacion-excel-06-interfaz-menu/jira-context.md; prompt06 en exploración. Revisión técnica del2026-11-24 y plan en refinement/design/spec/tasks, sin atribuir aprobación humana ni ejecución real.
