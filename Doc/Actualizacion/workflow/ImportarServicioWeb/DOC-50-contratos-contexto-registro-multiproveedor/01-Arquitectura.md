# Arquitectura

## Alcance y fronteras

DOC-50 crea una frontera paralela en .NET Framework 4.6.1. DTOs define transporte serializable; Modelo contiene contexto, invariantes y puertos; Services valida, resuelve y delega. Las dependencias apuntan hacia Modelo. Ninguna clase nueva depende de WebForms, ASMX, `HttpContext`, sesión, SQL o red.

`ContextoImportacionServicio` captura por constructor usuario, grupo, login, tarea, ruta, trámite, proveedor y permiso. Sus propiedades son de solo lectura, por lo que un cambio posterior en la sesión no altera una operación ya iniciada.

El registro usa identidad canónica sin distinguir mayúsculas y falla con `PROVIDER_NOT_SUPPORTED`; no existe fallback a `INTEGRACIONSII`. El código inspeccionado solo permite afirmar que esa identidad existe en el recorrido legacy; otros proveedores requieren configuración y adaptadores comprobados.

Los ASMX futuros podrán adaptar los contratos, pero DOC-50 no crea ni modifica endpoints. `AlmacenaDocumentoTareaWorkflow(...)` continúa en `workflow/ClassAlmacenamiento.vb` con su implementación y consumidores existentes, sin cambios.

## Rutas y dependencias permitidas

| Área | Ruta canónica | Puede depender de | No puede depender de |
| --- | --- | --- | --- |
| DTOs | `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb` | BCL | Modelo, Services, WebForms, ASMX, sesión |
| Modelo | `Modelo/Workflow/ImportarServicioWeb/*.vb` | BCL | DTOs, WebForms, ASMX, infraestructura |
| Services | `Services/Workflow/ImportarServicioWeb/*.vb` | Modelo y DTOs para mapeo | sesión, SQL, red, ASMX |
| Infraestructura futura | fuera de DOC-50 | DTOs, Modelo y Services | copias locales de contratos |

Los seis archivos VB están registrados una vez en `GestionDocumental-Docuarchi.net.vbproj`.

## Tabla de funciones

| Función | Ruta | Clase/interfaz | Parámetros | Responsabilidad | Condición |
| --- | --- | --- | --- | --- | --- |
| `ResolverCapacidades` | `Services/.../ServicioImportarServicioWeb.vb` | `ServicioImportarServicioWeb` | contexto, operación, correlación | Validar, resolver proveedor y mapear capacidades | Nueva |
| `ConsultarElementos` | `Services/.../ServicioImportarServicioWeb.vb` | `ServicioImportarServicioWeb` | contexto, continuación, tamaño, operación, correlación | Validar, resolver y consultar elementos | Nueva |
| `Resolver` | `Services/.../RegistroProveedoresImportacion.vb` | `RegistroProveedoresImportacion` | providerId | Resolver solo una identidad registrada o fallar cerrado | Nueva |
| `Validar` | `Services/.../ValidadorContextoImportacion.vb` | `ValidadorContextoImportacion` | contexto | Revalidar identidad, permiso, tarea, ruta, trámite y proveedor | Nueva |
| `ResolverCapacidades` | `Modelo/.../ImportarServicioWebInterfaces.vb` | `IExternalImportProvider` | contexto | Puerto de capacidades sin infraestructura | Nueva |
| `ConsultarElementos` | `Modelo/.../ImportarServicioWebInterfaces.vb` | `IExternalImportProvider` | contexto, continuación, tamaño | Puerto de consulta sin infraestructura | Nueva |
| `AlmacenaDocumentoTareaWorkflow` | `workflow/ClassAlmacenamiento.vb` | `ClassAlmacenamiento` | parámetros legacy existentes | Almacenamiento documental vigente | Existente, no modificada |
