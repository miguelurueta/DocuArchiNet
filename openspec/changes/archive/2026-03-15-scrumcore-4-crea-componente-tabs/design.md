# Design: app-tabs

## Contexto

`SCRUMCORE-4` introduce un componente `AppTabs` reusable en la capa `src/app/Components/UI` para seguir el mismo patron de abstraccion ya establecido con `AppButton`, `AppInput` y `AppModal`. La meta es encapsular el proveedor UI, exponer una API mas estable para las vistas y centralizar la consistencia visual del control de navegacion por pestañas.

## Decision

Se implementara `AppTabs` como wrapper tipado sobre `Tabs` de Ant Design, ubicado en `src/app/Components/UI/AppTabs/`, con:

- `AppTabsProps` basado en `Omit<ComponentProps<typeof Tabs>, ...>` para controlar las props expuestas y evitar dependencia directa de la API visual completa del proveedor.
- Un tipo `AppTabsItem` propio del proyecto para modelar `key`, `label`, `children`, `disabled` y metadatos visuales necesarios.
- Props de alto nivel como `items`, `activeKey`, `defaultActiveKey`, `onChange`, `orientation`, `variant` y `fullWidth`.
- Mapeo de variantes y orientacion a clases CSS Modules, delegando el comportamiento accesible de tabs al control base de Ant Design.
- Export centralizado desde `src/app/Components/UI/index.ts` y documentacion de uso en `README.md`.

## API propuesta

```ts
type AppTabsItem = {
  key: string;
  label: ReactNode;
  children: ReactNode;
  disabled?: boolean;
};

type AppTabsProps = Omit<
  ComponentProps<typeof Tabs>,
  "items" | "activeKey" | "defaultActiveKey" | "onChange" | "tabPosition"
> & {
  items: AppTabsItem[];
  activeKey?: string;
  defaultActiveKey?: string;
  onChange?: (activeKey: string) => void;
  orientation?: "horizontal" | "vertical";
  variant?: "default" | "card";
  fullWidth?: boolean;
};
```

## Estructura

- `AppTabs.tsx`: wrapper principal y adaptacion de props.
- `AppTabs.module.css`: estilos encapsulados para root, variantes, orientacion y estados.
- `AppTabs.test.tsx`: pruebas focalizadas de contrato y accesibilidad.
- `README.md`: descripcion, API y ejemplos.
- `index.ts`: export local del componente.

## Estilos

Los estilos se implementaran con CSS Modules, evitando sobreescrituras globales. Se usaran clases sobre el contenedor raiz y los semantic slots soportados por `Tabs` para:

- definir espaciados y bordes del tabset;
- reforzar el estado activo y de foco;
- soportar orientacion vertical;
- permitir `fullWidth` cuando la vista necesite que el control ocupe todo el ancho.

## Accesibilidad

Se reutilizara la semantica accesible de Ant Design (`tablist`, `tab`, `tabpanel`) y se verificara por pruebas:

- render de tabs y panel activo;
- imposibilidad de activar tabs deshabilitadas;
- propagacion de `onChange`;
- soporte de navegacion por teclado delegada al control base.

## Riesgos y trade-offs

- Ant Design expone una API amplia para `Tabs`; el wrapper reducira superficie publica, pero deja pasar props adicionales seguras mediante `...restProps`.
- El estilo de tabs puede variar entre versiones del proveedor; por eso el componente debe apoyarse en classes semanticas soportadas y no en selectores fragiles.
- La navegacion por teclado depende del comportamiento del proveedor; las pruebas se enfocaran en el contrato observable del componente.

## Migracion

No requiere migracion inmediata. `AppTabs` quedara disponible para adopcion progresiva en nuevas vistas o refactors de tabs existentes.
