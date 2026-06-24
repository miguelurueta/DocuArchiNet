# SCRUMCORE-263 - Pruebas

## Matriz cubierta

| Caso | Cobertura |
| --- | --- |
| Render controlado con `open` | `AppProgressBatch.test.tsx` |
| Cierre con `AppButton`/`AppModal` | `AppProgressBatch.test.tsx` |
| Lista vacia sin `processItem` | `AppProgressBatch.test.tsx` |
| `emptyMessage` | `AppProgressBatch.test.tsx` |
| `autoStart=true` una vez | `AppProgressBatch.test.tsx` |
| `autoStart=false` manual | `AppProgressBatch.test.tsx` |
| Preview de items en cola antes de iniciar | `AppProgressBatch.test.tsx` |
| Orden secuencial | `AppProgressBatch.test.tsx` |
| `success` | `AppProgressBatch.test.tsx` |
| `warning` | `AppProgressBatch.test.tsx` |
| `skipped` | `AppProgressBatch.test.tsx` |
| `controlled-error` con continuar | `AppProgressBatch.test.tsx` |
| `controlled-error` con cancelar | `AppProgressBatch.test.tsx` |
| `fatal-error` | `AppProgressBatch.test.tsx` |
| Excepcion de `processItem` | `AppProgressBatch.test.tsx` |
| Cancelacion con `AbortSignal` | `AppProgressBatch.test.tsx` |
| Resultado tardio tras cancelacion | `AppProgressBatch.test.tsx` |
| Normalizacion 0-100 | `AppProgressBatch.test.tsx` |
| `getItemLabel` | `AppProgressBatch.test.tsx` |
| Resultado invalido | `AppProgressBatch.test.tsx` |
| Exports publicos | `AppProgressBatch.test.tsx` |
| `closeOnComplete` | `AppProgressBatch.test.tsx` |

## Comandos esperados

```txt
npm test -- src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx
npx.cmd tsc --noEmit --pretty false
npx.cmd openspec validate scrumcore-263-crea-componente-appprogressbatch --strict
git diff --check
```

## Evidencia

| Comando | Resultado |
| --- | --- |
| `npx.cmd vitest run src/app/Components/UI/AppProgressBatch/AppProgressBatch.test.tsx` | OK. 19 tests passed. JSDOM reporto avisos no bloqueantes de `getComputedStyle` con pseudo-elementos usados por Ant Design. |
| `npx.cmd tsc --noEmit --pretty false` | OK. Sin errores. |
| `npx.cmd openspec validate scrumcore-263-crea-componente-appprogressbatch --strict` | OK. Change valida. El flush de PostHog fallo por red bloqueada, sin afectar validacion. |
| `git diff --check` | OK. Sin errores de whitespace. Git aviso normalizacion LF/CRLF en `src/app/Components/UI/index.ts`. |
| `npx.cmd eslint src/app/Components/UI/AppProgressBatch src/app/Components/UI/index.ts` | OK. Sin errores en archivos tocados. |
| `npm.cmd run lint` | Falla por deuda existente fuera del ticket: errores previos en `AppDocumentViewerOrchestrator`, `AppEditor`, `AppGuideTour`, `gestionCorrespondencia`, `login`, `radicacion`, entre otros. No corresponde al componente nuevo. |
| `npm.cmd run build` | Falla por errores existentes en `src/modules/digitalizacion`: `ScanProgressSnapshot` no exportado y `DUPLICATE_TIME` no incluido en la union esperada. No corresponde a `AppProgressBatch`. |

## Pruebas manuales recomendadas

1. Abrir modal en estado idle.
2. Ejecutar batch exitoso.
3. Cancelar desde running.
4. Forzar `controlled-error` y continuar.
5. Forzar `controlled-error` y cancelar.
6. Forzar `fatal-error`.
7. Validar labels largos.
8. Validar cierre con teclado durante ejecucion.

## Riesgos no cubiertos automaticamente

- Comportamiento visual exacto de Ant Design en navegador real.
- Integracion futura con consumidores de negocio.
- Rollback de operaciones externas, porque pertenece al consumidor.
