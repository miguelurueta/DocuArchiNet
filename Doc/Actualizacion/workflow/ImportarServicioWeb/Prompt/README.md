# Prompts de modernización de ImportarServicioWeb

Esta carpeta divide la modernización en entregas verificables. La fuente funcional y arquitectónica es `../Exploracion/01-exploracion-modernizacion-importar-servicio-web.md`; el modelo visual es `../Exploracion/02-modelo-ui-importar-servicio-web-moderno.html`.

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
