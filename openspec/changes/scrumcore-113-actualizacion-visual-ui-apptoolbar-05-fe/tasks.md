## 1. Refinamiento del toolbar

- [x] 1.1 Revisar la configuracion actual de `AppToolbar` en `GestionRespuestaMainTabContent.tsx`
- [x] 1.2 Reemplazar la combinacion `actions` + `primaryAction` por un solo grupo de tres acciones
- [x] 1.3 Configurar `Solicitud de Aprobacion`, `Guardar` y `Enviar` con `size="sm"` y `variant="ghost"`

## 2. Consistencia visual

- [x] 2.1 Alinear las tres acciones en el bloque izquierdo del `AppToolbar`
- [x] 2.2 Verificar que `Enviar` deje de renderizarse como accion primaria separada
- [x] 2.3 Ajustar la tinta base de texto e iconos para el estado normal de los botones `ghost`, sin cambiar hover ni el comportamiento general
- [x] 2.4 Confirmar que el ajuste no rompa el layout del editor, el panel de herramientas ni la zona de adjuntos

## 3. Validacion

- [x] 3.1 Ajustar o crear pruebas para validar la presencia de `Solicitud de Aprobacion`, `Guardar` y `Enviar`
- [x] 3.2 Validar que el toolbar del tab `Gestion` siga renderizando correctamente dentro del workbench
- [x] 3.3 Registrar evidencia de tests ejecutados para el change

## Evidencia de pruebas

- `node .\\node_modules\\vitest\\vitest.mjs --run src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx` (2026-04-15)
