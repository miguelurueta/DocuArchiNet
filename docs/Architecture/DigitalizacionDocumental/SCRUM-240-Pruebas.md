# SCRUM-240 - Pruebas

## Unitarias

- `DynamsoftTwainClient.test.ts`
- `loadDynamsoftScripts.test.ts`
- `useDigitalizacionScanner.test.tsx`

## Cobertura funcional

- initialize OK;
- runtime no disponible;
- licencia invalida;
- listado de scanners;
- seleccion de scanner;
- scan OK;
- scan sin scanner bloqueado;
- opciones invalidas;
- remove/rotate/clear;
- PDF vacio;
- PDF valido;
- dispose;
- hook con estados y errores;
- stale initialize ignorado.

## Evidencia

```powershell
npx eslint src/modules/digitalizacion --ext .ts,.tsx
npx vitest run src/modules/digitalizacion
```

Resultado: 30 pruebas OK.

`npm run build` queda bloqueado por errores existentes en `AppEditorToolbar.tsx` no relacionados con esta fase.
