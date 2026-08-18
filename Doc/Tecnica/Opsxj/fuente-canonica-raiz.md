# Fuente canónica: raíz del repositorio

La fuente canónica de DocuArchiNet es `D:\imagenesda\DocuachiNet\DocuArchiNet`.

## Alcance operativo

- GitHub Actions valida `tools/opsxj` y `openspec` desde la raíz.
- Las ejecuciones locales de OPSXJ se realizan desde la raíz con `npm.cmd --prefix tools/opsxj run <comando>`.
- La configuración local se guarda en `.env.jira`, archivo ignorado por Git.
- La copia histórica que estaba en `Desarrollo` fue movida fuera del repositorio a `D:\imagenesda\CopiaDocuArchinet\Desarrollo`; no es fuente de CI ni de nuevas operaciones.

## Dependencias de desarrollo

Para reproducir la validación de CI desde la raíz:

```powershell
npm.cmd --prefix tools/opsxj ci
npm.cmd --prefix tools/opsxj test
```

`node_modules` es local y no se versiona.

## Respaldo histórico externo

`Desarrollo` ya no hace parte del árbol de trabajo. Su respaldo local se conserva fuera del repositorio en `D:\imagenesda\CopiaDocuArchinet\Desarrollo` y no se versiona ni participa en las operaciones de la raíz.

Si se requiere recuperarlo, muévalo de nuevo al directorio raíz antes de usarlo.

Las referencias en `openspec/changes/archive` y las evidencias de ejecuciones pasadas describen su contexto original y no se reescriben.
