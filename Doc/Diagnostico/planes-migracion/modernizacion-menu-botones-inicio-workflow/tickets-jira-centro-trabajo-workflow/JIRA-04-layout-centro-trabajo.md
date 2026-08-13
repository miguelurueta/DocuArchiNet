# JIRA-04 — Layout Centro de Trabajo sin bandeja

## Prompt para Jira

**Rol:** Actúa como diseñador UI senior y arquitecto CSS especializado en modernización gradual de interfaces empresariales WebForms sin reemplazo de markup funcional.

Implementa el layout aprobado de `centro-trabajo-workflow-sin-bandeja.html` mediante CSS, sin mover controles críticos. Una tarea seleccionada debe mostrar contexto, acciones, documentos relacionados, visor e índice como una superficie de trabajo coherente.

Los controles presentados dentro del layout deben adoptar el contrato `CONTRATO-CSS-COMPONENTES-REUTILIZABLES.md`: barras blancas con línea `#dce2f1`, texto de contexto `#35477f`, fondo operativo `#f5f7fb` y componentes reutilizables, no estilos locales de página.

### Alcance

- Crear zonas Grid: contexto, acciones, documentos, visor, índice/metadatos y estado.
- En escritorio usar tres columnas para documentos, visor e índice; definir anchos mínimos y colapso controlado.
- En móvil apilar: contexto, acciones, documentos, visor, índice.
- Ocultar la bandeja solo en modo moderno y únicamente dentro del área de centro de trabajo.
- Conservar una acción visible de recuperación: cambiar tarea o volver a bandeja.

### Restricciones no negociables

- La bandeja conserva HTML, selección y lógica.
- No cambiar markup emitido por servidor salvo agregar la clase de activación aprobada.
- No usar alturas fijas que oculten visor, índice o acciones.

### Entregables técnicos

1. `01-LayoutGrid.md` con áreas, breakpoints y mínimos de columna.
2. `02-ImpactoUI.md` con capturas antes/después.
3. `03-RollbackLayout.md`.

### Criterios de aceptación

- Abrir tarea, documento e índice conserva contexto y visor.
- No existen solapamientos entre dropdown, visor, índice y modal.
- A 375 px no desaparecen acciones esenciales ni queda scroll horizontal no intencional.

### Pruebas requeridas

- 1366, 1024, 768 y 375 px.
- Documento con título largo, lista extensa y metadatos extensos.

### Reversión

Retirar la clase moderna; los paneles vuelven a su flujo legacy.
