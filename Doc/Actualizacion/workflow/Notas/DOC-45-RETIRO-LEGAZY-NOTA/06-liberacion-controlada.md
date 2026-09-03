# Liberación controlada

## Secuencia de cierre

1. Validar OpenSpec y su trazabilidad D-01 a D-10 / RQ-01 a RQ-10.
2. Ejecutar la política DOC-45 y las regresiones aplicables de DOC-42, DOC-43 y DOC-44.
3. Compilar la solución con cero errores y registrar por separado advertencias preexistentes.
4. Confirmar ausencia de controles y llamadas legacy de Notas en `Webworkflow`, conservando contratos externos vivos.
5. Verificar acceso con contador, modal estable, visor completo, confirmador auxiliar y gestión exclusiva del propietario.
6. Comprobar colores de acciones e índice y cargar las URLs versionadas de CSS/JavaScript.
7. Con autorización explícita, ejecutar `test:doc44:workflow-notes` sobre tarea descartable y nota ajena válida.
8. Sobre una tarea inicialmente vacía autorizada, ejecutar `test:doc45:empty-notes` y comprobar la restauración de `Nueva nota 0`.
9. Confirmar que la selección asíncrona funciona sin `page.reload` y que el acceso recién renderizado abre al primer clic.
10. Confirmar al finalizar `WorkflowCentroTrabajoModernActive=false` y usuarios/grupos vacíos.
11. Revisar diff, evidencia saneada, riesgos y rollback integral antes del merge.

## Evidencia de cierre disponible

- Regresión definitiva de propiedad, visor y CRUD sobre el ejecutor estabilizado: PASS 1/1, 19.6 segundos totales.
- Estado vacío con creación y limpieza de la nota temporal: PASS 1/1, 17.1 segundos totales.
- Acción `Asignar` sobre tarea no tomada, sin mutación: PASS 1/1, 15.7 segundos totales.
- Política DOC-45: PASS 4/4.
- OpenSpec estricto y `git diff --check`: PASS en la validación previa.
- Gate final: `false`, usuarios y grupos vacíos.

Código, pruebas focales, compilación, E2E real autorizada y evidencia saneada forman una única unidad de entrega. Una evidencia anterior a una reapertura no sustituye la corrida sobre la implementación final.
