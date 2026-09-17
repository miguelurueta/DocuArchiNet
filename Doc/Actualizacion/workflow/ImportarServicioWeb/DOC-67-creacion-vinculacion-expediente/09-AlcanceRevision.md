# Alcance de revisión documental

## Repositorio y módulos incluidos

Repositorio: `DocuArchiNet` (rutas relativas a su raíz).

- Frontera: `webservice/WebServiceImportarServicioWebModern.asmx.vb`.
- Aplicación: `Services/Workflow/ImportarServicioWeb/`.
- Contratos: `DTOs/Workflow/ImportarServicioWeb/` y `Modelo/Workflow/ImportarServicioWeb/`.
- Persistencia: `Infrastructure/Repositories/Workflow/ImportarServicioWeb/`.
- Integración física: `Infrastructure/Workflow/ImportarServicioWeb/Expedients/`, `Sii/` y `Storage/`.
- Pruebas: `tests/importar-servicio-web-*.test.cjs` y `tools/e2e/`.

## Exclusiones verificables

No se revisan internamente el proveedor remoto SII, MySQL, filesystem/XML ni el almacenamiento legacy; se documentan como actores externos o cajas negras. El contrato multi-expediente del proveedor continúa sin verificación externa y no se declara probado.

## Convención

- `CODE:` clase, interfaz, método o DTO resoluble contra VB.NET.
- `EXT:` actor o sistema externo, excluido de resolución contra código.
- `CONCEPT:` decisión o estado conceptual, excluido de resolución.
- Cada diagrama declara `Fuentes:` y `Referencias CODE:`. Las firmas usan `Clase.Método(Tipo,...):Retorno`; las sobrecargas se distinguen por parámetros.
