# Índice de planes de migración: precompilado a repositorio local

## Regla de documentación

Cada implementación funcional o visual se planifica, valida y revierte en su propio archivo. No se mezclan cambios de tablas, sesión, menú o login dentro de un único plan de ejecución.

**Repositorio destino:** `D:\imagenesda\DocuachiNet\DocuArchiNet`
**Origen de contraste:** `D:\temfile\Gestion`

## Planes vigentes

| Implementación | Plan específico | Estado |
|---|---|---|
| Modernización de tablas GridView | `plan-implementacion-modernizacion-tablas.md` | Vigente; incluye Fases 1, 2 y 3 de tablas. |
| Buscador, filtros y contador de tareas Workflow | `plan-migracion-controles-busqueda-workflow.md` | Pendiente de migración al repositorio. |
| Menú responsivo y cabecera de Inicio | `plan-migracion-menu-inicio-repositorio.md` | Pendiente de migración al repositorio. |
| Manejo de sesión Workflow | `plan-migracion-sesion-workflow-repositorio.md` | Pendiente de migración al repositorio. |
| Vigilancia general de sesión en Inicio | `plan-migracion-sesion-inicio-repositorio.md` | Pendiente; requiere handler y script como paquete único. |
| Ajuste visual del login | `plan-migracion-ajuste-login-repositorio.md` | Pendiente; cambio focalizado en `gestor.aspx`. |

## Reglas comunes

- Aplicar un solo plan por vez y validar antes de iniciar el siguiente.
- Migrar por bloques y archivos puntuales; nunca copiar directorios completos desde `D:\temfile\Gestion`.
- No tocar DLL, `bin`, `obj`, dependencias de terceros ni archivos minificados salvo que el plan específico lo autorice expresamente.
- Crear respaldo o commit previo antes de cada implementación.
- Registrar en el plan específico la versión de recursos, prueba realizada, resultado y reversión.
- La implementación de tablas se rige exclusivamente por su plan ya existente; no se duplica en los demás documentos.
