# Despliegue y rollback DOC-67

## Despliegue inicial

1. Confirmar que no existe una ejecución de importación activa.
2. Respaldar las tablas modernas y aplicar en orden las migraciones versionadas DOC-67.
3. Validar columnas, índices y claves mediante consultas `SELECT`/`information_schema`.
4. Desplegar los binarios con `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos vacíos.
5. Ejecutar pruebas locales y smoke test sin mutación.
6. Autorizar explícitamente ambiente, cuenta y recurso descartable antes de habilitar el gate.
7. Habilitar primero un usuario controlado, observar estados parciales/inciertos y verificar SQL/XML.
8. Al terminar, restaurar siempre:

```text
WorkflowCentroTrabajoModernActive=false
WorkflowCentroTrabajoModernUsers=
WorkflowCentroTrabajoModernGroups=
```

## Rollback no destructivo

1. Apagar y vaciar el alcance del gate; esto devuelve el tráfico al recorrido legacy.
2. No borrar ni compensar expedientes, documentos, vínculos, índices o XML ya confirmados.
3. Conservar el diario moderno para reconciliación y auditoría.
4. Revertir binarios si es necesario.
5. Ejecutar rollback DDL únicamente sin corridas activas, después de respaldar evidencia y comprobar que las tablas/columnas modernas no contienen datos que deban conservarse.

Está prohibido usar eliminación de documentos o expedientes como mecanismo de reversa.
