# k6 Performance

Esta carpeta contiene pruebas de carga y smoke para APIs/backend del proyecto.

## Alcance

`k6` aqui se usa solo para:

- endpoints HTTP del backend
- latencia
- throughput
- tasa de error
- smoke checks y pruebas de carga basicas

No se usa para:

- `AppEditor`
- Tiptap / ProseMirror
- paginacion visual
- zoom
- IndexedDB
- interacciones reales de navegador

Para esas capas corresponde usar pruebas de UI o browser automation.

## Estructura

```text
performance/k6/
  smoke/
  load/
  config/
  utils/
```

## Variables de entorno

Las pruebas usan variables `__ENV` de `k6`.

Variables principales:

- `API_URL`
- `API_TOKEN` opcional
- `LOGIN_EMPRESA_ID`
- `LOGIN_MODULO_ID`
- `LOGIN_USER`
- `LOGIN_PASSWORD`
- `APP_TABLE_ENDPOINT` opcional
- `APP_TABLE_ID` opcional

Archivo base sugerido:

- `performance/k6/.env.example`

Ejemplo:

```bash
k6 run -e API_URL=http://localhost:8080 performance/k6/smoke/workflow-inbox-autocomplete.k6.js
```

Con token:

```bash
k6 run -e API_URL=https://api.qa.midominio.com -e API_TOKEN=tu_token performance/k6/load/workflow-inbox-autocomplete.k6.js
```

## Scripts incluidos

- `smoke/workflow-inbox-autocomplete.k6.js`
  valida el endpoint `/api/workflowInboxgestion/inboxgestion/autocomplete`
- `load/workflow-inbox-autocomplete.k6.js`
  aplica una carga inicial moderada al mismo endpoint
- `smoke/login.k6.js`
  valida el endpoint `/api/accout/ValidaUserAplicacion`
- `smoke/renew-token.k6.js`
  valida el endpoint `/api/auth/renew`
- `smoke/app-table.k6.js`
  valida el endpoint base de `AppTable`
- `load/app-table.k6.js`
  aplica carga inicial moderada al endpoint base de `AppTable`

## Recomendaciones

- correr contra backend de desarrollo o QA, no contra produccion sin plan
- versionar thresholds y escenarios junto con los scripts
- no hardcodear secretos
- usar payloads realistas por endpoint

## Ejemplos

Smoke login:

```bash
k6 run ^
  -e API_URL=http://localhost:8080 ^
  -e LOGIN_EMPRESA_ID=1 ^
  -e LOGIN_MODULO_ID=1 ^
  -e LOGIN_USER=admin ^
  -e LOGIN_PASSWORD=secret ^
  performance/k6/smoke/login.k6.js
```

Smoke renew:

```bash
k6 run ^
  -e API_URL=http://localhost:8080 ^
  -e API_TOKEN=tu_token ^
  performance/k6/smoke/renew-token.k6.js
```

Load AppTable:

```bash
k6 run ^
  -e API_URL=http://localhost:8080 ^
  -e API_TOKEN=tu_token ^
  -e APP_TABLE_ID=inbox-gestion ^
  performance/k6/load/app-table.k6.js
```

## Uso recomendado

1. copiar `performance/k6/.env.example`
2. llenar valores reales del entorno
3. exportar variables o pasarlas con `-e`
4. ejecutar primero `smoke`
5. ejecutar `load` solo cuando el contrato del endpoint ya este validado

## Proximo paso recomendado

Agregar suites equivalentes para:

- login / renovacion de token
- AppTable / dynamic-ui-table
- radicacion
- recovery / OTP
