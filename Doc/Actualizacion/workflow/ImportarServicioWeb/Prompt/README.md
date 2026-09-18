# Prompts de modernización de ImportarServicioWeb

Esta carpeta divide la modernización en entregas verificables. La fuente funcional y arquitectónica es `../Exploracion/01-exploracion-modernizacion-importar-servicio-web.md`; el modelo visual es `../Exploracion/02-modelo-ui-importar-servicio-web-moderno.html`.

El contrato normativo entre ambos lados y el orden cruzado de implementación están en [`../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md`](../CONTRATO-COMPARTIDO-FRONTEND-BACKEND.md). Los prompts frontend no pueden inventar ni anticipar una operación backend que allí figure como dependencia no publicada.

## Orden de ejecución

1. `01-nucleo-importacion-y-registro-adaptadores.md`
2. `02-adaptador-integracionsii-consulta-y-listado.md`
3. `03-vista-segura-recursos-externos.md`
4. `04-preparacion-individual-y-multiple.md`
5. `05-progreso-y-resultados-parciales.md`
6. `06-reconciliacion-y-lista-documentos.md`
7. `07-proteccion-contexto-tarea-y-recuperacion.md`
8. `08-pruebas-gate-y-retiro-legacy.md`

## Alcance reducido aprobado

- Una sola consulta `QueryItems` obtiene elementos y metadatos; filtros y catálogo no repiten llamadas SII.
- El preview usa `GetPreview` para obtener un descriptor y el handler seguro de B10 para visualizar o descargar.
- `PreflightImport` informa requisitos, tipología, destino lógico y efectos previstos sin crear ni revelar expediente físico.
- Uno o varios elementos crean una sola intención y una sola llamada síncrona a `ExecuteImportIntent`.
- Durante la ejecución existe espera global indeterminada; resultados y lista documental se actualizan al finalizar.
- La espera usa un indicador propio del feature; no importa, adapta ni invoca `JSProgresBar`, que permanece exclusivamente legacy.
- No hay progreso individual en tiempo real, ejecución por inscripción, cancelación en curso ni reintento automático.

Cada prompt debe implementarse y validarse dentro de su propio cambio OpenSpec o dentro de tareas atómicas explícitamente trazadas. No se debe ejecutar E2E real ni activar gates sin autorización expresa para el ambiente y las cuentas de prueba.

## Frontera común

- El núcleo no conoce campos ni reglas SII.
- `INTEGRACIONSII` es el primer adaptador, no el comportamiento predeterminado para proveedores desconocidos.
- El frontend no simula persistencia, autorización, idempotencia, reconciliación ni progreso en código productivo.
- Todo documento confirmado debe reconciliarse con la tarea original y aparecer en su lista de documentos.
- El recorrido legacy permanece disponible con el gate desactivado hasta completar la validación autorizada.
- En la ruta moderna, `ImportServiceOrchestrator` es el único ejecutor. El frontend envía una sola intención, muestra espera global durante `ExecuteImportIntent` y presenta los resultados por elemento únicamente al finalizar.
- Después de la respuesta, cada item `Disponible` con `DocumentId` se incorpora una sola vez a la lista documental de la tarea indicada por `TaskId`; el frontend nunca espera progreso individual en tiempo real.
- La traducción de `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` pertenece al adaptador backend; el frontend moderno no interpreta esos códigos.
- La implementación es paralela y aditiva; no reemplaza ni modifica endpoints, handlers o recorridos legacy.
- Está prohibido modificar `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` o sus consumidores existentes desde cualquier prompt frontend.
- El gate compartido es `WorkflowCentroTrabajoModernActive` y debe respetar las reglas de habilitación y restauración del contrato normativo.
- La UI moderna no puede habilitarse en producto hasta que todos los endpoints validen el alcance completo del gate: booleano, usuario y grupo autorizados.

## Raíces canónicas

- Código del feature: `js/workflow/importar-servicio-web/`.
- Estilos: `Styles/importar-servicio-web-modern.css`.
- Integración aditiva de página: `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`.
- Pruebas focales: `Tests/importar-servicio-web-*.test.cjs`.
- Fixtures compartidos: `Tests/Fixtures/Workflow/ImportarServicioWeb/`.
- Validación local: `tools/validation/`.
- E2E autorizable: `tools/e2e/tests/importar-servicio-web-modern.spec.cjs`.
- Documentación: `docs/modulos/workflow/importar-servicio-web/SCRUMCORE-<ID>-<alcance>/`.

No crear `src/app`, `src/modules`, otra raíz frontend ni paquetes documentales duplicados.
