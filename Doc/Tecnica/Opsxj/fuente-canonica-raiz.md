# Fuente canónica: raíz del repositorio

La fuente canónica de DocuArchiNet es `D:\imagenesda\DocuachiNet\DocuArchiNet`.

## Alcance operativo

- GitHub Actions valida `tools/opsxj` y `openspec` desde la raíz.
- Las ejecuciones locales de OPSXJ se realizan desde la raíz con `npm.cmd --prefix tools/opsxj run <comando>`.
- La configuración local se guarda en `.env.jira`, archivo ignorado por Git.
- La carpeta `Desarrollo/old/oldanterior/GestionDocumental-Docuarchi.net` se conserva temporalmente como copia histórica; no es fuente de CI ni de nuevas operaciones.

## Dependencias de desarrollo

Para reproducir la validación de CI desde la raíz:

```powershell
npm.cmd --prefix tools/opsxj ci
npm.cmd --prefix tools/opsxj test
```

`node_modules` es local y no se versiona.

## Retiro de la copia histórica

No eliminar `Desarrollo` hasta que se cumplan todos estos puntos:

1. La CI de la raíz haya validado correctamente el flujo migrado.
2. IIS local y cualquier perfil de depuración apunten a la raíz.
3. Las operaciones activas de OPSXJ y OpenSpec se hayan realizado desde la raíz.
4. Se conserve una referencia histórica de las evidencias que mencionan la ruta anterior.

Las referencias en `openspec/changes/archive` y las evidencias de ejecuciones pasadas describen su contexto original y no se reescriben.
