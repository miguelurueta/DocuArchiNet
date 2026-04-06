## Context

El proyecto ya tiene componentes base en `src/app/Components/UI`, incluyendo `AppInput`, `AppButton`, `AppDropdown` y consumidores como `AppTableQueryWrapper`. Actualmente el buscador de `AppTableQueryWrapper` usa `AppInput` directamente y cada consumidor que necesite un campo de busqueda debe repetir contrato, iconografia, accesibilidad y estilos.

`SCRUMCORE-60` formaliza un componente compartido `AppInputSearch` para centralizar ese patron sin cambiar la arquitectura existente ni introducir dependencias nuevas. El componente debe convivir con Ant Design, TypeScript estricto, CSS modules y las pruebas Vitest actuales.

## Goals / Non-Goals

**Goals:**

- Crear un componente UI reutilizable `AppInputSearch` en `src/app/Components/UI/AppInputSearch/`.
- Componer `AppInput` para preservar comportamiento, accesibilidad base y estilos del sistema.
- Definir un contrato claro para valor controlado, cambio de texto, placeholder, estado disabled/error y atributos accesibles.
- Permitir que `AppTableQueryWrapper` use `AppInputSearch` como primer consumidor real sin alterar la logica de paginacion o consulta.
- Cubrir el contrato con pruebas Vitest y referencias `[SPEC:app-input-search]`.

**Non-Goals:**

- No reemplazar todos los buscadores del proyecto en el mismo cambio.
- No cambiar el contrato general de `AppInput` ni migrarlo fuera de Ant Design.
- No agregar debounce obligatorio ni logica de backend; el consumidor conserva la responsabilidad de consultar datos.
- No introducir nuevas dependencias de UI o iconos fuera de las ya usadas en el repo.

## Decisions

1. `AppInputSearch` debe componer `AppInput` en lugar de duplicar Ant Design `Input`.

   Rationale: `AppInput` ya concentra labels, helper text, error state, disabled state, `aria-describedby` y clases base. Componerlo reduce divergencia visual y evita mantener dos wrappers paralelos sobre Ant Design.

   Alternative considered: usar `Input.Search` de Ant Design directamente. Se descarta para evitar saltarse el contrato local de `AppInput` y para mantener un API propio consistente con el sistema UI.

2. El contrato debe ser controlado y compatible con handlers de input.

   Rationale: los consumidores existentes, como `AppTableQueryWrapper`, ya gestionan el estado de busqueda desde un `queryState` externo. El componente debe aceptar `value`, `onChange`, `placeholder`, `aria-label`, `disabled`, `error`, `state`, `className` y props compatibles con input de texto, sin imponer estado interno.

   Alternative considered: exponer solo `onSearchChange(value: string)`. Se descarta como API unica porque obliga a adaptar consumidores que ya trabajan con eventos de input; puede agregarse despues si aparece una necesidad real.

3. La iconografia de busqueda debe ser decorativa y local al componente.

   Rationale: `AppInputSearch` puede usar `SearchOutlined` como prefijo visual si `AppInput` expone el punto de extension necesario, pero el significado accesible debe venir de `aria-label` o `label`. El icono no debe reemplazar el nombre accesible del control.

   Alternative considered: no incluir icono. Es viable, pero el ticket pide un componente de busqueda especializado y el patron visual ayuda a diferenciarlo de un input generico sin acoplarlo a la logica de consulta.

4. `AppTableQueryWrapper` sera el primer consumidor de bajo riesgo.

   Rationale: ya contiene un campo de busqueda basado en `AppInput` y no requiere cambios de datos ni backend. La migracion valida que el nuevo componente funciona en un flujo real y mantiene el contrato `onQueryChange({ search })`.

   Alternative considered: crear solo el componente sin consumidor. Se descarta porque deja el componente sin validacion de integracion real y reduce el valor del ticket.

## Risks / Trade-offs

- Riesgo: duplicar responsabilidades entre `AppInput` y `AppInputSearch`. Mitigacion: limitar `AppInputSearch` a semantica de busqueda y delegar rendering base a `AppInput`.
- Riesgo: cambiar accidentalmente el comportamiento de busqueda de `AppTableQueryWrapper`. Mitigacion: conservar el mismo `value`, `onChange`, placeholder y `aria-label`, y actualizar tests existentes.
- Riesgo: el icono decorativo afecte alineacion o accesibilidad. Mitigacion: usar estilos locales, marcarlo como decorativo y validar con Testing Library por rol/nombre accesible.
- Riesgo: sobre-extender el API desde el primer cambio. Mitigacion: no agregar debounce, clear button o callbacks adicionales hasta que una spec futura lo requiera.

## Migration Plan

- Agregar `AppInputSearch` con su CSS module, barrel export y pruebas unitarias.
- Migrar el buscador de `AppTableQueryWrapper` desde `AppInput` a `AppInputSearch` manteniendo el mismo comportamiento observable.
- Ejecutar pruebas focales del nuevo componente y de `AppTableQueryWrapper`.
- Si hay regresion visual o funcional, revertir el consumo en `AppTableQueryWrapper` manteniendo el componente aislado hasta ajustar el contrato.

## Open Questions

- Si futuros consumidores requieren boton de limpiar o busqueda por Enter, se debe abrir una extension de spec antes de ampliar el contrato.
