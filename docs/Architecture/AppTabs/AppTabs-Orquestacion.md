# PROMPT ARQUITECTONICO  Orquestacion AppTabs

OBJETIVO

Definir la integracion entre:

- Ticket 01 (Core)
- Ticket 02 (UI)
- Ticket 03 (Behavior)

## Matriz de responsabilidades

Ticket 01 (Core)

- contrato unico `AppTabItem` y `AppTabsProps`
- controlado vs no controlado
- bloqueo `disabled` y `beforeChange`
- logica de estado (prioridad: disabled -> beforeChange -> router -> onChange)
- mapping base a AntD Tabs
- internal mapper obligatorio `mapToAntdItems(items)`
- accesibilidad concreta (role tablist, focus programatico)

Ticket 02 (UI)

- estilos enterprise (`customTabs`)
- iconos + badges
- variantes (`variant`) y tamanos (`size`)
- responsive y overflow (more)
- feedback visual disabled
- design tokens `--tabs-padding-sm/md/lg`
- performance UI (memo items, evitar re-render)

Ticket 03 (Behavior)

- sync con router (path + query)
- lazy rendering
- telemetry (`onTabVisible`)
- docs README y ejemplos
- edge cases router (fallback, conflicto, sync inicial vs cambios)
- lazy cacheado (no re-mount)

REGLAS GLOBALES

1. ORDEN DE IMPLEMENTACION
   - Paso 1: implementar Core (Ticket 01)
   - Paso 2: aplicar UI (Ticket 02)
   - Paso 3: integrar Behavior (Ticket 03)

2. FUENTE DE VERDAD DEL ESTADO

- si `syncWithRouter=true`:
  - el router controla `activeKey`

- si `syncWithRouter=false`:
  - comportamiento normal controlado/no controlado

3. CONTRATO UNICO

- `AppTabItem` es la unica fuente de verdad para todos los tickets

4. PRIORIDAD DE LOGICA

1. `disabled`
2. `beforeChange`
3. router (si aplica)
4. `onChange`

5. INTEGRACION UI + BEHAVIOR

- el tab activo debe ser visible incluso en overflow
- en mobile, el scroll debe ajustar automaticamente al tab activo
- lazy rendering no debe romper el layout ni el scroll

6. EDGE CASES GLOBALES

- tab inexistente: fallback a primer tab habilitado
- todos `disabled`: bloquear interaccion
- `items` vacio: estado vacio

7. TESTING INTEGRADO

- validar comportamiento completo:
  - UI + Core + Router + Lazy
  - fallback + conflictos router
  - beforeChange + disabled

## Dependencias entre tickets

- Ticket 02 depende de la estructura base creada en Ticket 01.
- Ticket 03 depende de la API definida en Ticket 01 y estilos base de Ticket 02.
- No se deben introducir props nuevas en Ticket 02/03 sin actualizar Ticket 01.
