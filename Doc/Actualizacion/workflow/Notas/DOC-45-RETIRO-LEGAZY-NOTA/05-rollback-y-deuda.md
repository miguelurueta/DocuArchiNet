# Rollback y deuda residual

## Unidad de rollback

DOC-45 debe revertirse como una unidad coherente mediante control de versiones. No es seguro restaurar solamente el botón legacy ni mantener simultáneamente las dos presentaciones.

El rollback comprende:

1. Restaurar la rutina VB retirada y los controles, designer y handlers legacy exclusivos de `Webworkflow`.
2. Revertir coordinadamente `PuedeGestionar`, `NotOwner` y sus cambios en modelo, DTO, repositorio y ASMX.
3. Revertir el acceso moderno, diálogos, listener delegado, integración `PageRequestManager.endRequest` y versiones de caché.
4. Revertir los estilos locales de modal, visor, confirmación, acciones de tareas e índice.
5. Revertir las ampliaciones DOC-45 del runner, políticas y E2E si dejan de corresponder al producto restaurado.
6. Compilar, ejecutar regresiones y comprobar el gate seguro antes de liberar la reversión.

No se requiere DDL ni reversión de datos. Las notas temporales de E2E se eliminan dentro de sus corridas. Si una reversión se interrumpe, el sistema no debe liberarse con doble presentación o contratos cliente/servidor desalineados.

## Deuda conservada conscientemente

- Endpoints `Service_*_nota_tarea_workflow`, porque mantienen consumidores externos vivos.
- `WebFormAnotacion.aspx(.vb)`, porque continúa incluido y su uso operativo no ha sido descartado.
- Scripts legacy compartidos por Workflow, Radicación y Gestión de Correspondencia.
- Las advertencias de compilación preexistentes, cuantificadas por separado y no introducidas por DOC-45.

Cada retiro futuro exige un cambio independiente, inventario sin consumidores, pruebas y autorización de alcance.
