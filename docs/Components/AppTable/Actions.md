# Action Layer para Dynamic UI Table

Esta fase agrega la capa transversal de acciones dinámicas para `AppTable` sin contaminar el grid base ni reutilizar concerns de dominio.

## Alcance

- centraliza ejecución HTTP de acciones sobre `clienteApi`
- encapsula la mutación con React Query en `useDynamicUiTableActions`
- resuelve `behavior` y `presentation` como metadata extensible
- construye payloads de ejecución sin mutar metadata
- evalúa disponibilidad de acciones solo con reglas seguras en frontend

## Archivos

- `src/app/Components/UI/AppTable/types/dynamicUiTableAction.types.ts`
- `src/app/Components/UI/AppTable/services/dynamicUiAction.service.ts`
- `src/app/Components/UI/AppTable/hooks/useDynamicUiTableActions.ts`
- `src/app/Components/UI/AppTable/utils/dynamicUiActionPayloadBuilder.ts`
- `src/app/Components/UI/AppTable/utils/dynamicUiActionGuard.ts`
- `src/app/Components/UI/AppTable/utils/dynamicUiActionBehaviorResolver.ts`
- `src/app/Components/UI/AppTable/utils/dynamicUiActionPresentationResolver.ts`

## Separación metadata vs ejecución

- los adapters y contratos de fases previas preservan la metadata de acción
- el service solo ejecuta HTTP y preserva el contrato backend
- el hook solo orquesta la mutación y expone helpers reutilizables
- los utils siguen siendo funciones puras, sin navegación, modales ni render UI

## Payload

El payload builder usa esta precedencia:

1. campos derivados desde fila/selección por `PayloadFields`
2. metadata `request`
3. payload propio de la acción
4. payload manual, que sobrescribe todo lo anterior

También soporta:

- `RowIdField`
- `selectedRowIds`
- fallback a `context.row.id` si no hay `RowIdField`

## Guard y límites frontend

El guard evalúa:

- `RequiredClaimsAny`
- `RequiredClaimsAll`
- `ClaimKey`
- reglas booleanas simples como `visible/isVisible` y `enabled/isEnabled`

No intenta interpretar reglas arbitrarias del backend. Si encuentra reglas no seguras de evaluar en frontend, las reporta en `reasons` y evita inventar semántica.

## Extensibilidad

Los resolvers de `behavior` y `presentation`:

- reconocen los valores conocidos actuales
- preservan el valor original
- mantienen soporte para futuros valores sin enums rígidos
- no ejecutan efectos secundarios

## Preparación para Fase 4

Esta fase deja listo:

- servicio reusable con endpoint default e inyectable
- hook reusable para contenedores
- evaluación de disponibilidad
- payload reusable de ejecución
- clasificación de metadata de acciones

Todavía no renderiza toolbar, menús, botones o bulk actions. Esa integración visual queda para la siguiente fase.

## Render visual de cell actions

La fase visual de cell actions agrega un renderer reusable dentro de `AppTable` para columnas marcadas como `isActionColumn`.

Arquitectura:

- `appGridToAppTableColumns.ts` detecta la columna de acción
- el adapter inyecta `cellRenderer` y `cellRendererParams`
- `AppTableActionCellRenderer.tsx` reutiliza `useDynamicUiTableActions`

Flujo del renderer:

1. evaluar disponibilidad con el guard compartido
2. construir payload con el payload builder compartido
3. ejecutar con `executeAction`

El renderer usa `behavior` y `presentation` solo para clasificar metadata. No navega, no abre modales y no dispara descargas reales.

## Soporte mínimo actual

- `Presentation = icon_button`
- render inline de múltiples acciones
- preservación del orden recibido desde backend
- fallback neutro para presentaciones no soportadas

Regla visual:

- `isVisible = false` -> no renderiza la acción
- `isVisible = true` y `isEnabled = false` -> renderiza disabled

## Límites actuales

- `userClaims` depende de que la query dinámica los propague hasta el adapter final de columnas
- `selectedRows` solo se usa si puede derivarse de forma segura desde la selección actual del grid
- presentaciones distintas a `icon_button` todavía no tienen render dedicado

## Menús dinámicos con MenuActions

La integración de menús contextuales ya no humaniza ids. Cuando una acción principal trae `behaviorConfig.menuItems`, `AppTable` resuelve esos ids contra `MenuActions` preservados desde el DTO backend hasta el renderer.

Flujo:

1. `DynamicUiTableDto.MenuActions`
2. normalización compartida de `AppTable`
3. `AppTableActionCellRenderer`
4. `AppDropdown`

Reglas actuales:

- ids no resueltos se ignoran sin romper render
- si no hay items válidos resueltos, se mantiene el fallback de acción directa
- `Children` se mapea recursivamente a `children`
- `IsDivider` se mapea a `type: "divider"` y nunca ejecuta action layer
- solo items resolubles y ejecutables usan `guard`, `payload builder`, `resolvers` y `executeAction`
