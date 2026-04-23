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

## Ejemplo `progress`

```tsx
<AppSteps
  variant="progress"
  current={2}
  progressPercent={67}
  items={[
    { key: "1", title: "Creado", status: "finish" },
    { key: "2", title: "Revision", status: "process" },
    { key: "3", title: "Aprobacion", status: "wait" },
  ]}
/>
```

## Ejemplo `timeline`

```tsx
<AppSteps
  variant="timeline"
  items={[
    {
      key: "t1",
      title: "Radicado recibido",
      description: "Ingreso en ventanilla unica",
      timestamp: "2026-04-23 08:34",
      status: "finish",
    },
    {
      key: "t2",
      title: "Asignado a analista",
      timestamp: "2026-04-23 09:10",
      status: "process",
    },
  ]}
/>
```
