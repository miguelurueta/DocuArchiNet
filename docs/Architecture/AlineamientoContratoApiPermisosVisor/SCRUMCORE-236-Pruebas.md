# SCRUMCORE-236 - Pruebas

## Unit tests requeridos

- `AppVisorEmbedPdf.service.test.ts`
- `AppVisorEmbedPdf.permissions.test.ts`

## Casos cubiertos

- Service consulta endpoint correcto.
- Service no envia `idUsuario`.
- Service retorna `data` del envelope.
- Service rechaza `success=false`.
- Service rechaza `Permissions` en raiz.
- Service rechaza ausencia de `data.Permissions`.
- Mapper resuelve `gestioncorrespondencia -> gestion_correspondencia`.
- Mapper usa `pdf.print`.
- Mapper usa `pdf.download`.
- Mapper usa `pdf.annotate.signature.*`.
- Permisos vacios quedan fail-closed.
- Documento firmado bloquea firma/edicion.

## Validacion manual

Activar:

```js
window.__DV_DEBUG__ = true
```

Abrir documento desde Gestion Correspondencia y verificar:

```text
codigoImpl: gestion_correspondencia
response: data del backend
raw: response.Permissions
effective: permisos mapeados
```

## Comandos sugeridos

```powershell
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.test.ts src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.test.ts
```

```powershell
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.test.tsx
```

```powershell
npx.cmd eslint src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts
```

## Evidencia ejecutada - 2026-06-06

### Tests nuevos de contrato y mapping

```powershell
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.test.ts src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.test.ts
```

Resultado:

- 2 archivos.
- 12 tests.
- 12 passed.
- 0 fallos.

### Regresion focalizada existente del visor

```powershell
npx.cmd vitest run src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.test.tsx src/app/Components/UI/AppVisorEmbedPdf/presentation/AppPdfToolbar.test.tsx
```

Resultado:

- 2 archivos.
- 20 tests.
- 20 passed.
- 0 fallos.

### Lint focalizado

```powershell
npx.cmd eslint src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.permissions.ts src/app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf.service.ts
```

Resultado:

- Pass sin hallazgos.

## Nota de entorno

La primera corrida de tests nuevos dentro del sandbox fallo por resolucion de `setupTests.ts` en `C:/Users/CodexSandboxOffline/...`. La misma corrida fuera del sandbox paso correctamente. No fue un fallo funcional del codigo ni de los tests.

## Evidencia OpenSpec

- Fecha: 2026-06-06.
- Comando: `npx.cmd openspec validate scrumcore-236-alineamiento-api-permisos-appvisorpdf --strict`.
- Resultado funcional: `Change 'scrumcore-236-alineamiento-api-permisos-appvisorpdf' is valid`.
- Nota: el flush de telemetria PostHog fallo por red restringida (`EACCES`) despues de la validacion; no afecta el resultado porque el comando finalizo con codigo 0.
