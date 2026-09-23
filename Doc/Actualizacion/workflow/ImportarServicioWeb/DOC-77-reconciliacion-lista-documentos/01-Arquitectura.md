# Arquitectura

- Ticket: DOC-77
- Cambio OpenSpec: doc-77-reconciliacion-lista-documentos
- Clasificacion: cross_cutting

## Objetivo

DOC-77 convierte el resultado estructurado de DOC-76 en una vista consistente con el estado persistido. El backend sigue siendo la única fuente de verdad: el navegador no consulta SII, no almacena documentos y no interpreta contratos delimitados legacy.

## Componentes

`ImportarServicioWebReconciliation` encapsula `GetImportIntent` y `ReconcileImportIntent`. Normaliza únicamente el contrato moderno y reconcilia cada identidad externa incierta una sola vez.

`ImportarServicioWebDocumentListAdapter` valida la tarea visible, filtra confirmados, deduplica por `DocumentId` y encapsula la proyección sobre la lista existente. Cuando no recibe un contrato visual seguro, solicita recargar la vista completa.

`ImportarServicioWebUi` compone ambos módulos después de `ExecuteImportIntent`. No inserta durante la espera global y conserva el adaptador de apertura autorizada que resuelve el documento contra una fila renderizada por el servidor.

## Límites

- Los endpoints se consumen solo mediante `importar-servicio-web-api.js`.
- No se modifican ASMX, persistencia, `ClassAlmacenamiento` ni `AlmacenaDocumentoTareaWorkflow(...)`.
- No se modifica ni se interpreta `insert_row_documento_relacionado(...)` o `dato_lista`.
- Los archivos nuevos se registran en `GestionDocumental-Docuarchi.net.vbproj` y `workflow/Webworkflow.aspx.vb`.

## Reversión

La implementación es aditiva. Para revertir se retiran los dos módulos, sus registros y la composición DOC-77 en la UI; no existe migración de datos.
