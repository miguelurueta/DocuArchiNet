## Context

El ticket `SCRUMCORE-3` requiere crear un modal estandar reusable para la SPA, desacoplando a las vistas consumidoras del componente de dialogo del proveedor UI y preservando consistencia visual, semantica y de accesibilidad. El repositorio ya dispone de una capa compartida en `src/app/Components/UI`, usa CSS Modules en varios modulos y no tiene hoy un componente base equivalente para overlays y dialogos reutilizables.

El cambio es transversal porque introduce una pieza del design system que puede ser adoptada por confirmaciones, formularios modales y overlays en distintos modulos. Tambien necesita fijar un contrato publico claro para apertura, cierre, contenido, acciones, estados visuales y accesibilidad antes de futuras adopciones.

## Goals / Non-Goals

**Goals:**
- Crear `AppModal` como abstraccion tipada sobre el componente de dialogo base de la libreria UI en `src/app/Components/UI/AppModal/`.
- Exponer una API publica propia con control de apertura, titulo, contenido, acciones primarias/secundarias y callbacks de cierre.
- Mantener compatibilidad con props utiles heredadas del proveedor UI sin exponer directamente su semantica visual a las vistas.
- Implementar estilos con CSS Modules para alinear overlay, cabecera, cuerpo y footer con una apariencia enterprise coherente con la UI actual.
- Garantizar accesibilidad en foco inicial, titulo asociado, cierre por teclado y semantica de dialogo.
- Agregar pruebas con Vitest + Testing Library y documentacion local del componente.

**Non-Goals:**
- Migrar en este ticket todos los modales existentes del proyecto.
- Introducir un framework nuevo de dialogos o manejar reglas de negocio especificas de cada flujo modal.
- Acoplar `AppModal` a modulos de dominio o estados globales concretos.
- Redefinir el theme global del proveedor UI o la arquitectura SPA.

## Decisions

### Decision 1: Ubicar AppModal en `src/app/Components/UI/AppModal/`
- **Decision:** crear una carpeta dedicada con `AppModal.tsx`, `AppModal.module.css`, `AppModal.test.tsx`, `index.ts` y `README.md`.
- **Rationale:** `src/app/Components/UI` ya es la capa correcta para componentes compartidos y no ligados a dominio. La carpeta dedicada mantiene cohesion y simplifica evolucion, pruebas y documentacion.
- **Alternatives considered:** ubicarlo en `src/shared` o incrustarlo en un modulo de formularios; se descartan por mezclar responsabilidades o debilitar el contrato compartido.

### Decision 2: Componer sobre el dialogo del proveedor UI en lugar de reimplementar overlays manuales
- **Decision:** construir `AppModal` como wrapper sobre el componente de dialogo base ya disponible en la libreria UI del proyecto, usando herencia controlada (`Omit<ComponentProps<...>, ...>`) para redefinir la API propia.
- **Rationale:** reutiliza manejo maduro de portal, overlay y accesibilidad del proveedor UI, mientras oculta su API a las vistas consumidoras.
- **Alternatives considered:** renderizar overlays manuales o exponer directamente el modal del proveedor; se descartan por riesgo de inconsistencias o por mantener el acoplamiento actual.

### Decision 3: Estandarizar acciones del footer en una capa propia
- **Decision:** incluir acciones primarias y secundarias como parte del contrato publico del modal, con posibilidad de personalizar labels, estado de carga y callbacks.
- **Rationale:** la mayoria de flujos modales del proyecto requieren confirmacion/cancelacion; resolverlo en una capa compartida evita repeticion de estructuras y reduce divergencias visuales.
- **Alternatives considered:** delegar siempre el footer completo a `children`; se descarta porque obliga a repetir patrones de confirmacion y limita consistencia.

### Decision 4: Mantener accesibilidad y control de foco como responsabilidad del componente
- **Decision:** aprovechar el dialogo base del proveedor UI y complementar donde haga falta para asegurar titulo asociado, foco inicial y cierre por teclado cuando este habilitado.
- **Rationale:** un modal compartido sin comportamiento accesible consistente degrada la UX y hace mas fragiles los flujos criticos de confirmacion.
- **Alternatives considered:** delegar accesibilidad a cada vista; se descarta por alta probabilidad de implementaciones inconsistentes.

### Decision 5: Validar el contrato con pruebas de comportamiento
- **Decision:** agregar pruebas de apertura/cierre, render de titulo y contenido, acciones primarias/secundarias, bloqueo por estado de carga y accesibilidad basica del dialogo.
- **Rationale:** `AppModal` sera componente base de interacciones criticas; una suite de comportamiento reduce regresiones y hace verificables los escenarios del spec.
- **Alternatives considered:** snapshots o pruebas solo de render superficial; se descartan por baja cobertura del contrato real.

## Risks / Trade-offs

- **[Risk]** El contrato de acciones puede volverse demasiado opinionado para modales complejos.  
  **Mitigation:** mantener soporte de `children` y configuracion basica de footer, sin bloquear composicion adicional.
- **[Risk]** Los estilos propios pueden competir con el overlay y layout internos del proveedor UI.  
  **Mitigation:** encapsular estilos con CSS Modules y limitar overrides a capas estables del componente base.
- **[Risk]** El manejo de foco puede variar segun el proveedor UI y el contenido interno del modal.  
  **Mitigation:** apoyarse en el comportamiento base del proveedor y cubrir con pruebas los escenarios criticos de apertura y cierre.
- **[Risk]** La adopcion inicial puede ser baja si no queda claro como usarlo en confirmaciones simples.  
  **Mitigation:** incluir README con ejemplos concretos de confirmacion, cancelacion y modal con contenido libre.

## Migration Plan

1. Crear la carpeta `src/app/Components/UI/AppModal/` con implementacion, estilos, tests, barrel export y README.
2. Exponer el componente desde la capa UI compartida siguiendo la convencion del repo.
3. Ejecutar la suite de pruebas del componente y registrar evidencia en OpenSpec.
4. Dejar el componente listo para adopcion progresiva en futuros tickets de formularios y confirmaciones.

Rollback:
- Revertir los archivos de `AppModal` y sus exports; no hay migracion de datos ni efectos persistentes fuera del frontend.

## Open Questions

- Confirmar si el footer del modal debe renderizarse siempre por defecto o permitir modo sin acciones desde la primera version.
- Confirmar si el alcance inicial necesita tamano configurable (`sm`, `md`, `lg`) o basta con una presentacion base.
- Verificar si el equipo quiere soporte inicial para cierre al hacer click fuera del dialogo o si debe venir deshabilitado por defecto.
