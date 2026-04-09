## Why

Implementar en `AppTable` una affordance navegable reusable, opt-in y accesible para tablas con accion primaria por fila o celda, sin acoplar el componente shared a modulos especificos ni mantener CSS local duplicado en consumidores como `GestionCorrespondencia`.

## What Changes

- Se extiende el contrato shared de `AppTable` con una prop opt-in para affordance navegable.
- Se define implementacion reusable en el renderer del grid para aplicar estilos y soporte de teclado sin interceptar la navegacion real.
- Se preserva la separacion entre affordance visual del shared component y navegacion del modulo consumidor.
- Se deja preparado el componente para que modulos consumidores eliminen CSS local equivalente en tickets de adopcion.

## Capabilities

### New Capabilities
- None.

### Modified Capabilities
- `crea-componente-table`: Extiende `AppTable` con affordance navegable reusable y soporte de teclado.

## Impact

- Cambios en `src/app/Components/UI/AppTable/`.
- Cobertura de pruebas en `src/app/Components/UI/AppTable/tests/*`.
- Sin cambios de dominio ni navegacion hardcodeada en el shared component.
