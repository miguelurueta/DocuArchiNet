## 1. Ajustes visuales del header en GestionRespuestaMainTabContent

- [x] 1.1 Reemplazar el boton textual `Volver a la bandeja` por un control icon-only con `LeftOutlined` conservando la accion de retorno.
- [x] 1.2 Anadir accesibilidad al boton icon-only (`aria-label`, foco visible y navegacion por teclado).
- [x] 1.3 Reubicar el bloque `metadata` para que quede a la derecha inmediata del boton de retorno dentro del mismo contenedor de header.
- [x] 1.4 Eliminar render y espacio visual de `headerDescription` en el header de la vista.

## 2. Compactacion visual de AppUpload

- [x] 2.1 Ajustar estilos de `AppUpload` en el contexto de gestion respuesta para reducir alto, padding y densidad visual.
- [x] 2.2 Verificar que la variante compacta conserve interaccion de carga, acciones y estado de foco sin degradacion funcional.

## 3. Responsive y calidad

- [x] 3.1 Validar layout del header (`boton + metadata`) en desktop, tablet y mobile evitando overflow horizontal.
- [x] 3.2 Validar que `AppUpload` compacto no colisione con otros bloques ni rompa el flujo en pantallas pequenas.
- [x] 3.3 Actualizar pruebas unitarias/integracion del modulo para cubrir boton icon-only, ausencia de `headerDescription` y presencia de metadata en header.
- [x] 3.4 Ejecutar suite focalizada de pruebas del modulo y registrar evidencia de ejecucion asociada al cambio.

## Evidencia

- `npm.cmd run test -- src/modules/gestionCorrespondencia/routes/GestionCorrespondenciaRoute.spec.test.tsx`
- `npm.cmd run test -- src/modules/gestionCorrespondencia/tests/GestionRespuestaMainTabContent.test.tsx`
- `npm.cmd run test -- src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx`
