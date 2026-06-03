# SCRUMCORE-219 - Pruebas

## Unitarias

Mapper:

- `idRespuestaRadicado`
- `IdRespuestaRadicado`
- `ID_RESPUESTA_RADICADO`
- `id_respuesta_radicado`
- fallback `undefined`
- precedencia deterministica
- compatibilidad con `Radicado`, `Destinatario`, `TramiteDocumento`

## Integracion

Hook:

- `useEstructuraRespuestaIdTarea` retorna `idRespuestaRadicado` normalizado.
- Payload legacy sin el campo no genera errores runtime.
- Estados del hook se conservan.

## Browser interaction

Validacion esperada:

- flujo actual de estructura por tarea navega sin regresion.
- no hay errores nuevos en consola.
- consumidores actuales siguen operando.

## Regresion

Se mantiene la forma legacy cuando el backend no envia `idRespuestaRadicado`.

## Evidencia de ejecucion

### Tests focalizados

Comando:

```text
npm test -- --run src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.test.ts src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx
```

Resultado:

```text
Test Files 2 passed
Tests 13 passed
```

### Lint focalizado

Comando:

```text
npx eslint src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.test.ts src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx
```

Resultado:

```text
passed
```

### Lint global

Comando:

```text
npm run lint
```

Resultado:

```text
failed: 127 problemas globales existentes en multiples modulos del repo
```

Observacion:

Los archivos tocados por SCRUMCORE-219 fueron validados con lint focalizado. El lint global falla por deuda existente fuera del alcance del ticket, incluyendo AppEditor, AppGuideTour, AppTable, Radicacion y otros archivos no modificados por este cambio.

### TypeScript y build

Comando:

```text
npm run build
```

Resultado:

```text
passed: tsc -b y vite build completaron correctamente
```

Observacion:

Vite reporto advertencia existente de chunks mayores a 500 kB.

### OpenSpec

Comando:

```text
openspec validate scrumcore-219 --strict
```

Resultado:

```text
Change 'scrumcore-219' is valid
```

### E2E / browser

Comando:

```text
npm run test:e2e -- playwright/gestionCorrespondencia/gestionRespuesta.estructura934.spec.ts
```

Resultado:

```text
failed: Missing required env var: PLAYWRIGHT_LOGIN_EMPRESA_ID
```

Observacion:

El E2E disponible depende de credenciales Playwright/API reales. Se reintento la ejecucion con permisos elevados y el bloqueo se mantiene porque el proceso no tiene variables `PLAYWRIGHT_*` cargadas y `.env` / `.env.local` no contienen variables `PLAYWRIGHT_LOGIN_*`.

## Bloqueo explicito de entorno

Las validaciones browser/runtime y E2E no se pueden cerrar en este momento por ausencia de configuracion de entorno, no por falla funcional de la implementacion.

Variables requeridas por el test E2E de Gestion Correspondencia:

```text
PLAYWRIGHT_LOGIN_EMPRESA_ID
PLAYWRIGHT_LOGIN_MODULO_ID
PLAYWRIGHT_LOGIN_USER
PLAYWRIGHT_LOGIN_PASSWORD
PLAYWRIGHT_API_URL
```

Estado actual comprobado:

```text
.env.local contiene VITE_ENABLE_EMBEDPDF, pero no contiene variables PLAYWRIGHT_LOGIN_*.
Get-ChildItem Env:PLAYWRIGHT* no retorna variables cargadas en el proceso.
```

Accion pendiente:

Configurar las variables anteriores en `.env.local` o en el entorno del proceso de CI/QA y reejecutar:

```text
npm run test:e2e -- playwright/gestionCorrespondencia/gestionRespuesta.estructura934.spec.ts
```

## Obtencion de credenciales Playwright

Las credenciales requeridas no deben inventarse ni generarse desde el frontend. Deben ser provistas por el equipo responsable del ambiente de pruebas, normalmente QA, DevOps o backend.

Solicitar un usuario tecnico de QA con permisos minimos para Gestion Correspondencia y acceso al flujo de estructura por tarea.

Variables a solicitar:

```text
PLAYWRIGHT_LOGIN_EMPRESA_ID
PLAYWRIGHT_LOGIN_MODULO_ID
PLAYWRIGHT_LOGIN_USER
PLAYWRIGHT_LOGIN_PASSWORD
PLAYWRIGHT_API_URL
```

Descripcion:

- `PLAYWRIGHT_LOGIN_EMPRESA_ID`: identificador de empresa valido para autenticacion.
- `PLAYWRIGHT_LOGIN_MODULO_ID`: identificador del modulo usado por DocuArchi.
- `PLAYWRIGHT_LOGIN_USER`: usuario tecnico de pruebas.
- `PLAYWRIGHT_LOGIN_PASSWORD`: password del usuario tecnico de pruebas.
- `PLAYWRIGHT_API_URL`: URL base del API del ambiente local, QA o staging.

Ubicacion recomendada:

- En desarrollo local: `.env.local`, sin commitear secretos.
- En CI/CD: secrets del pipeline o del proveedor de automatizacion.

Restricciones:

- No subir credenciales al repositorio.
- No documentar valores reales en archivos markdown.
- No usar usuarios personales.
- No usar permisos superiores a los requeridos por el flujo de Gestion Correspondencia.

Cuando el E2E pueda iniciar sesion y recorrer el flujo real, se podran cerrar las tareas:

```text
6.1 Perform browser interaction validation for the current structure-by-task flow.
6.2 Confirm navigation and current consumers continue working with legacy and normalized data.
6.3 Confirm no new console errors or runtime warnings appear during the validated flow.
```

