## 1. AppLoadingState (shared UI)

- [x] 1.1 Crear `src/app/Components/UI/AppLoadingState/AppLoadingState.tsx` con props tipadas (sin `any`) y lógica de delay interna.
- [x] 1.2 Crear `src/app/Components/UI/AppLoadingState/AppLoadingState.module.css` (card pequeña, responsive, centrada, no full-screen).
- [x] 1.3 Crear `src/app/Components/UI/AppLoadingState/index.ts` para exportar el componente.
- [x] 1.4 (Opcional) Alinear export/barrel global si el proyecto lo requiere (sin romper imports existentes).

## 2. Tests del componente

- [x] 2.1 Agregar tests unitarios: no renderiza antes de `delayMs` (usar fake timers).
- [x] 2.2 Agregar tests unitarios: renderiza después de `delayMs` si `loading` sigue true.
- [x] 2.3 Agregar tests unitarios: se oculta al pasar `loading=false`.
- [x] 2.4 Agregar tests unitarios: limpia timers (unmount / cambios rápidos) y no hace setState tras unmount.
- [x] 2.5 Agregar tests de integración UI: wrapper mode con `children` (si aplica) y render dentro de contenedores/paneles.

## 3. Migración en Gestión Correspondencia (consumidor)

- [x] 3.1 Migrar `src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.tsx` para usar `AppLoadingState` en estado `loading`.
- [x] 3.2 Eliminar completamente lógica local de temporización (p.ej. `showDelayedLoader`, `setTimeout/clearTimeout`) del consumidor.
- [x] 3.3 Mantener `data-testid="gestion-correspondencia-loading-state"` (en wrapper o dentro de `AppLoadingState`, de forma consistente).
- [x] 3.4 Confirmar que `loading` solo represente “primer load” (sin flicker por refetch en background) y que `blocked/ready` no cambian.

## 4. Validación y calidad

- [x] 4.1 Ejecutar tests relevantes (Vitest) y asegurar que pasan.
- [x] 4.2 Verificación manual en navegador: aparece tras el delay, desaparece correctamente, sin flicker perceptible.
- [x] 4.3 Actualizar documentación si se ajusta el contrato final (props) o decisiones visuales.
