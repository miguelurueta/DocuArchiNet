# Ticket 02 FE

## Titulo

Implementar UI/UX, responsive y tamaños de `AppInputSelect`

## Objetivo

Construir la capa visual real de `AppInputSelect` para que conserve la
apariencia nativa de Ant Design, incluya estados vacios y de carga consistentes,
respete un `border-radius` leve y moderno, y mapee sus tamaños al lenguaje de
`AppButton`. El entregable de esta FE es la implementación visual del
componente reusable, no una documentación aislada.

## Contexto existente

- Arquitectura de referencia: `docs/Architecture/AppInputSelect/AppInputSelect-Architecture.md`
- Base visual: `Select`, `Spin`, `Empty` y tokens de Ant Design
- Referencia de sizing: `src/app/Components/UI/AppButton/AppButton.module.css`

## Restricciones (obligatorio)

- No rehacer visualmente Ant Design desde cero
- CSS Modules solo para ajustes compartidos mínimos
- Mantener hover, focus y status de Ant Design
- El empty state debe sentirse nativo de Ant Design

## Ubicacion (obligatoria)

```txt
src/app/Components/UI/AppInputSelect/AppInputSelect.module.css
```

## Reglas de UI (obligatorio)

- `size="sm"`: altura compacta alineada a `AppButton` small
- `size="md"`: tamaño base compartido
- `size="lg"`: altura amplia para formularios destacados
- `border-radius` discreto y moderno, con esquinas suaves pero sobrias
- Empty state con `notFoundContent` y `Empty` de Ant Design
- Loading visible en dropdown y/o control principal
- Soporte visual para `status="error"` y `status="warning"`

## Estados visuales obligatorios

- idle
- focused
- disabled
- loading
- empty
- error
- warning
- selected

## Responsive (obligatorio)

Desktop

- ancho configurable por contenedor
- dropdown alineado al ancho del input o mayor si el contenido lo exige

Tablet

- navegable sin overflow visual
- tags y multiple con wrap correcto

Mobile

- area táctil suficiente
- dropdown y opciones legibles
- sin colapsos visuales en labels largos

## Entregables de implementacion

- `AppInputSelect.module.css` con ajustes mínimos sobre Ant Design
- mapeo real de tamaños `sm`, `md`, `lg`
- render real de `notFoundContent` y estado de loading
- comportamiento responsive correcto en desktop, tablet y mobile

## Pruebas visuales obligatorias

- Desktop: control simple, multiple y vacío sin deformaciones
- Tablet: wrap correcto de tags y dropdown usable
- Mobile: control legible, táctil y sin cortes visuales

## Criterios de aceptacion

- Apariencia alineada con Ant Design
- Estados visuales claros y consistentes
- Sizing compartido con la escala `sm/md/lg`
- Empty state y loading sin comportamientos ambiguos
- Responsive correcto dentro del componente real
