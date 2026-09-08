## Why

La integración SII actual mezcla transporte, semántica registral, códigos legacy y coordinación documental dentro de fronteras históricas. DOC-55 introduce un adaptador moderno y paralelo que reutiliza el núcleo de importación construido en B01-B05 sin alterar los ASMX ni el almacenamiento vigente.

## What Changes

- Incorporar `INTEGRACIONSII` como proveedor explícito del registro moderno.
- Normalizar consulta, capacidades y preview SII hacia los contratos comunes.
- Confinar `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` a un adaptador de compatibilidad.
- Publicar un ASMX moderno delgado, protegido por gate y sin lógica de negocio.
- Reutilizar el transporte HTTP y el puerto de almacenamiento existentes.
- Probar contratos con fixtures saneados y sin red real.
- Documentar la solución en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-55-adaptador-sii-compatibilidad-asmx/`.

## Capabilities

### New Capabilities

- `adaptador-sii-asmx`: Adaptación aislada de SII, preview mediado y compatibilidad ASMX reversible.

### Modified Capabilities

- Ninguna.

## Impact

- Nuevos componentes en `Infrastructure/Workflow/ImportarServicioWeb/Sii/`.
- Nueva frontera `webservice/WebServiceImportarServicioWebModern.asmx*`.
- Registro de archivos nuevos en `GestionDocumental-Docuarchi.net.vbproj`.
- Pruebas focales y fixtures en `Tests/`.
- Sin cambios en ASMX existentes, `Integracionccv/`, `ServiciosIntegracion/`, `workflow/ClassAlmacenamiento.vb` ni JavaScript legacy.
