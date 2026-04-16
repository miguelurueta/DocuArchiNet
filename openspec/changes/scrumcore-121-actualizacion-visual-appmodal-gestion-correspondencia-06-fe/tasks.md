## 1. Trigger y estructura base

- [x] 1.1 Revisar el botón `Solicitud de Aprobacion` en `GestionRespuestaMainTabContent.tsx`
- [x] 1.2 Crear `modalGestionDocumento/` dentro de `gestionRespuestaMainTab/`
- [x] 1.3 Implementar `GestionDocumentoModal.tsx` como componente controlado con `open` y `onClose`
- [x] 1.4 Integrar el trigger del modal desde el botón existente

## 2. UI del modal

- [x] 2.1 Construir `infoBox` superior con estilo moderno
- [x] 2.2 Construir `formGrid` usando `AppInputSelect`, `AppInput` checkbox y `AppInputTags`
- [x] 2.3 Construir `actions` alineadas a la derecha con `Cancelar` y `Guardar`
- [x] 2.4 Implementar `GestionDocumentoModal.module.css` con CSS Modules
- [x] 2.5 Validar layout moderno y responsive en desktop, tablet y mobile

## 3. Accesibilidad y estado local

- [x] 3.1 Mantener el modal controlado sin lógica de negocio
- [x] 3.2 Implementar estado local básico del formulario
- [x] 3.3 Validar foco inicial, navegación por teclado y labels claros
- [x] 3.4 Confirmar cierre consistente por cancelar y por `onClose`

## 4. Validación

- [x] 4.1 Crear o ajustar pruebas para apertura del modal desde el botón existente
- [x] 4.2 Validar render de todos los componentes internos
- [x] 4.3 Validar interacción básica de los controles locales
- [x] 4.4 Validar cierre correcto del modal
- [x] 4.5 Registrar evidencia de tests ejecutados para el change

## Evidencia

- 2026-04-16: `node .\\node_modules\\vitest\\vitest.mjs --run src\\app\\Components\\UI\\AppInput\\AppInput.test.tsx src\\modules\\gestionCorrespondencia\\tests\\GestionRespuestaMainTabContent.test.tsx`
- Resultado: `10 tests passed`
