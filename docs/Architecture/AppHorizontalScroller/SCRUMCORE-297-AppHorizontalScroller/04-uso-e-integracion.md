# 04 Uso E Integración

## Import Directo Del Componente

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";
```

## Import Desde Barrel UI

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI";
```

## Rail Básico

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";

export function ExampleRail() {
  return (
    <AppHorizontalScroller ariaLabel="Listado horizontal de acciones">
      <button type="button">Crear</button>
      <button type="button">Editar</button>
      <button type="button">Exportar</button>
    </AppHorizontalScroller>
  );
}
```

## Rail Compacto Con Snap

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";

export function ExampleRail() {
  return (
    <AppHorizontalScroller
      ariaLabel="Listado horizontal de elementos"
      density="compact"
      gap="sm"
      itemMinWidth={220}
      itemMaxWidth={280}
      scrollSnap="start"
      edgeFade
    >
      <button type="button">Elemento 1</button>
      <button type="button">Elemento 2</button>
      <button type="button">Elemento 3</button>
    </AppHorizontalScroller>
  );
}
```

## Rail Con Items De Dominio

```tsx
import { AppHorizontalScroller } from "../../../../app/Components/UI/AppHorizontalScroller";

type SummaryItem = {
  id: string;
  title: string;
  description: string;
};

export function SummaryRail({ items }: { items: SummaryItem[] }) {
  return (
    <AppHorizontalScroller
      ariaLabel="Resumen horizontal"
      density="compact"
      gap="sm"
      itemMinWidth={220}
      itemMaxWidth={320}
      edgeFade
    >
      {items.map((item) => (
        <article key={item.id}>
          <strong>{item.title}</strong>
          <p>{item.description}</p>
        </article>
      ))}
    </AppHorizontalScroller>
  );
}
```

Este ejemplo ilustra composición; el componente de dominio sigue siendo responsable de obtener datos, manejar estados y definir acciones.

## Integración Futura Con SCRUM-162

```txt
GestionRespuestaAdjuntosRespuestaRail
  - obtiene idRespuestaRadicado desde contexto
  - llama service/hook SCRUM-162
  - maneja loading/empty/error/success
  - renderiza tarjetas de adjuntos
        |
        | children
        v
AppHorizontalScroller
  - layout horizontal
  - accesibilidad base
  - scroll en X
```

Regla: SCRUM-162 no debe modificar `AppHorizontalScroller` para agregar conocimiento de documentos. Debe componerlo desde un componente de dominio.

`AppHorizontalScroller` no debe conocer:

- `idRespuestaRadicado`
- endpoint SCRUM-162
- DTOs documentales
- estados de carga de adjuntos
- reglas de seguridad documental
