# Alcance de revisión y pendientes

## Archivos inspeccionados completamente

Repositorio `DocuArchiNet`:

- cuatro archivos de `js/workflow/importar-servicio-web/`;
- `Styles/importar-servicio-web-modern.css`;
- bloques DOC-72 en `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`;
- appSettings relevantes de `Web.config` y contenidos del `.vbproj`;
- `webservice/WebServiceImportarServicioWebModern.asmx.vb`;
- `DTOs/Workflow/ImportarServicioWeb/ImportarServicioWebDtos.vb`;
- `Modelo/Workflow/ImportarServicioWeb/ImportarServicioWebInterfaces.vb`;
- pruebas DOC-72, gate y regresión legacy;
- `.github/workflows/opsxj-validation.yml`.

## Dependencias revisadas para relación, no exhaustivamente

Se verificaron llamadas y composición de `ImportServiceOrchestrator`, `ServicioPreflightImportacion`, `ServicioIntencionImportacion`, `ServicioReconciliacionImportacion`, registros de proveedor y repositorios MySQL visibles en `Compose`. No se volvió a auditar internamente cada algoritmo, consulta SQL, gateway físico o adaptador SII: corresponden a DOC-50–DOC-71 y sus paquetes técnicos.

## Inconsistencias corregidas

1. Se eliminó la afirmación implícita de frontend productivo multiproveedor: backend acepta hoy solo SII.
2. Se separó “método API disponible” de “caso de uso visible implementado”.
3. Se aclaró que `reconciliando` está modelado pero no alcanzado por el adaptador/UI actual.
4. Se documentó que la ejecución única es por instancia JavaScript, no garantía distribuida.
5. Se reemplazó el diagrama conceptual sin firmas por cuatro diagramas trazables a símbolos exactos.
6. Se explicitó que la prueba documental verifica estructura, no fidelidad conductual completa.

## Pendientes que impiden declarar cobertura total

- QA WebForms autenticada, UpdatePanel real y lector de pantalla.
- Validación visual en navegadores/zoom soportados.
- Definición operativa de `ImportarServicioWebProviderId` en ambientes autorizados.
- Controles UI para selección, tipología, preflight, intención, ejecución y reconciliación.
- Revisión exhaustiva de implementaciones backend heredadas y SQL, fuera del alcance DOC-72.
- Verificación contra proveedor/red/base de datos real, prohibida sin autorización explícita.

Por estos pendientes, este paquete declara cobertura completa únicamente sobre símbolos y comportamiento estático del núcleo DOC-72 y su frontera inmediata, no sobre el subsistema ImportarServicioWeb completo.
