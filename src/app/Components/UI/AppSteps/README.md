# AppSteps

`AppSteps` es el componente reusable para flujos por etapas. Expone una API tipada y soporta modo controlado/no controlado.

## Importacion

```tsx
import { AppSteps } from "src/app/Components/UI";
```

## Ejemplo `default`

```tsx
<AppSteps
  variant="default"
  current={1}
  onChange={setCurrent}
  items={[
    { key: "a", title: "Inicio", status: "finish" },
    { key: "b", title: "Validacion", status: "process" },
    { key: "c", title: "Cierre", status: "wait" },
  ]}
/>
```

## Ejemplo `form`

```tsx
<AppSteps
  variant="form"
  current={current}
  validateStep={async (step) => validateCurrentStep(step)}
  onChange={setCurrent}
  items={[
    { key: "s1", title: "Datos basicos" },
    { key: "s2", title: "Direccion" },
    { key: "s3", title: "Confirmacion" },
  ]}
/>
```
