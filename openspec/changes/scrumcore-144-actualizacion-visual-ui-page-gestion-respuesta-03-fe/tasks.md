## 1. Ajustes visuales del header en GestionRespuestaMainTabContent

- [ ] 1.1 Reemplazar el botón textual `Volver a la bandeja` por un control icon-only con `LeftOutlined` conservando la acción de retorno.
- [ ] 1.2 Añadir accesibilidad al botón icon-only (`aria-label`, foco visible y navegación por teclado).
- [ ] 1.3 Reubicar el bloque `metadata` para que quede a la derecha inmediata del botón de retorno dentro del mismo contenedor de header.
- [ ] 1.4 Eliminar render y espacio visual de `headerDescription` en el header de la vista.

## 2. Compactación visual de AppUpload

- [ ] 2.1 Ajustar estilos de `AppUpload` en el contexto de gestión respuesta para reducir alto, padding y densidad visual.
- [ ] 2.2 Verificar que la variante compacta conserve interacción de carga, acciones y estado de foco sin degradación funcional.

## 3. Responsive y calidad

- [ ] 3.1 Validar layout del header (`botón + metadata`) en desktop, tablet y mobile evitando overflow horizontal.
- [ ] 3.2 Validar que `AppUpload` compacto no colisione con otros bloques ni rompa el flujo en pantallas pequeñas.
- [ ] 3.3 Actualizar pruebas unitarias/integración del módulo para cubrir botón icon-only, ausencia de `headerDescription` y presencia de metadata en header.
- [ ] 3.4 Ejecutar suite focalizada de pruebas del módulo y registrar evidencia de ejecución asociada al cambio.
