# PROMPT ARQUITECTONICO  Ticket 01 FE
# Implementar AppTabs core (wrapper + contrato controlado)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Construir `AppTabs` como wrapper desacoplado de Ant Design `Tabs` en `src/app/Components/UI/AppTabs/`, con contrato tipado estricto, soporte controlado/no controlado y bloqueo por `disabled`.


CONTEXTO EXISTENTE

- especificacion principal: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- estilos base: `src/app/Components/UI` (convencion CSS Modules)


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppTabs/
```


RESTRICCIONES (OBLIGATORIAS)

- no consumir APIs dentro del componente
- no acoplar a modulos o pantallas especificas
- no introducir estilos globales
- no usar `any`
- no romper estilos globales de AntD


CONTRATO (OBLIGATORIO)

type AppTabItem = {
  key: string;
  label: ReactNode;
  children: ReactNode;
  icon?: ReactNode;
  badge?: number;
  disabled?: boolean;
};

type AppTabsProps = ComponentProps<typeof Tabs> & {
  items: AppTabItem[];
  activeKey?: string;
  defaultActiveKey?: string;
  beforeChange?: (nextKey: string, currentKey?: string) => boolean | Promise<boolean>;
  variant?: "default" | "card" | "underline" | "pills";
  size?: "sm" | "md" | "lg";
  more?: TabsProps["more"];
  syncWithRouter?: boolean;
  lazy?: boolean;
  onTabVisible?: (key: string) => void;
};

INTERNAL MAPPER (CRITICO)

Definir mapper interno obligatorio:

function mapToAntdItems(items: AppTabItem[]): TabsProps["items"]

Reglas:
- no mutar `items`
- mapear icono + badge + label


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. CONTROLADO VS NO CONTROLADO
   - si existe `activeKey`, se ignora `defaultActiveKey`
   - no mezclar ambos modos

2. BLOQUEO POR DISABLED
   - tabs con `disabled: true` no permiten click ni teclado
   - `onChange` no se ejecuta si el tab destino esta deshabilitado
   - `activeKey` no cambia si el tab destino esta deshabilitado

3. BEFORE CHANGE
   - `beforeChange` puede bloquear cambio (sync o async)
   - si retorna false, no ejecutar `onChange`

4. TIPADO ESTRICTO
   - extender `ComponentProps<typeof Tabs>` sin perder tipado
   - prohibido usar `any`

5. ACCESIBILIDAD CONCRETA
   - `role=\"tablist\"` en el contenedor principal
   - manejo de focus programatico al cambiar de tab


REGLAS DE CONSISTENCIA

- wrapper estricto sobre AntD Tabs
- no mutar `items`


RIESGOS A EVITAR (OBLIGATORIO)

- mezclar controlado/no controlado
- permitir cambio ignorando `disabled`
- ejecutar `onChange` si `beforeChange` bloquea
- mutar `items` en el mapper interno


PRUEBAS UNITARIAS (OBLIGATORIAS)

- respeta `activeKey` controlado
- `defaultActiveKey` en modo no controlado
- `onChange` no se ejecuta si `disabled`
- `beforeChange` bloquea cambios


PRUEBAS QT (CALIDAD / E2E)

- cambio de tab permitido
- bloqueo por `disabled` y `beforeChange`


CRITERIOS DE ACEPTACION

- componente reusable en `src/app/Components/UI/AppTabs`
- contrato estable y tipado estricto
- bloqueo por disabled y beforeChange funcional
