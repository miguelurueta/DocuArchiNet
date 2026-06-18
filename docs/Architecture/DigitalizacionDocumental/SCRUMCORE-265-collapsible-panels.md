# SCRUMCORE-254 / SCRUMCORE-265 - Paneles colapsables

## Alcance implementado

Se agregan controles para ocultar y mostrar los dos paneles laterales del workspace de digitalizacion documental:

- Miniaturas.
- Configuracion de escaneo.

El preview PDF permanece como area central y se expande automaticamente cuando uno o ambos paneles se colapsan.

No se modificaron:

- inicializacion de Dynamsoft;
- seleccion de scanner;
- captura de paginas;
- drag and drop de miniaturas;
- generacion PDF;
- upload, metadata ni backend.

## Layout actual

Archivo:

```txt
src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.tsx
```

El workspace mantiene tres areas renderizadas:

```txt
Miniaturas | Preview PDF | Configuracion
```

Los dos laterales se renderizan con `AppCollapseRail`:

| Panel | Componente | Placement | Variant |
| --- | --- | --- | --- |
| Miniaturas | `AppCollapseRail` | `left` | `inline` |
| Configuracion de Escaneo | `AppCollapseRail` | `right` | `inline` |

El layout externo se controla desde CSS Grid:

```txt
src/modules/digitalizacion/components/DigitalizacionDocumentalWorkspace/DigitalizacionDocumentalWorkspace.module.css
```

Estados de grid:

| Estado | Columnas |
| --- | --- |
| Ambos visibles | `minmax(8rem, 0.62fr) minmax(18rem, 2.25fr) minmax(10rem, 0.82fr)` |
| Miniaturas ocultas | `0 minmax(18rem, 1fr) minmax(10rem, 0.82fr)` |
| Configuracion oculta | `minmax(8rem, 0.62fr) minmax(18rem, 1fr) 0` |
| Ambos ocultos | `0 minmax(18rem, 1fr) 0` |

En mobile se conserva una sola columna y los paneles colapsados pasan a `block-size: 0`.

## Controles

El ocultar/mostrar se delega al header y rail de `AppCollapseRail`:

| Control | Estado visible | Estado oculto |
| --- | --- | --- |
| Miniaturas | `Ocultar Miniaturas` | `Mostrar Miniaturas` |
| Configuracion | `Ocultar Configuracion` | `Mostrar Configuracion` |

Los botones son los del componente compartido, usando `AppButton` icon-only con `aria-controls` y `aria-expanded`.

## Persistencia

Clave:

```txt
docuarchi:digitalizacion:panel-preferences
```

Valor:

```json
{
  "showThumbnails": true,
  "showConfiguration": true
}
```

La lectura tolera storage corrupto o bloqueado y vuelve al estado por defecto: ambos paneles visibles.

## Dependencias preservadas

Miniaturas no se desmonta al colapsar; `AppCollapseRail` conserva la superficie montada y el grid externo contrae la columna lateral a `0`.

Esto conserva:

- paginas capturadas;
- seleccion actual;
- drag and drop configurado sobre las miniaturas;
- scroll interno del contenedor;
- estado del scanner;
- PDF pendiente o generado.

Configuracion tampoco se desmonta; los valores React de captura se conservan al ocultar y mostrar.

## Riesgos y mitigaciones

| Riesgo | Mitigacion |
| --- | --- |
| Re-render completo del scanner | No se cambia `scannerClient`, no se llama `initialize`, `dispose`, `scan` ni `clear` al alternar paneles. |
| Perdida de paginas o seleccion | Los paneles laterales permanecen montados y el estado vive en hooks superiores. |
| Controles ocultos navegables | Se usa el contrato accesible de `AppCollapseRail` en lugar de `aria-hidden` manual; las miniaturas reciben `tabIndex=-1` cuando el rail esta colapsado. |
| Storage no disponible | `localStorage` esta encapsulado en `try/catch`; la UI sigue funcionando en memoria. |
| Preview sin ancho minimo | La columna central mantiene `minmax(18rem, 1fr)`. |

## Validacion

Prueba focal:

```txt
npm run test -- src/app/Components/UI/AppDigitalizador/tests/AppDigitalizador.test.tsx --run
Resultado: OK
```

Casos cubiertos:

- toggle de Miniaturas y Configuracion mediante `AppCollapseRail` actualiza `data-thumbnails-collapsed` y `data-configuration-collapsed`;
- preview sigue montado;
- paneles laterales siguen en el DOM;
- preferencias se escriben en `localStorage`;
- preferencias se restauran al montar.

## Validacion manual recomendada

1. Abrir `/__sandbox/app-digitalizador`.
2. Seleccionar scanner.
3. Capturar varias paginas.
4. Ocultar Miniaturas y confirmar que el preview se expande.
5. Mostrar Miniaturas y confirmar que seleccion, scroll y orden se mantienen.
6. Ocultar Configuracion y confirmar que el preview se expande.
7. Ocultar ambos paneles y confirmar que el preview ocupa el ancho disponible.
8. Recargar la pagina y confirmar que los estados se restauran.
