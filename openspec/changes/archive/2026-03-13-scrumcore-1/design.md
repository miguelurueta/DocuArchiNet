## Context

El ticket `SCRUMCORE-1` requiere crear un boton estandar reusable para la SPA, desacoplando a las vistas consumidoras de `Button` y `Tooltip` de Ant Design sin romper la arquitectura actual. El repositorio ya contiene una carpeta compartida en `src/app/Components/UI`, usa CSS Modules en distintos modulos y no tiene hoy un componente base equivalente para acciones consistentes de UI.

El cambio es transversal porque introduce una nueva pieza de design system que sera consumida progresivamente por distintas vistas. Tambien necesita definir con claridad el contrato publico del componente, su estrategia de estilos, accesibilidad y pruebas para evitar que cada modulo siga resolviendo botones de forma ad-hoc.

## Goals / Non-Goals

**Goals:**
- Crear `AppButton` como abstraccion tipada sobre Ant Design en `src/app/Components/UI/AppButton/`.
- Exponer una API publica propia con `variant`, `size`, `loading`, `disabled`, `htmlType`, `leftIcon`, `rightIcon`, `icon`, `fullWidth` y `tooltip`.
- Soportar `forwardRef<HTMLButtonElement, AppButtonProps>` y preservar compatibilidad con props utiles heredadas de Ant Design.
- Implementar estilos con CSS Modules para mantener consistencia visual enterprise sin acoplar las vistas al detalle visual nativo de Ant Design.
- Garantizar accesibilidad en foco visible, semantica, `aria-disabled` y modo `icon-only` con `aria-label` obligatorio.
- Agregar pruebas con Vitest + Testing Library y documentacion local del componente.

**Non-Goals:**
- Reemplazar en este cambio todos los botones existentes del proyecto.
- Introducir una libreria nueva de design system o tokens globales.
- Acoplar `AppButton` a contextos de dominio o infraestructura como `OperationBlockerContext`.
- Alterar el theme global de Ant Design o la arquitectura SPA.

## Decisions

### Decision 1: Ubicar AppButton en `src/app/Components/UI/AppButton/`
- **Decision:** crear una carpeta dedicada con `AppButton.tsx`, `AppButton.module.css`, `AppButton.test.tsx`, `index.ts` y `README.md`.
- **Rationale:** la carpeta `src/app/Components/UI` ya existe como capa compartida y es el lugar correcto para un componente base no ligado a dominio. La carpeta dedicada mantiene alta cohesion y facilita evolucion, pruebas y documentacion.
- **Alternatives considered:** ubicarlo en un modulo de dominio o como archivo suelto en `src/shared`; se descartan por mezclar responsabilidades o no seguir la estructura visible del repo.

### Decision 2: Componer sobre Ant Design en lugar de reimplementar un boton nativo
- **Decision:** construir `AppButton` como wrapper sobre `Button` y `Tooltip` de Ant Design, usando `forwardRef` y un contrato tipado con `Omit<ComponentProps<typeof AntButton>, ...>` para redefinir la API propia del proyecto.
- **Rationale:** reutiliza accesibilidad, estados visuales y comportamiento maduro de Ant Design, mientras oculta su detalle de implementacion a las vistas consumidoras.
- **Alternatives considered:** renderizar un `<button>` nativo o exponer directamente `Button`; se descartan por perder consistencia con la libreria UI existente o por mantener el acoplamiento actual.

### Decision 3: Separar semantica de negocio visual mediante props propias
- **Decision:** mapear `variant` y `size` a clases CSS Modules y, solo cuando aporte valor, a props internas de Ant Design.
- **Rationale:** `variant` debe representar el design system del proyecto, no el API visual de Ant Design. CSS Modules permiten expresar identidad visual estable sin depender por completo de nombres o variantes externas.
- **Alternatives considered:** usar solo props como `type`, `danger` o `size` de Ant Design; se descarta porque expone detalles de proveedor y limita la evolucion del lenguaje visual propio.

