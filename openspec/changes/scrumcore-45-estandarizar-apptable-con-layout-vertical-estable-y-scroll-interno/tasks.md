## 1. Contrato compartido

- [x] 1.1 Agregar `layoutMode?: "content" | "fill"` a `AppTable`.
- [x] 1.2 Mantener compatibilidad hacia atrás cuando `layoutMode` no se informa.

## 2. Implementación shared

- [x] 2.1 Forzar `domLayout="normal"` cuando `layoutMode="fill"`.
- [x] 2.2 Ajustar estilos base de `AppTable` para ocupar el alto restante y permitir scroll interno.
- [x] 2.3 Ajustar `AppTableQueryWrapper` para soportar un tramo flexible de tabla.

## 3. Integración inicial

- [x] 3.1 Aplicar `layoutMode="fill"` en `GestionCorrespondencia`.
- [x] 3.2 Ajustar el layout del módulo para ceder el alto restante a la tabla.

## 4. Validación

- [x] 4.1 Agregar o actualizar pruebas de `AppTable`.
- [x] 4.2 Validar que `GestionCorrespondencia` siga en `server mode` y adopte `fill`.
- [x] 4.3 Ejecutar suite focalizada:
  - `npm.cmd test -- src/app/Components/UI/AppTable/tests/AppTable.test.tsx src/app/Components/UI/AppTable/tests/AppTableQueryWrapper.test.tsx src/modules/gestionCorrespondencia/tests/GestionCorrespondencia.test.tsx src/modules/gestionCorrespondencia/tests/useGestionCorrespondenciaTable.test.tsx`
