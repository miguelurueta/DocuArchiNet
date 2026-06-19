# SCRUMCORE-257 / SCRUMCORE-269 - Seleccion visual y recorte manual

## Arquitectura

La seleccion de area vive en `DigitalizacionDocumentalWorkspace`, sobre el preview existente. No crea modal, no desmonta el preview y no reconstruye `scanner.pages`.

Capas:

- Toolbar Preview PDF: boton `Seleccionar area` con icono de tijera y estado activo/inactivo.
- Preview: superficie relativa alrededor de la imagen activa para capturar eventos pointer.
- Scanner hook: expone `cropPage(pageId, selection)` y limpia el PDF pendiente al cambiar la pagina.
- Dynamsoft client: traduce la seleccion a `DWT.Crop(index, left, top, right, bottom)` y refresca solo la pagina afectada.

## Flujo

1. Usuario activa `Seleccionar area`.
2. En la pagina visible hace pointer down, drag y pointer up.
3. El workspace dibuja el rectangulo sobre la imagen activa.
4. Al existir seleccion se muestran acciones flotantes: recortar, reiniciar seleccion y cancelar.
5. `Recortar` llama `cropPage` con la pagina seleccionada.
6. Dynamsoft aplica `Crop` en el indice de esa pagina y el hook actualiza `scanner.pages` con el resultado.

## Modelo De Coordenadas

La seleccion se almacena como:

```ts
{
  x: number;
  y: number;
  width: number;
  height: number;
}
```

Los valores estan expresados contra las dimensiones reales de la pagina (`ScanPage.width` y `ScanPage.height`). El rectangulo visual se renderiza con porcentajes, por lo que se mantiene coherente con zoom 50%, 100%, 200%, fit width, fit page y pantalla completa.

## Eventos

La superficie del preview usa eventos pointer:

- `onPointerDown`: inicia la seleccion si el modo esta activo.
- `onPointerMove`: actualiza el rectangulo.
- `onPointerUp` / `onPointerCancel`: cierra el draft y conserva una seleccion valida.

No se usan coordenadas del zoom como fuente de verdad; solo sirven para convertir la posicion del puntero a coordenadas reales de pagina.

## Rendimiento

El recorte no solicita imagenes nuevas a Dynamsoft fuera de la pagina afectada. Tampoco regenera miniaturas ni PDF completo de forma anticipada. La operacion invalida `scanner.pdf` para que el PDF pendiente se regenere cuando el usuario pulse `Generar PDF`.

## Riesgos

- Si el runtime no expone `DWT.Crop`, el cliente devuelve error controlado y no modifica el lote.
- Si una pagina no trae dimensiones, se usa el rect del preview como fallback para no bloquear la seleccion visual.
- La fase actual implementa recorte rectangular; mover o redimensionar una seleccion ya creada queda preparado para una fase posterior.

## Evidencia Visual

```text
Toolbar Preview PDF
[Organizar] [Seleccionar area] [Pagina] [Ir] [Rotar] [Zoom]

Preview
+--------------------------------------+
|                                      |
|     +--------------------------+     |
|     |                          |     |
|     |       Area seleccionada  |     |
|     |                          |     |
|     +--------------------------+     |
|                         [acciones]   |
+--------------------------------------+
```

## Validaciones

- Pagina vertical A4: coordenadas reales desde `width < height`.
- Pagina horizontal: coordenadas reales desde `width > height`.
- Zoom 50%, 100%, 200%: el rectangulo usa porcentajes sobre la imagen.
- Pantalla completa: la superficie mantiene los mismos eventos pointer.
- Mas de una pagina: `cropPage` recibe solo el `pageId` activo.
