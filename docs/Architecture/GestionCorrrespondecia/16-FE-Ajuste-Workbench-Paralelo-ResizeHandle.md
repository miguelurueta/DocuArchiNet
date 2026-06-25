# 16-FE - Ajuste visual enterprise del workbench paralelo

## Metadata

- Fecha: 2026-06-25
- Rama: `feature/SCRUMCORE-270`
- Commit: este mismo commit de cierre del ajuste visual
- Area: Gestion Correspondencia
- Componente: `GestionWorkbenchParallelTabs`
- Tipo de cambio: ajuste visual CSS/JSX sin cambio funcional
- Backend: no modificado
- Endpoints: no modificados
- Estado: implementado y documentado

## Objetivo

Refinar el separador visual entre los paneles paralelos de Gestion y Documentos para que el workbench mantenga un aspecto mas sobrio, delgado y alineado con el azul enterprise usado en la interfaz de inicio de sesion.

Tambien se elimina el borde redondeado del contenedor de paneles para evitar que la vista lateral se perciba como una tarjeta enmarcada, dejando una composicion mas integrada con el shell principal.

## Archivos modificados

```txt
src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.tsx
src/modules/gestionCorrespondencia/components/workbenchParallelTabs/GestionWorkbenchParallelTabs.module.css
```

## Cambios tecnicos aplicados

### Eliminacion del grip interno

Se removio el elemento decorativo interno:

```tsx
<span className={styles.resizeGrip} aria-hidden="true" />
```

El separador conserva su semantica accesible a traves del componente `Separator` de `react-resizable-panels`, con:

- `aria-label="Redimensionar paneles"`;
- `title="Arrastra para redimensionar"`;
- `role="separator"` generado por la libreria;
- soporte de teclado y arrastre provisto por `react-resizable-panels`.

La eliminacion del `span` reduce ruido visual y evita que el affordance se vea como un patron de puntos dentro del handle.

### Separador azul delgado

El area interactiva del separador queda en `5px`, con una linea visual de `2px` en reposo y `3px` en hover/focus/activo.

Valores aplicados:

```css
flex: 0 0 5px;
width: 5px;
min-width: 5px;
```

La linea visual se renderiza con `::before`, centrada con `left: 50%` y `transform: translateX(-50%)`, evitando elementos HTML extra.

Color principal:

```css
rgba(37, 99, 235, 0.72)
```

Color interactivo:

```css
#2563eb
```

Estos valores mantienen coherencia con la paleta enterprise azul usada en el login.

### Estado hover, focus y resize activo

Se conserva retroalimentacion visual cuando el usuario interactua con el separador:

- aumenta la linea de `2px` a `3px`;
- usa azul solido `#2563eb`;
- aplica halo discreto `rgba(37, 99, 235, 0.12)`;
- mantiene `cursor: col-resize`.

Esto permite que el separador sea visualmente sobrio en reposo, pero claramente operable al pasar el cursor o usar teclado.

### Paneles sin borde ni radio

Se actualizaron los paneles para retirar el borde y las esquinas redondeadas:

```css
border: 0;
border-radius: 0;
```

Motivo:

- evitar apariencia de tarjetas anidadas;
- integrar mejor los paneles dentro del workbench;
- mantener una composicion mas plana y enterprise;
- conservar el fondo blanco y el manejo de overflow existente.

## Alcance funcional

Este ajuste no modifica:

- logica de redimensionamiento;
- tamanos por defecto de paneles;
- restricciones `minSize`;
- comportamiento mobile;
- datos de Gestion;
- datos de Documentos;
- llamadas a servicios;
- estado de React;
- comportamiento de `react-resizable-panels`.

## Accesibilidad

El separador mantiene:

- etiqueta accesible;
- titulo descriptivo;
- foco visible mediante `:focus-visible`;
- semantica de separador controlada por la libreria;
- affordance de cursor para mouse.

La eliminacion del `span` no afecta lectores de pantalla porque era `aria-hidden="true"` y puramente decorativo.

## Validacion

Comando esperado:

```txt
npx.cmd tsc --noEmit --pretty false
```

Resultado al cierre del cambio: pendiente de registrar tras validacion final del commit.

Resultado registrado:

```txt
OK - TypeScript sin errores.
```

## Riesgos y mitigaciones

- Riesgo: un handle demasiado delgado puede ser dificil de tomar con mouse.
- Mitigacion: se dejo area interactiva de `5px`, mayor que la linea visual.

- Riesgo: perder affordance al quitar el grip de puntos.
- Mitigacion: linea azul visible en reposo, hover/focus con mayor grosor y halo.

- Riesgo: afectar mobile.
- Mitigacion: la vista paralela sigue oculta bajo `@media (max-width: 900px)` sin cambios.

## Estado final esperado

El workbench paralelo debe verse como una composicion continua entre Gestion y Documentos, con un separador azul delgado, limpio y operable, sin bordes redondeados en los paneles.
