# DOC-117 — 02-ImpactoUI

- Ticket: DOC-117
- Cambio OpenSpec: doc-117-produccion-importacion-carga-carpeta-almacenamiento
- Clasificacion: cross_cutting

## Superficies UI

Se usa la página oficial de producción documental y su carga individual al expediente467/nivel8/tipología50, junto a los endpoints existentes de importación en nivel7. No se introduce UI de producto ni un gate por usuarios/grupos.

## Validacion visual

La ejecución real automatizada usó la UI oficial y comprobó preparación, selección exacta del tipo documental, guardado e identidades devueltas. El helper exige una cabecera de alcance emitida por la candidata antes de seleccionar PDF y compara la descripción canónica exacta antes de Guardar. Las pruebas sintéticas cubren cabecera ausente, nombre alterado, respuesta perdida y resultados parciales. No se afirma una inspección visual humana de todos los navegadores o pantallas.

El formulario de reapertura falló una vez con campos enviados distintos; la reapertura independiente posterior con DLL original pasó. No se atribuye una causa de UI concreta ni se cambia el login de producto. Evidencia: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc117-alm03c-coexistencia-pass-20261106.md.
