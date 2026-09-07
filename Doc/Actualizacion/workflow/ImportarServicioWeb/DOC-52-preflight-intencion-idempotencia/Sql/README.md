# Aplicación manual del esquema DOC-52

Estos scripts no son ejecutados por OPSXJ ni por las pruebas. Requieren autorización explícita del ambiente, respaldo verificado y ventana de cambio.

## Precondiciones

1. Confirmar que no existen las tres tablas `workflow_import_intent*`.
2. Validar versión MySQL, charset y permisos DDL con el responsable del ambiente.
3. Tomar respaldo y registrar responsable, fecha y ambiente sin guardar credenciales.

## Aplicación

Ejecutar manualmente `001-create-import-intents.sql` completo y verificar tablas, claves foráneas, índices y restricción única. El código permanece desconectado de endpoints hasta una entrega posterior.

## Rollback

Solo si no existen intenciones que deban conservarse, ejecutar `002-rollback-import-intents.sql`. El orden elimina elementos, requisitos y finalmente cabeceras. Si hay datos, detenerse y definir exportación/retención antes de eliminar.
