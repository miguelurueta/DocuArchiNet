# Flujo de integración

## Flujo principal

1. El usuario abre el modal y consulta documentos SII.
2. El adaptador renderiza cada fila con `ExternalKey`; nunca expone una URL como autoridad.
3. Al elegir **Vista previa**, la UI captura foco y scroll y pasa a `preparando`.
4. El controlador llama `api.getPreview` con contexto confiable, proveedor e identidad externa.
5. ASMX valida gate, sesión, tarea y proveedor; obtiene bytes y crea un descriptor temporal.
6. El navegador valida el descriptor y carga el handler same-origin una sola vez.
7. Cerrar o volver limpia el frame y restaura el contexto anterior.

## Flujos alternos

- MIME no visualizable: aparece descarga temporal por el mismo handler.
- Descriptor vencido: aparece renovación explícita; no se muta tarea ni auditoría.
- Gate/B10/proveedor ausente: estado bloqueado o no disponible, sin fallback externo.
- Acceso rechazado: estado no autorizado sin detalle interno.
- Documento importado reconciliado: el adaptador busca una coincidencia exacta en la lista documental server-rendered, verifica la tarea y delega al postback del visor vigente. Sin coincidencia autorizada, la acción permanece oculta.

No existe procesamiento batch en preview. La selección múltiple de importación permanece independiente.
