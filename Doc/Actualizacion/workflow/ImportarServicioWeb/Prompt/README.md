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

Cada prompt debe implementarse y validarse dentro de su propio cambio OpenSpec o dentro de tareas atómicas explícitamente trazadas. No se debe ejecutar E2E real ni activar gates sin autorización expresa para el ambiente y las cuentas de prueba.

## Frontera común

- El núcleo no conoce campos ni reglas SII.
- `INTEGRACIONSII` es el primer adaptador, no el comportamiento predeterminado para proveedores desconocidos.
- El frontend no simula persistencia, autorización, idempotencia, reconciliación ni progreso en código productivo.
- Todo documento confirmado debe reconciliarse con la tarea original y aparecer en su lista de documentos.
- El recorrido legacy permanece disponible con el gate desactivado hasta completar la validación autorizada.
- En la ruta moderna, `ImportServiceOrchestrator` es el único ejecutor; `JSProgresBar` solo presenta eventos y estados confirmados.
- La traducción de `YES`, `CTRL`, `CTRLRETURN` y `dato_lista` pertenece al adaptador backend; el frontend moderno no interpreta esos códigos.
- La implementación es paralela y aditiva; no reemplaza ni modifica endpoints, handlers o recorridos legacy.
- Está prohibido modificar `AlmacenaDocumentoTareaWorkflow(...)`, `ClassAlmacenamiento` o sus consumidores existentes desde cualquier prompt frontend.
- El gate compartido es `WorkflowCentroTrabajoModernActive` y debe respetar las reglas de habilitación y restauración del contrato normativo.

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
