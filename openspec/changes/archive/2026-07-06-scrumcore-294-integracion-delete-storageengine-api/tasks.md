## 1. Refinement

- [x] 1.1 Confirmar que el flujo usa `eliminar_item` del workbench como entrada funcional.
- [x] 1.2 Cerrar el mapeo de identificadores para delete (`idAlmacen`, `IdDocumento`, `DocumentId`, `NombreGabinete`).
- [x] 1.3 Definir la politica de mensajes y severidad para error de negocio, autorizacion y error tecnico.
- [x] 1.4 Fijar `sourceModule=WORKFLOW` para esta pantalla y dejarlo documentado en el contrato.

## 2. Implementation

- [x] 2.1 Agregar o ajustar el servicio frontend para el delete persistido del StorageEngine.
- [x] 2.2 Conectar la accion `eliminar_item` del `DocumentosWorkbench` al flujo de delete.
- [x] 2.3 Respetar `CanDelete` cuando este presente y mantener compatibilidad con filas legacy.
- [x] 2.4 Refrescar la lista y limpiar el documento activo cuando la fila borrada este abierta en el visor.
- [x] 2.5 Preservar `requestId` y soportar mensajes de error con precedencia estricta.

## 3. Testing

- [x] 3.1 Agregar pruebas de exito, bloqueo de negocio y permiso denegado.
- [x] 3.2 Agregar pruebas del refresh posterior al delete y del cleanup del estado activo.
- [x] 3.3 Validar que el mensaje mostrado priorice `UserMessage` sobre `Message` y `message`.
- [x] 3.4 Verificar que la UI no exponga rutas, SQL, tokens ni stack traces en el mensaje principal.

## 4. Publish

- [x] 4.1 Validar el cambio con OpenSpec.
- [x] 4.2 Revisar el diff final para asegurar que el alcance publica solo el contrato necesario.
- [x] 4.3 Generar la documentacion enterprise en `docs/Architecture/GestionCorrrespondecia/Integracion-Delete-StorageEngine/`.
- [x] 4.4 Registrar el ticket en el indice documental del modulo si aplica.
