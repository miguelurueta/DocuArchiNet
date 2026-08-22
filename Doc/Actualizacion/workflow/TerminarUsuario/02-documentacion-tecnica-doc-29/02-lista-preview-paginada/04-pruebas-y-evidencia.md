# Pruebas, evidencia y riesgos — Lista preview

- Ticket: DOC-29
- Cambio OpenSpec: doc-29-interfaz-moderna-enviar-usuario
- Clasificacion: cross_cutting

## Pruebas y evidencia

`tests/workflow-user-send-ui.test.cjs` verifica la normalización del preview, el `POST` mínimo con `same-origin`, debounce de 300 ms, cursores, invalidación de respuestas obsoletas, foco del modal y ausencia de `IdConector`. La batería focal DOC-29 se ejecutó localmente y terminó con 61 pruebas correctas; no usa red, sesión ni datos de Workflow.

Los estilos y la prueba de presentación verifican que el modal de usuario tiene geometría estable y que el trigger nuevo conserva las clases de acción de transferencia. El contrato de servidor y las consultas de lectura se cubren también en `tests/workflow-user-send.test.cjs`.

## QA/E2E WebForms

El operador entregó la grabación `Grabación 2026-08-21 174246-comportamiento-todos.mp4` como evidencia de QA visual. La revisión de fotogramas distribuidos entre los segundos 0 y 36 cubre búsqueda, estado transitorio, resultados, selección, recarga y cierre. La lista se actualiza sin cambiar la geometría del modal, incluido el instante transitorio sin filas durante la búsqueda; se acepta la corrección visual de estabilidad. La grabación no ejercita todas las rutas de paginación ni teclado, que quedan cubiertas por la batería CJS.

El preview E2E autenticado se ejecutó con `test:doc28:preview` en el ambiente local autorizado. Las huellas de estado y auditoría fueron idénticas antes y después de consultar la página de destinos, confirmando que no hubo transición ni auditoría nueva. La ejecución E2E autorizada posterior confirmó los cambios esperados de estado y auditoría usando la selección vigente del preview. No se ejecutaron carga ni activación de gate. Antes de una prueba autenticada se lee `tools/e2e/AGENT-RUNBOOK.md`; las consultas de control son `SELECT` y el gate debe finalizar desactivado.

## Riesgos residuales

La experiencia de datos reales depende de la ruta y de los destinos autorizados por el backend. Una modificación futura del contrato de cursor o de los campos mostrados exige revisar tabla, tarjetas, paginación, estados vacíos y las pruebas de obsolescencia antes de desplegar.
