## 1. Migracion del bloque superior

- [x] 1.1 Reemplazar los tres `AppInput type="checkbox"` por `AppCheckbox` en `GestionDocumentoModal.tsx`
- [x] 1.2 Eliminar la dependencia de `AppInput` para el bloque superior del modal
- [x] 1.3 Mantener los labels finales y el estado local visual de cada decision

## 2. Ajustes visuales del modulo

- [x] 2.1 Ajustar `GestionDocumentoModal.module.css` para que el bloque de checks siga en columna
- [x] 2.2 Garantizar wrap correcto de labels largos sin overflow horizontal
- [x] 2.3 Mantener estable la composicion del modal respecto a selects, info box, tags y acciones

## 3. Accesibilidad y consistencia

- [x] 3.1 Mantener roles y labels accesibles correctos en los tres checkboxes
- [x] 3.2 Conservar el foco inicial y el flujo de cierre del modal
- [x] 3.3 Confirmar que la integracion consume `AppCheckbox` desde el shared y no crea wrappers locales

## 4. Validacion

- [x] 4.1 Actualizar pruebas del modal para validar la migracion a `AppCheckbox`
- [x] 4.2 Validar apertura, cierre e interaccion de los tres checkboxes
- [x] 4.3 Validar que el resto del contenido del modal sigue presente
- [x] 4.4 Ejecutar la suite relevante y registrar evidencia en este archivo

## Evidencia

- 2026-04-16: `node .\node_modules\vitest\vitest.mjs --run src\modules\gestionCorrespondencia\tests\GestionRespuestaMainTabContent.test.tsx`
- Resultado: `3 tests passed`
