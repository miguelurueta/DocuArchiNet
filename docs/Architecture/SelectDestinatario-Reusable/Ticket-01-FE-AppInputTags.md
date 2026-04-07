# PROMPT ARQUITECTÓNICO  Ticket 01 FE
# Implementar AppInputTags core (UI + eventos + estilos)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Construir `AppInputTags` como control reusable en `src/app/Components/UI/AppInputTags/`, basado en `AutoComplete` + `Input` de Ant Design, con semántica controlada de eventos (debounce, minLength, Enter, click en icono), estados de loading, accesibilidad y variantes de tamaño alineadas a `AppInput`. Garantizar la adición y eliminación manual de tags sin depender de `KeyPress`.


CONTEXTO EXISTENTE

- especificación inicial: `docs/Architecture/SelectDestinatario-Reusable/AppInputTags-reqs.md`
- estilos base: `src/app/Components/UI/AppInput`
- flujo actual descrito en `RadicacionForm.tsx` usa `BaseSelectUsuarios` para tags con menú contextual


UBICACIÓN (OBLIGATORIA)

```
src/app/Components/UI/AppInputTags/
```


RESTRICCIONES (OBLIGATORIAS)

- no consumir APIs dentro del componente
- no acoplar a módulos o pantallas específicas
- no usar `Input.Search` de AntD ni `Tag` con `KeyPress`
- no bloquear input durante loading
- no introducir estilos globales ni romper la consistencia visual con `AppInput`
- garantizar accesibilidad desde el componente (aria-labels, keyboard navigation)


CONTRATO (OBLIGATORIO)

type AppInputTagsProps = {
  name: string;
  label: React.ReactNode;
  mode?: "single" | "multiple";
  value?: string[];
  defaultValue?: string[];
  options?: { label: string; value: string }[];
  rules?: Rule[];
  minLength?: number;
  debounceMs?: number;
  loading?: boolean;
  toolbar?: {
    render: () => React.ReactNode;
  };
  onAddTag: (tag: string) => void;
  onRemoveTag: (tag: string) => void;
  onRemoveAll: () => void;
  onSearch: (query: string) => void;
  abrirInformacion: (id: number) => void;
  clearOnEscape?: boolean;
  selectDisabled?: boolean;
  size?: "sm" | "md" | "lg";
  formItemDataIdent?: string;
  selectDataIdent?: string;
};


REGLAS DE IMPLEMENTACIÓN (OBLIGATORIAS)

1. CONTROLADO VS NO CONTROLADO
   - si se provee `value`, el componente es controlado; `defaultValue` solo aplica cuando no hay `value`
   - no mezclar ambos modos

2. AGREGAR TAGS
   - exponer `onAddTag` para confirmación manual (botón, dropdown, autocomplete)
   - no depender de `KeyPress` para agregar etiquetas
   - en modo `single`, reemplazar el tag actual

3. ELIMINAR TAGS
   - `onRemoveTag` se invoca desde botones/dropdown/menú contextual
   - `onRemoveAll` borra toda la lista sin `KeyPress`

4. EVENTOS DE BÚSQUEDA
   - `onSearch` se dispara por:
       * Enter (sin debounce)
       * Click en icono de búsqueda (sin debounce)
       * Debounce tras escritura (según `debounceMs`)
   - Enter/click deben cancelar cualquier debounce pendiente

5. VALIDACIÓN DE BÚSQUEDA
   - `onSearch` solo se ejecuta si `query.length >= minLength` cuando está definido

6. DEBOUNCE
   - `debounceMs` controla la pausa tras escritura (0 o undefined = sin debounce)
   - solo aplica a escritura, no bloquea Enter/click

7. CLEAR
   - limpiar ejecuta `onChange` con string vacío y `onClear`
   - no dispara `onSearch("")`
   - Escape limpia solo si `clearOnEscape === true`

8. OPTIONS / AUTOCOMPLETE
   - `options` alimenta `AutoComplete`; no debe mutarse
   - mantener navegación por teclado
   - soportar estados vacíos sin errores

9. LOADING
   - input editable y con foco estable
   - spinner visible en suffix/icono sin bloquear eventos

10. ESTILOS
    - border radius 12px, sombra y estados (focus, hover, error, disabled) como `AppInput`
    - variantes `sm | md | lg` que afectan altura, padding e iconos
    - clases sugeridas: `.field`, `.input`, `.inputSm`, `.inputMd`, `.inputLg`, `.icon`, `.iconLoading`

11. ACCESIBILIDAD
    - `aria-label` o `aria-labelledby` configurables
    - botón `Limpiar` con `aria-label="Limpiar"`
    - roles correctos en `AutoComplete` y tags


REGLAS DE CONSISTENCIA

- no incorporar lógica de API
- no introducir side effects externos
- mantener la posibilidad de integrar `AppDropdown` y `AppToolbar`
- conservar la experiencia visual de `AppInput`


RIESGOS A EVITAR (OBLIGATORIO)

- doble ejecución de `onSearch`
- romper la separación controlado/no controlado
- bloqueo del input en loading
- mutar `options`


PRUEBAS UNITARIAS (OBLIGATORIAS)

- `onChange` se ejecuta en cada input
- `onSearch` por Enter, debounced y click
- Enter ignora debounce pendiente
- `minLength` bloquea búsquedas cortas
- `debounceMs = 0` desactiva debounce
- `clear`: dispara `onChange("")`, `onClear()`; no `onSearch("")`
- `Escape`: limpia solo si habilitado
- controlado respeta `value`
- no controlado usa `defaultValue`
- `loading` no bloquea input ni pierde foco
- `options`: renderiza sugerencias sin errores
- `size`: aplica clases correctas
- accesibilidad: `aria-label` presente,y `clear` accesible


PRUEBAS QT (CALIDAD / E2E)

- escribir texto: `debounce` dispara búsqueda
- presionar Enter: búsqueda inmediata sin duplicados
- click en icono: búsqueda inmediata
- texto menor a `minLength`: sin búsqueda
- limpiar con botón: input vacío sin disparar búsqueda
- Escape: limpia solo si habilitado
- navegación por teclado: selección funcional
- loading: input editable
- integración visual: consistente con `AppInput`


CRITERIOS DE ACEPTACIÓN

- componente reusable implementado en `src/app/Components/UI/AppInputTags`
- semántica de eventos determinística
- accesibilidad funcional
- estilos alineados a `AppInput`
- comportamiento controlado/no controlado correcto
- cobertura de pruebas completa (unitarias + QT)
