# Migración manual GUID → AUTO_INCREMENT — DOC-110

Estado: entregada, no aplicada por el agente. Destino: base RA resuelta mediante OdbcServicesGestor → gestor_modulos activo GESTOR DOCUMENTAL. No ejecutar en DA ni suponer que todas las tablas comparten base. SQL 01 es para instalación nueva; no volver a ejecutarlo en la instalación GUID existente.

## Orden de aplicación

1. Preparar respaldo restaurable de RA y de la versión desplegada. Detener todos los escritores del perfil, cerrar sesiones del editor y mantenerlos detenidos hasta terminar verificaciones y despliegue. DDL MySQL 5.1 no tiene rollback transaccional.
2. Confirmar las tablas originales y las FK fk_ra_integracion_version_perfil y fk_ra_integracion_regla_version contra SQL 04. Confirmar que no existen tablas con sufijos _ai/_guid_respaldo ni las tres tablas ra_integracion_mapa_* ni ra_integracion_migracion_huella de una ejecución anterior. Si existen, investigar la etapa alcanzada; no borrar ni repetir automáticamente.
3. Ejecutar manualmente 03-migrar-identificadores-perfil-mysql51.sql en RA, deteniendo el cliente ante el primer error. Crea tablas de destino, correspondencias y auditoría; copia filas y referencias. No activa todavía el esquema ni interpreta JSON.
4. Desde PowerShell, ejecutar la utilidad sin -Apply para comparar los datos. Requiere las DLL MySql.Data y Newtonsoft.Json existentes en bin, Web.config accesible y permisos de lectura. No imprime conexiones ni contenido documental.

```powershell
& '.\Doc\Actualizacion\ProduccionDcoumental\ImportacionCarpetas\Sql\03-verificar-transformar-json.ps1' -RepositoryRoot 'D:\imagenesda\DocuachiNet\DocuArchiNet'
```

5. Si PREVIEW_READ_ONLY termina PASS, ejecutar el mismo comando agregando -Apply. Esta modalidad modifica solo tablas _ai y auditoría, en una transacción; compara filas, pertenencias, JSON y huellas. Exigir PASS con modo STAGING_TRANSFORMED antes de continuar. El preview por sí solo no autoriza el cambio de tablas. El usuario aplica esta transformación manual, igual que el SQL.
6. Ejecutar 04-activar-identificadores-perfil-mysql51.sql en RA, sin continuar ante errores. Retira las dos FK originales, renombra las tablas conservando respaldos y agrega las FK de destino y respaldo. No se depende de la actualización implícita de nombres de FK ni se desactiva FOREIGN_KEY_CHECKS.
7. Ejecutar la utilidad agregando -PostCutover (sin -Apply): exige POST_CUTOVER_READ_ONLY PASS, compara respaldo/correspondencias/destino y huellas auditadas, y comprueba las relaciones. No modifica datos.
8. Desplegar código incremental junto con sus recursos JS y reiniciar el sitio dentro de la ventana. El bin activo anterior no es compatible con IDs incrementales. Comprobar disponibilidad/esquema y correr los recorridos E2E existentes con perfiles descartables autorizados; comprobar IDs nuevos, historial, publicación y aislamiento. Solo entonces reabrir escritores.

La utilidad acepta hasta 50.000 filas por tabla, JSON de hasta 262.144 caracteres y profundidad 16; supera esos límites con rechazo, no truncamiento. No está diseñada como migrador genérico. Un fallo muestra la fase sin secretos. Un fallo requiere inspección controlada; no pasar al paso siguiente.

## Recuperación de una ejecución parcial

Antes del cambio de nombres, las tablas originales permanecen intactas. Mantener escritores detenidos y corregir la causa. La transformación -Apply puede repetirse únicamente sobre la misma copia y correspondencias verificadas; SQL 03 no es idempotente. Si SQL 03 quedó parcial, recuperar desde el respaldo o preparar un procedimiento específico tras identificar objetos y filas creados.

Durante SQL 04, registrar la última sentencia exitosa. No repetir todo el archivo: puede haber FK retiradas, nombres ya cambiados o FK pendientes. Inspeccionar metadatos y completar únicamente el paso pendiente con revisión del operador, o restaurar el respaldo completo y el código compatible. No confundir una transacción de datos con rollback del DDL.

Después del cambio, conservar tablas _guid_respaldo, mapas y auditoría. No hay limpieza automática. Si ya se admitieron escrituras nuevas, volver a las tablas antiguas perdería esos cambios: requiere conciliación o restauración planificada. Las claves incrementales no sustituyen pruebas de concurrencia ni aislamiento por empresa.

## Evidencia pendiente

No se ha ejecutado SQL 03/04 ni -Apply contra RA. La compilación aislada y pruebas del conversor acreditan código, no migración real. Tareas oficiales 10.16, 10.18 y 10.19 permanecen abiertas hasta resultados reales.

Actualización 2026-10-20: aplicada y verificada en localhost/RA por autorización explícita posterior del usuario; ver Validacion/doc110-correccion-incrementales.md. El procedimiento manual sigue vigente para otros destinos. El servidor principal no ha sido intervenido.
