# SCRUMCORE-293 - Manual Deskew

## Objetivo

Permitir corregir manualmente la inclinacion de paginas ya capturadas en el modulo reusable de Digitalizacion Documental, sin duplicar el motor de Deskew existente en la captura automatica.

## Flujo funcional

1. El usuario captura o incorpora paginas al documento.
2. Selecciona una pagina activa o varias paginas desde miniaturas/organizador.
3. Ejecuta la accion Deskew ubicada junto a rotar izquierda y rotar derecha.
4. El workspace muestra el overlay corporativo con el estado "Corrigiendo inclinacion".
5. El scanner client procesa las paginas seleccionadas en orden visual.
6. El hook actualiza `pages`, invalida el PDF generado y refresca preview, miniaturas, organizador y navegacion.

## Reutilizacion del motor existente

`DynamsoftTwainClient.deskewPage(pageId)` reutiliza la misma entrada `deskew` de `automaticProcessingFeatures` que se usa durante el escaneo automatico. La implementacion conserva un unico registro de metodos compatibles (`Deskew`, `deskew`, `DeskewImage`, `AutoDeskew`) y evita crear un algoritmo paralelo en React.

Cuando la API nativa no esta disponible, la operacion se reporta como `unsupported` y retorna la coleccion actual de paginas. Esto mantiene el comportamiento no destructivo para paginas ya alineadas o estaciones sin soporte nativo.

## Compatibilidad

La accion opera sobre cualquier pagina que exista en la coleccion del scanner:

- Escaneos nuevos.
- Paginas incorporadas por flujos de importacion.
- Paginas duplicadas.
- Paginas insertadas antes o despues de la activa.
- Paginas reemplazadas.
- Capturas agregadas al final.

El contrato reusable queda expuesto como `DigitalizacionScannerClient.deskewPage(pageId)`, por lo que `AppDigitalizador`, el modal y el workspace comparten el mismo flujo.

## Casos de uso cubiertos

- Corregir una pagina activa con inclinacion residual.
- Corregir varias paginas seleccionadas desde miniaturas.
- Corregir seleccion multiple desde el organizador.
- Ejecutar la accion sobre una pagina ya alineada sin mostrar error funcional.
- Regenerar el PDF despues de la correccion para reflejar la nueva imagen.

## Validacion

La implementacion incluye pruebas de:

- Cliente Dynamsoft: reutilizacion de Deskew nativo sobre una pagina capturada.
- Hook `useDigitalizacionScanner`: progreso, actualizacion de paginas e invalidacion de PDF.
- `AppDigitalizador`: accion manual en seleccion multiple y overlay corporativo.
