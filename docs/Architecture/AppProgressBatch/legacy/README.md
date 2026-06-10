# Legacy reference: JSProgresBar.js

## Archivo original

```txt
D:\imagenesda\GestorDocumental\Desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\js\java_general\JSProgresBar.js
```

## Archivo copiado

```txt
docs\Architecture\AppProgressBatch\legacy\JSProgresBar.legacy.js
```

## Uso en la migracion

Este archivo se conserva como referencia para disenar `AppProgressBatch`.

Comportamientos legacy relevantes:

- ejecucion secuencial de una lista de items;
- barra de progreso global;
- contador `x de y`;
- nombre de proceso;
- cancelacion;
- pausa por confirmacion;
- manejo de errores controlados;
- resumen implicito del proceso.

## No ejecutar

Este archivo no debe importarse ni ejecutarse desde `src`.

Contiene dependencias y patrones legacy:

- jQuery;
- Bootstrap modal manual;
- variable global `_JSProgresBar`;
- seleccion de acciones por `name_service`;
- llamadas a funciones globales;
- HTML construido manualmente.

La migracion debe convertir el comportamiento en un componente generico que reciba `processItem` desde afuera.

