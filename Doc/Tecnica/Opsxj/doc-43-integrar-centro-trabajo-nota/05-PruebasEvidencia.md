# INTEGRAR-CENTRO-TRABAJO-NOTA

- Ticket: DOC-43
- Cambio OpenSpec: doc-43-integrar-centro-trabajo-nota
- Clasificacion: cross_cutting (Transversal)
## Evidencia requerida

- [x] unit: `node --test tools/e2e/tests/doc43-notes-ui-policy.test.cjs`; 8/8 PASS; verificación de gate, contrato, versión, `textContent`, estados, cambio de tarea, accesibilidad, doble envío y captura segura de credenciales efímeras.
- [x] manual_qa: capturas Chromium del modelo aprobado en 375, 768, 1024 y 1440 px, además de móvil horizontal; referencia `.opsxj/evidence/doc43-qa-*.png`.
- [x] e2e: `npm.cmd --prefix tools/e2e run test:doc43:notes-ui`; 1/1 PASS en 20.1 s; CRUD autorizado con contenido especial sobre una tarea descartable, sin secretos en la evidencia y con restauración del gate.

## QA/E2E WebForms

La etapa automatizada reutiliza la autenticación y configuración de `tools/e2e`. El runner solicita autorización explícita, recibe secretos únicamente por TTY y ejecuta el recorrido moderno con una tarea descartable proporcionada en tiempo de ejecución.

### Resultado E2E DOC-43

- Resultado: PASS, 1 prueba en 20.1 s.
- Cobertura: selección explícita de tarea, creación, consulta visible, edición y eliminación de una nota propia con Unicode, comillas, salto de línea y cadena tipo XSS renderizada como texto.
- Seguridad: credenciales y sesión permanecieron efímeras; el resultado registrado no contiene secretos ni identificadores de usuario.
- Configuración posterior: `WorkflowCentroTrabajoModernActive=false`, `WorkflowCentroTrabajoModernUsers` vacío y `WorkflowCentroTrabajoModernGroups` vacío.
- Limitación: la corrida valida Chromium en el ambiente autorizado; la matriz responsive permanece respaldada por QA visual estática separada.
