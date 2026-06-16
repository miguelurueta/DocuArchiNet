# SCRUMCORE-251 - Pruebas

## Pruebas automatizadas ejecutadas

```powershell
npx.cmd vitest run src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.test.tsx src/modules/gestionCorrespondencia/pages/GestionRespuesta.test.tsx
```

Resultado:

- 2 archivos de test OK.
- 4 tests OK.

Cobertura validada:

- `GestionWorkbenchParallelTabs` renderiza `Gestion` y `Documentos` simultaneamente.
- Los paneles tienen labels accesibles.
- Existe divisor accesible.
- `GestionRespuesta` inicia en modo tabs.
- El boton `Vista paralela` activa modo paralelo.
- El boton `Vista normal` vuelve a modo tabs.
- El switch de vista paralela refleja el estado con `aria-pressed` y `aria-checked`.
- `data-layout-state` refleja estado visual `inactive` / `active`.
- En ancho reducido el boton queda deshabilitado.

## Build

```powershell
npm.cmd run build
```

Resultado: bloqueado por error TypeScript preexistente fuera del alcance:

```txt
src/modules/digitalizacion/infrastructure/dynamsoft/DynamsoftTwainClient.ts(908,42):
error TS2552: Cannot find name 'DynamsoftWebTwainFactory'. Did you mean 'DynamsoftWebTwainObject'?
```

Despues de corregir los errores propios de `GestionWorkbenchParallelTabs`, el build ya no reporta errores del cambio SCRUMCORE-251.

## OpenSpec

```powershell
npx.cmd openspec validate scrumcore-251-tabs-workbench-gestion-correspondencia --strict
```

Resultado:

- Change valido.
- La salida muestra errores de telemetria PostHog por red/sandbox; no invalidan el resultado OpenSpec.

## QA manual pendiente

- Abrir Gestion Correspondencia.
- Entrar a un tramite con tabs `Gestion` y `Documentos`.
- Verificar modo normal.
- Activar `Vista paralela`.
- Confirmar que ambos paneles se ven simultaneamente.
- Redimensionar paneles.
- Seleccionar un documento.
- Volver a modo normal.
- Confirmar que no se pierde contexto.
- Probar en ancho reducido.

## Riesgo residual

El build general queda condicionado por la deuda TypeScript existente en el modulo de digitalizacion. Esta deuda no fue introducida por SCRUMCORE-251.
