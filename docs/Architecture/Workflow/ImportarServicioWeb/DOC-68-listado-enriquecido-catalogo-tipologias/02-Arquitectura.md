# Arquitectura

`WebServiceImportarServicioWebModern` valida sesión/tarea y coordina:

- `SiiImportContractMapper.MapQuery`: mapeo de la única respuesta SII y filtro autoritativo `tipoanexo=505`.
- `SiiExternalImportProviderClient.ResolveImageAsync`: vuelve a validar `tipoanexo=505` antes de preview o descarga.
- `ImportItemPresentationService`: catálogo, estado, acciones y paginación.
- `MySqlImportDocumentTypeCatalogRepository`: lectura Radicación por trámite.
- `MySqlImportItemStatusRepository`: lectura Workflow por tarea/proveedor/clave.

Las dependencias se construyen con conexiones autoritativas de `ResultadoContextoSesionWorkflow`; el navegador no aporta trámite, usuario ni estado.
