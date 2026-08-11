# JIRA-02 — Infraestructura visual aislada y reversible

## Prompt para Jira

**Rol:** Actúa como arquitecto frontend senior para aplicaciones ASP.NET WebForms, experto en CSS incremental, carga de recursos legacy y mecanismos de feature flag reversibles.

Implementa la infraestructura opt-in de modernización del centro de trabajo Workflow. Debe cargar una capa visual aislada sin modificar el comportamiento existente y debe poder desactivarse sin despliegue de lógica de negocio.

### Alcance

- Crear `Styles/workflow-centro-trabajo-moderno.css` y `js/workflow/centro-trabajo-visual.js`.
- Cargarlos después de recursos legacy, con versión de caché explícita.
- Definir una bandera de entorno/configuración que emita exclusivamente la clase `workflow-centro-trabajo-moderno` sobre el contenedor autorizado.
- Definir contrato de selectores, variables CSS, capas `z-index` y clases de estado.
- Implementar los tokens y componentes de `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`; el resultado visual debe reproducir el HTML base, no una interpretación libre.

### Restricciones no negociables

- Sin la clase de activación la interfaz debe ser idéntica a la actual.
- No sustituir HTML ni alterar controles ASP.NET.
- No habilitar por CSS una acción que el servidor haya ocultado por permisos.
- El adaptador no puede lanzar errores que bloqueen scripts existentes.

### Entregables técnicos

1. `01-ArquitecturaActivacion.md`: bandera, carga, selectores permitidos y rollback.
2. `02-ContratoCSS.md`: variables, breakpoints y `z-index`.
3. `03-PruebasActivacion.md`: evidencia modo apagado/encendido.
4. Hoja CSS con componentes scoped: `.ctw-btn`, `.ctw-icon-btn`, `.ctw-menu`, `.ctw-menu__panel`, `.ctw-badge`, `.ctw-action-bar` y `.ctw-document-bar`.

### Criterios de aceptación

- Activar/desactivar la clase cambia solo presentación.
- No hay errores JavaScript en consola durante carga ni postback.
- La reversión es retirar la bandera o la clase; no requiere revertir eventos.

### Pruebas requeridas

- Comparación visual y funcional con bandera en `0` y `1`.
- Validar caché tras recarga forzada y navegación interna.

### Reversión

Desactivar la bandera; mantener recursos publicados para no generar referencias rotas.
