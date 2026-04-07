# Arquitectura Técnica: SelectDestinatario reusable

## Objetivo

Definir los requerimientos para extraer `SelectDestinatario` como un control reutilizable dentro de `src/app/Components/UI`, manteniendo la experiencia actual de selección única, tags personalizados, dropdown de información y accesibilidad.

## Contexto

- Actualmente `SelectDestinatario` se define en `RadicacionForm.tsx` como una derivada de `BaseSelectUsuarios` con botones solo de información.
- `RadicacionForm` le pasa props como `opcionesUsuarios`, `abrirInformacion`, validaciones dinámicas, data-ident del formulario y del select, `ariaLabel`, `selectDisabled` y `key` basado en `resetKey` para forzar remount tras limpiar.
- La lógica de búsqueda, apertura manual del dropdown y renderizado de tags ya está encapsulada en `BaseSelectUsuarios` y debe permanecer intacta.

## Requerimientos funcionales

1. El componente reutilizable debe exponer los props necesarios para personalizar:
   - `label`, `name`, `options`, `rules`, `ariaLabel`.
   - Identificadores `formItemDataIdent` y `selectDataIdent` para tracking de QA.
   - `selectDisabled` y `showUserPlusButton`/`allowEdit`/`allowDelete` (configurables).
   - `abrirInformacion` que recibe el id del usuario y dispara el modal de detalles.
2. Debe continuar usando `mode="multiple"` con un array de un solo valor para mantener los tags con el dropdown contextual.
3. Los tags deben renderizar el menú con la acción “Información” y permitir cerrar el select.
4. El componente no debe decidir autocompletar ni consumir APIs; recibe `options` desde el padre.
5. Debe respetar la interfaz de validación de Antd, permitiendo reglas de `Form.Item` con mensajes personalizados.
6. El parent seguirá controlando el `key` que fuerza remount (`resetKey`) y el `selectDisabled` (por restricciones o datos de backend).

## Requerimientos no funcionales

- Reutilizar las clases CSS existentes para cards y spacing; no se cambian estilos de tags.
- Mantener la accesibilidad actual (`aria-label`, placeholder derivado del label).
- Soportar `onSearch` y `onChange` internos sin exponerlos en la API del control; el componente solo informa a través del `Form.Item`.

## Ubicación propuesta

- Nuevo archivo: `src/app/Components/UI/SelectUsuario/SelectUsuario.tsx` (o similar) que exporta `SingleUserSelect`.
- Importar ese componente en `RadicacionForm.tsx` para usarlo en lugar del inline `SelectDestinatario`.

## Pasos sugeridos

1. Extraer `BaseSelectUsuarios` a `src/app/Components/UI/SelectUsuario/BaseSelectUsuario.tsx` y documentar props.
2. Crear `SelectDestinatario` como wrapper que desactiva los botones extra y expone solo props necesarios.
3. Ajustar `RadicacionForm.tsx` para importar el nuevo componente y pasarle las props actuales (rules dinámicas, data-ident, etc.).
4. Validar que el comportamiento del modal de información se mantiene.
5. Agregar pruebas unitarias si existen (o actualizar tests) para asegurar la selección única y el dropdown.

## Consideraciones adicionales

- Si en el futuro otro formulario necesita selección similar (remitente, otro rol), podrá reutilizar la misma API.
- La separación facilita mantener la lógica de tags + dropdown en un solo lugar sin repetir implementaciones en cada formulario.
- Esta estrategia mantiene `RadicacionForm` enfocada en su lógica de estado/formulario y facilita pruebas de UI.
