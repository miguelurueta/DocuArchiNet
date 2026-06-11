## Why

`SCRUMCORE-239` dejo una base reutilizable de `DigitalizacionDocumental`, pero la validacion final demostro que el componente publico real (`DigitalizacionDocumentalModal`) estaba acoplado a `AppModal`. Eso impedia montarlo inline dentro del layout de `CapDocument` ocupando el panel izquierdo sin overlay.

`SCRUMCORE-245` corrige esa brecha y formaliza trazabilidad legacy minima para que el refactor no se cierre como una pantalla parcial sin equivalencias funcionales.

## What Changes

- Extraer el contenido operativo de digitalizacion a `DigitalizacionDocumentalWorkspace`.
- Mantener `DigitalizacionDocumentalModal` como wrapper compatible con `AppModal`.
- Exportar el workspace inline desde `src/modules/digitalizacion`.
- Documentar trazabilidad legacy de scanner, upload temporal, metadata, crear documento y adjuntar digitalizacion.
- Registrar brechas que dependen de backend o de archivos legacy no accesibles.
- Agregar prueba que confirma que el workspace se renderiza inline sin `role="dialog"`.

## Legacy Inputs

Archivos disponibles usados como fuente:

- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\online_demo_initpage.js`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\online_demo_operation.js`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.js`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.aspx`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\WebFormEscan.aspx.vb.txt`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\Webform_save_digital_image.aspx`
- `C:\Users\SEBASTIAN FORERO\Documents\Archivos de Scaner\Webform_save_digital_image.aspx.vb.txt`

Rutas `D:\imagenesda\...` no estuvieron accesibles en esta sesion; se documentan como bloqueo de confirmacion contra el legacy original.

## Impact

- Nuevo componente corporativo embebible: `src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx`.
- Wrapper existente preservado: `src/modules/digitalizacion/components/DigitalizacionDocumentalModal/DigitalizacionDocumentalModal.tsx`.
- Nuevo documento de trazabilidad: `docs/Architecture/DigitalizacionDocumental/SCRUMCORE-239-legacy-traceability.md`.

## Validation

- `npx eslint src/modules/digitalizacion --ext .ts,.tsx`: PASS.
- `npx vitest run src/modules/digitalizacion`: PASS, 50 tests.
- `npm run build`: FAIL por errores preexistentes fuera del alcance en `src/app/Components/UI/AppEditor/presentation/AppEditorToolbar.tsx` (`dropdownProps` no existe en `AppDropdownProps`).
