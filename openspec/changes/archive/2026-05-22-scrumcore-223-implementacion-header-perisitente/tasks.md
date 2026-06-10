## 1. Header persistente robusto en DocumentosWorkbench

- [x] 1.1 Ajustar la estrategia de layout/scroll de `AppTreeTable` en `DocumentosWorkbench` para que el header permanezca visible durante scroll vertical.
- [x] 1.2 Revisar y ajustar el contenedor del listado (`listSurface`/panel) para evitar doble scroll conflictivo y garantizar comportamiento estable en desktop/tablet.
- [x] 1.3 Confirmar que la solucion quede encapsulada en `DocumentosWorkbench` sin impacto global en otros consumidores de `AppTable`.

## 2. No-regresion funcional de lista documental

- [x] 2.1 Verificar que se mantenga un unico documento activo por click de visualizacion.
- [x] 2.2 Verificar que la seleccion multiple por checkbox se mantenga para acciones masivas sin interferir con documento activo.
- [x] 2.3 Confirmar que la columna de acciones y `onActionTriggered` continúen operando correctamente.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar/ajustar pruebas de `DocumentosWorkbench` para cubrir header visible y no-regresion de comportamiento.
- [x] 3.2 Ejecutar pruebas del modulo afectado y registrar resultado (pass/fail).
- [x] 3.3 Documentar diff final y decisiones de alcance local (sin impacto global en `AppTable`).
