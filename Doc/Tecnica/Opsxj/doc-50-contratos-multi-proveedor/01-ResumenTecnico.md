# CONTRATOS-MULTI-PROVEEDOR

- Ticket: DOC-50
- Cambio OpenSpec: doc-50-contratos-multi-proveedor
- Clasificacion: cross_cutting (Transversal)
## Objetivo

Publicar el núcleo contractual v1 de `ImportarServicioWeb` con contexto autorizado e inmutable, registro explícito de proveedores y fachada de capacidades/consulta sin sesión, red ni persistencia.

## Alcance y compatibilidad

- [x] Componentes afectados: seis archivos VB nuevos en DTOs, Modelo y Services; tres suites CommonJS; ocho fixtures; proyecto VB y especificaciones OpenSpec. No se modificaron páginas, controles ni ASMX.
- [x] Comportamiento preservado: recorrido legacy y `AlmacenaDocumentoTareaWorkflow(...)` intactos. Reversa aditiva mediante retiro de archivos y entradas `Compile Include`, sin migración de datos.
