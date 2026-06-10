# Legacy reference: FileUploadHandler.js

## Archivo original

```txt
D:\imagenesda\gestordocumental\desarrollo\old\oldanterior\GestionDocumental-Docuarchi.net\generic_control\FileUploadHandler.js
```

## Archivo copiado

```txt
docs\Architecture\AppUploadDocumental\legacy\FileUploadHandler.legacy.js
```

## Uso en la migracion

Este archivo se conserva como referencia para disenar `AppUploadDocumental`.

Comportamientos legacy relevantes:

- seleccion multiple de archivos;
- carga de parametros de extension y tamano desde servicio;
- tipologia documental por archivo;
- sugerencia de tipologia por nombre de archivo;
- validacion de tipologia obligatoria;
- armado de metadata documental por archivo;
- envio de archivo al backend legacy;
- callbacks para refrescar la interfaz.

## No ejecutar

Este archivo no debe importarse ni ejecutarse desde `src`.

Contiene dependencias y patrones legacy:

- jQuery;
- Bootstrap modal manual;
- variables globales;
- servicios ASMX/ASHX;
- callbacks por nombre string;
- HTML construido manualmente.

La migracion debe extraer comportamiento y contratos, no reutilizar este codigo directamente.

