# PROMPT ARQUITECTONICO  Ticket 02 FE
# UI/UX AppTabs (iconos, badges, variantes, responsive, overflow)

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Implementar UI/UX enterprise de `AppTabs` con iconos, badges, variantes de diseno, tamanos y comportamiento responsive/overflow.


CONTEXTO EXISTENTE

- arquitectura: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- estilos requeridos en CSS Modules (`AppTabs.module.css`)


UBICACION (OBLIGATORIA)

```
src/app/Components/UI/AppTabs/
```


RESTRICCIONES (OBLIGATORIAS)

- no introducir estilos globales
- no romper estilos de AntD fuera del scope
- usar CSS Modules


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. ICONOS + BADGES
   - icono antes del label
   - badge a la derecha con `Badge` de AntD

2. VARIANTES Y TAMANOS
   - `variant`: default | card | underline | pills
   - `size`: sm | md | lg
   - impacta padding, font-size, altura
   - usar design tokens:
     - `--tabs-padding-sm`
     - `--tabs-padding-md`
     - `--tabs-padding-lg`

3. RESPONSIVE
   - desktop: tabs horizontales completos
   - tablet: reduccion de spacing
   - mobile: scroll horizontal (`overflow-x`)
   - no wrap descontrolado

4. OVERFLOW
   - `more` para tabs extra
   - trigger hover, label "Mas" + contador (+N)
   - dropdown alineado a la derecha

5. FEEDBACK DISABLED
   - menor opacidad, cursor not-allowed, sin hover

6. PERFORMANCE (UI)
   - memo de items visuales
   - evitar re-render de children si no cambia activeKey


PRUEBAS UNITARIAS (OBLIGATORIAS)

- renderiza iconos y badges
- aplica clase `customTabs`
- estado visual disabled


PRUEBAS QT (CALIDAD / E2E)

- responsive correcto por breakpoint
- overflow con dropdown


CRITERIOS DE ACEPTACION

- UI enterprise consistente
- variantes y tamanos funcionales
- responsive y overflow sin romper layout
