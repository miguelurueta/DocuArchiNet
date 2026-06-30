# SCRUMCORE-271 - Pruebas y verificacion

## TypeScript

```txt
npx.cmd tsc -p tsconfig.app.json --noEmit
```

Resultado:

```txt
OK
```

## Suite focal del modulo

```txt
npx.cmd vitest run src/modules/almacenamientoDocumental --environment jsdom
```

Resultado:

```txt
7 test files passed
37 tests passed
```

Cobertura focal:

- storage client SCRUMCORE-272;
- utilidades de archivo existentes;
- sugerencia de tipologia;
- mapper de registro de interfaz;
- hook de estado;
- hook de acciones;
- componente `AppUploadDocumental`.

## Tests especificos SCRUMCORE-271

```txt
npx.cmd vitest run src/modules/almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.test.tsx src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalState.test.ts src/modules/almacenamientoDocumental/components/AppUploadDocumental/hooks/useAppUploadDocumentalActions.test.ts src/modules/almacenamientoDocumental/services/uploadDocumentalInterfaceRegistration.mapper.test.ts src/modules/almacenamientoDocumental/utils/tipoDocumentalSuggestion.utils.test.ts --environment jsdom
```

Resultado:

```txt
5 test files passed
15 tests passed
```

## OpenSpec

```txt
npx.cmd openspec validate scrumcore-271-crea-componente-appuploaddocumental --strict
```

Resultado:

```txt
Change 'scrumcore-271-crea-componente-appuploaddocumental' is valid
```

El warning de PostHog corresponde a telemetria bloqueada por red/sandbox; no invalida OpenSpec.

## Busqueda de prohibidos

Se reviso codigo productivo del modulo contra:

```txt
.ashx
XMLHttpRequest
FormData
jquery
jQuery
clienteApi
fetch(
: any
as any
<any>
```

Resultado: no hay uso productivo. Los matches restantes estan en documentacion, explicando exclusiones.

## Verificacion navegador/manual

No se ejecuto flujo navegador/manual de cinco archivos porque el repo no contiene una pantalla consumidora montada con:

- loaders reales de config;
- loaders reales de tipologias;
- fixtures PDF/imagen;
- simulador de error por tamano;
- simulador de error storage/retry.

La deuda quedo documentada en el README del componente y en tasks OpenSpec.