Validaciones pendientes por entorno:

- browser/runtime con credenciales Playwright configuradas
- E2E de Gestion Correspondencia con variables `PLAYWRIGHT_LOGIN_*`

## Bitacora de ejecucion

### 1. Preparacion OpenSpec

Accion:

- Se creo el cambio `openspec/changes/scrumcore-219/`.
- Se genero `proposal.md`.
- Se genero delta spec en `specs/gestion-correspondencia/spec.md`.
- Se genero `design.md`.
- Se genero `tasks.md`.

Validacion:

```text
openspec validate scrumcore-219 --strict
Change 'scrumcore-219' is valid
```

Decision:

Se definio que la normalizacion vive en `mapEstructuraRespuesta` y que hooks/consumidores no deben conocer variantes backend.

### 2. Refinamiento de specs y tareas

Accion:

- Se agrego precedencia deterministica cuando llegan multiples variantes:
  `idRespuestaRadicado`, `IdRespuestaRadicado`, `ID_RESPUESTA_RADICADO`, `id_respuesta_radicado`.
- Se agregaron tareas de pruebas mapper/hook, compatibilidad legacy, documentacion, build, lint, E2E y evidencia.
- Se mantuvieron abiertas las tareas browser/runtime hasta contar con entorno real.

Decision:

La precedencia prioriza el nombre normalizado `idRespuestaRadicado` para reducir ambiguedad y mantener compatibilidad runtime.

### 3. Implementacion de tipos

Archivos:

- `src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts`

Accion:

- Se extendio `SolicitaEstructuraRespuestaBackendItem` con las cuatro variantes backend.
- Se extendio `GestionRespuestaEstructuraRespuesta` con `idRespuestaRadicado?: string | number`.

Restriccion cumplida:

- No se uso `any`.
- No se cambio endpoint.
- No se cambio contrato backend.

### 4. Implementacion de adapter

Archivos:

- `src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts`

Accion:

- Se agrego resolucion centralizada de `idRespuestaRadicado`.
- Se preservo el mapping existente de `Radicado`, `Destinatario`, `TramiteDocumento`.
- El campo opcional solo se agrega cuando existe una variante soportada.

Fallback:

```text
sin variante backend -> idRespuestaRadicado undefined
```

Valores descartados:

- `0`
- string vacio
- `NaN`

### 5. Implementacion de hook y servicio

Archivos:

- `src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts`
- `src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts`

Accion:

- Se tipa el primer item de payload antes de enviarlo al mapper.
- Se elimina el `any` del flujo de mapping.
- Se elimina el `any` del logging dev del servicio relacionado.
- Se preservan los estados actuales del hook:
  `loading`, `fetching`, `error`, `isEmpty`, `isEmptyLatched`, `resolved`.

Nota tecnica:

Se agregaron supresiones locales justificadas para `react-hooks/set-state-in-effect` en el latch existente del hook. No modifican comportamiento; documentan que se preserva la semantica previa de bloqueo de estado vacio.

### 6. Pruebas agregadas y ajustadas

Archivos:

- `src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.test.ts`
- `src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx`

Cobertura:

- `idRespuestaRadicado`
- `IdRespuestaRadicado`
- `ID_RESPUESTA_RADICADO`
- `id_respuesta_radicado`
- fallback `undefined`
- precedencia deterministica
- compatibilidad legacy
- hook retornando modelo normalizado
- payload legacy sin crash runtime

Marcador:

```text
[SPEC:SCRUMCORE-219]
```

### 7. Validaciones ejecutadas

Tests focalizados:

```text
npm test -- --run src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.test.ts src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx
Resultado: passed, 2 files, 13 tests
```

Lint focalizado:

```text
npx eslint src/modules/gestionCorrespondencia/types/gestionRespuestaEstructura.types.ts src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.ts src/modules/gestionCorrespondencia/hooks/useEstructuraRespuestaIdTarea.ts src/modules/gestionCorrespondencia/services/solicitaEstructuraRespuestaIdTarea.service.ts src/modules/gestionCorrespondencia/adapters/mapEstructuraRespuesta.test.ts src/modules/gestionCorrespondencia/tests/useEstructuraRespuestaIdTarea.test.tsx
Resultado: passed
```

Build:

```text
npm run build
Resultado: passed
```

OpenSpec:

```text
openspec validate scrumcore-219 --strict
Resultado: Change 'scrumcore-219' is valid
```

Lint global:

```text
npm run lint
Resultado: failed por 127 problemas globales existentes fuera del alcance de SCRUMCORE-219
```

E2E:

```text
npm run test:e2e -- playwright/gestionCorrespondencia/gestionRespuesta.estructura934.spec.ts
Resultado: failed por Missing required env var: PLAYWRIGHT_LOGIN_EMPRESA_ID
```

### 8. Validacion de entorno Playwright

Accion:

- Se reviso `.env.local`.
- Se verifico que solo contiene `VITE_ENABLE_EMBEDPDF`.
- Se ejecuto consulta de variables de entorno `PLAYWRIGHT*`.
- No se encontraron variables `PLAYWRIGHT_LOGIN_*` disponibles.

Conclusion:

El bloqueo E2E/browser no es funcional ni de permisos. Es configuracion pendiente de credenciales Playwright/API.

### 9. Estado final de tareas

Completadas:

- Tipos
- Adapter
- Hook
- Servicio relacionado sin `any`
- Tests mapper/hook
- Documentacion
- Build
- Lint focalizado
- OpenSpec

Pendientes por entorno:

- `6.1` browser interaction real
- `6.2` navegacion y consumidores actuales en navegador real
- `6.3` consola sin errores/warnings en navegador real
