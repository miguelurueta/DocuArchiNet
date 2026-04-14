# Ticket 01 FE: AppEditor Core

## Titulo

Implementar `AppEditor` core con Tiptap MIT y API reusable.

## Objetivo

Crear el componente reusable `AppEditor` usando exclusivamente Tiptap como base,
alineado al comportamiento esperado de `SimpleEditor`, cumpliendo la
arquitectura definida y garantizando desacoplamiento, escalabilidad,
performance y testabilidad.

## Referencias obligatorias

- Arquitectura maestra: `docs/Architecture/AppEditor/AppEditor-Architecture.md`
- Referencia funcional objetivo:

```tsx
import { SimpleEditor } from '@/components/tiptap-templates/simple/simple-editor'

export default function App() {
  return <SimpleEditor />
}
```

## Restricciones obligatorias

- Solo Tiptap.
- Solo extensiones/licenciamiento MIT.
- No usar `any`.
- Sin logica de negocio.
- Tipado estricto.
- No acoplar a modulos consumidores.
- API controlable por props.
- Prohibido usar Tiptap directamente en UI.

## Ubicacion obligatoria

`src/app/Components/UI/AppEditor/`

## Estructura obligatoria

```text
src/app/Components/UI/AppEditor/
  domain/
    editor.types.ts
    editor.model.ts
  application/
    useAppEditor.ts
  infrastructure/
    tiptap.config.ts
    tiptap.extensions.ts
  presentation/
    AppEditor.tsx
    AppEditorToolbar.tsx
```

## Regla arquitectonica obligatoria

Esta prohibido usar Tiptap directamente en `presentation/AppEditor.tsx`.

Distribucion obligatoria:

- `infrastructure` -> configuracion y extensiones de Tiptap.
- `application` -> hook `useAppEditor` con la logica del editor.
- `presentation` -> UI y composicion visual.

`AppEditor` solo debe consumir el hook.

## Contrato obligatorio

```ts
export type AppEditorProps = {
  value?: string;
  onChange?: (value: string) => void;
  placeholder?: string;
  disabled?: boolean;
  readOnly?: boolean;
  label?: string;
  error?: string;
  helperText?: string;
  className?: string;
};
```

## Regla de estado

El componente debe soportar:

- Controlled: `value` + `onChange`.
- Uncontrolled: estado interno.

Debe sincronizar correctamente cambios externos sin:

- perder el cursor;
- romper `undo/redo`.

## Reglas de implementacion

- La base del editor debe construirse con Tiptap a traves de `useAppEditor`.
- La salida del contenido debe ser controlable via `value` y `onChange`.
- El toolbar debe soportar:
  - negrita;
  - cursiva;
  - subrayado;
  - bullet list;
  - ordered list;
  - task list;
  - alineacion izquierda;
  - alineacion centro;
  - alineacion derecha;
  - alineacion justificada;
  - heading dropdown;
  - undo;
  - redo.
- Funcionalidades adicionales obligatorias:
  - UI para agregar y editar enlaces;
  - soporte de carga de imagenes;
  - `disabled` y `readOnly` funcionales;
  - `placeholder` configurable.

## Reglas de performance

- No recrear la instancia de Tiptap en cada render.
- Evitar re-render completo del editor.
- Memoizar toolbar.
- Mantener estabilidad del estado del editor.

## Ejemplo de uso esperado

```tsx
<AppEditor
  value={value}
  onChange={setValue}
  placeholder="Escribe aqui..."
  label="Contenido"
  helperText="Editor enriquecido reusable"
/>
```

## Estrategia de testing

- `presentation`: Testing Library para render e interaccion.
- `application`: pruebas unitarias de `useAppEditor`.
- `infrastructure`: mock de Tiptap.
- No testear implementacion interna de Tiptap.

## Pruebas obligatorias

- Renderiza toolbar y area de edicion.
- Aplica formato `bold`, `italic` y `underline`.
- Soporta `bullet list`, `ordered list` y `task list`.
- Permite cambiar headings.
- Permite alinear texto.
- Ejecuta `undo` y `redo`.
- Inserta y edita enlaces.
- Inserta imagenes.
- Dispara `onChange` con contenido actualizado.

## Pruebas de arquitectura

- `useAppEditor` funciona de forma independiente.
- No existe uso directo de Tiptap en UI.
- No hay perdida de estado en re-render.
- Controlled y uncontrolled funcionan correctamente.

## Criterios de aceptacion

- Componente reusable creado en UI shared.
- Basado 100% en Tiptap.
- API estable y tipada.
- Sin acoplamiento a negocio.
- Cumple Clean Architecture.
- Sin uso directo de Tiptap en UI.
- Soporta controlled y uncontrolled.
- Sin problemas de performance.
- Tests pasando correctamente.
- Comportamiento alineado a `SimpleEditor`.

## Notas de implementacion

- El rol esperado es Frontend Senior con React 19, TypeScript estricto, Clean
  Architecture y testing con Vitest + Testing Library.
- El resultado debe quedar listo para produccion, reutilizacion y
  escalabilidad tipo SaaS.