### Decision 4: Resolver `icon-only` y precedencia de iconos en una unica capa
- **Decision:** centralizar en `AppButton` la logica de precedencia entre `icon`, `leftIcon`, `rightIcon` y `children`, considerando `icon-only` cuando exista `icon` sin texto y validando que haya nombre accesible.
- **Rationale:** este comportamiento forma parte del contrato funcional y no debe delegarse a cada vista. Mantenerlo en una sola capa evita inconsistencias de layout y accesibilidad.
- **Alternatives considered:** dejar la responsabilidad a consumidores o soportar combinaciones libres; se descartan por producir variaciones visuales y reglas ambiguas.

### Decision 5: Envolver tooltip de forma segura incluso en estados no interactivos
- **Decision:** cuando exista `tooltip`, renderizar un contenedor envolvente compatible con `Tooltip` para que el contenido siga mostrando ayuda contextual aunque el boton este `disabled` o `loading`.
- **Rationale:** los botones deshabilitados de librerias UI suelen bloquear eventos del mouse; el wrapper evita perder el tooltip y mantiene una experiencia consistente.
- **Alternatives considered:** ocultar tooltip en estado disabled/loading o exigir que cada vista lo resuelva; se descartan por degradar UX y repetir logica.

### Decision 6: Cubrir el contrato con pruebas de comportamiento y accesibilidad
- **Decision:** agregar pruebas unitarias para render, interaccion, bloqueo por `disabled/loading`, `htmlType` por defecto, `aria-disabled`, iconos, `icon-only`, tooltip, variantes, `fullWidth` y tamanos.
- **Rationale:** el componente sera base para otras vistas; una suite de comportamiento evita regresiones tempranas y materializa los escenarios definidos en el spec.
- **Alternatives considered:** snapshots o pruebas superficiales; se descartan por menor valor ante cambios visuales controlados y menor cobertura del contrato real.

## Risks / Trade-offs

- **[Risk]** El mapeo entre props propias y props heredadas de Ant Design puede generar conflictos de tipado o precedencia.  
  **Mitigation:** limitar con `Omit` las props redefinidas y documentar claramente la API publica.
- **[Risk]** Los estilos locales pueden chocar con clases internas de Ant Design.  
  **Mitigation:** usar clases encapsuladas por CSS Modules y apoyarse en la estructura oficial del componente en lugar de overrides fragiles.
- **[Risk]** El soporte de tooltip en botones deshabilitados puede requerir wrappers extra que afecten layout inline.  
  **Mitigation:** usar un contenedor neutro controlado y cubrirlo con pruebas de render y estilos.
- **[Risk]** Forzar `aria-label` solo en runtime puede dejar huecos si el consumidor omite el atributo.  
  **Mitigation:** validar en desarrollo la combinacion `icon-only` y reflejar el requisito tanto en tests como en README.
- **[Risk]** El componente nuevo no garantiza adopcion inmediata en pantallas existentes.  
  **Mitigation:** dejar barrel export y documentacion listos para facilitar migracion progresiva en tickets posteriores.

## Migration Plan

1. Crear la carpeta `src/app/Components/UI/AppButton/` con implementacion, estilos, tests, barrel export y README.
2. Exponer el componente desde la capa UI compartida siguiendo la convencion detectada en el repo.
3. Ejecutar la suite de pruebas del componente y cualquier prueba relacionada que valide integracion basica.
4. Dejar documentado en OpenSpec y README el contrato publico y los patrones de uso.

Rollback:
- Revertir los archivos del componente y sus exports; no hay migracion de datos ni cambios persistentes fuera del frontend.

## Open Questions

- Confirmar si el repositorio ya tiene una preferencia de naming para barrels en `src/app/Components/UI` que deba replicarse a nivel de carpeta superior.
- Confirmar si alguna vista prioritaria debe migrarse en este mismo ticket para demostrar adopcion inicial, o si el alcance termina en la entrega del componente reusable.
- Verificar si el equipo quiere enforcement adicional en desarrollo para `icon-only` sin `aria-label` mediante warning o error explicito.
