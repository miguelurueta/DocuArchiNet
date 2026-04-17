# Playwright

Espacio reservado para pruebas E2E del frontend con navegador real.

## Alcance esperado

Aqui iran pruebas para:

- login visual
- navegacion
- formularios
- `AppEditor`
- `AppTable`
- flujos de `gestionCorrespondencia`

## Estructura sugerida

```text
playwright/
  auth/
  appeditor/
  correspondencia/
```

## Estado actual

La base ya incluye:

- `playwright.config.ts`
- scripts npm para E2E
- primer smoke spec en `auth/login.smoke.spec.ts`

## Comandos

```bash
npm run test:e2e
```

```bash
npm run test:e2e:headed
```

```bash
npm run test:e2e:ui
```

## Nota

Esto no afecta build, runtime ni funcionalidad del sistema.
