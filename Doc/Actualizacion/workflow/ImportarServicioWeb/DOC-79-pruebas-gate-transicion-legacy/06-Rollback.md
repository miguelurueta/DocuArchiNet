# Rollback

## Procedimiento

1. Establecer `WorkflowCentroTrabajoModernActive=false`.
2. Vaciar `WorkflowCentroTrabajoModernUsers` y `WorkflowCentroTrabajoModernGroups`.
3. Confirmar que no se registran assets ni bootstrap de Importar Servicio Web moderno.
4. Confirmar que el ASMX moderno responde `FEATURE_DISABLED` sin efectos.
5. Verificar que `btnloadservice`, modales, postbacks y handlers legacy siguen disponibles.

El rollback no elimina documentos, no revierte intenciones persistidas y no reejecuta operaciones.

## Estado obligatorio después de pruebas

- Gate: `false`.
- Usuarios: vacío.
- Grupos: vacío.
