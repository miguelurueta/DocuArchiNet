<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04 -->
## 1. Contexto y contrato

- [x] 1.1 [S] Inventariar integración actual. Área/archivos: `workflow/Webworkflow.aspx(.vb)`, `js/workflow/Webworkflow.js`. Origen: D-01, RQ-01. Verificación: rutas, eventos y fallback documentados.
- [x] 1.2 [S] Confirmar contratos modernos de Notas. Área/archivos: `webservice/`, DTOs y DOC-42. Origen: D-02, RQ-02. Verificación: operaciones y payloads identificados.

## 2. Bootstrap y compatibilidad

- [x] 2.1 [M] Integrar bootstrap condicionado por gate. Área/archivos: `workflow/Webworkflow.aspx(.vb)`. Origen: D-01, RQ-01. Verificación: gate inactivo carga solo legacy.
- [x] 2.2 [S] Preservar GridView, postbacks y eventos legacy. Área/archivos: `workflow/Webworkflow.aspx(.vb)`. Origen: D-01, RQ-01. Verificación: no hay doble ejecución.

## 3. Adaptador cliente

- [x] 3.1 [M] Crear adaptador único con JSON real. Área/archivos: `js/workflow/Webworkflow.js`. Origen: D-02, RQ-02. Verificación: sin JSON concatenado.
- [x] 3.2 [M] Propagar `idTarea` explícito. Área/archivos: `workflow/Webworkflow.aspx(.vb)`, `js/workflow/Webworkflow.js`. Origen: D-02, RQ-02. Verificación: sin `Session("ID_TAREA_SELECCIONDA")`.
- [x] 3.3 [M] Implementar listado, paginación y contador. Área/archivos: `js/workflow/Webworkflow.js`. Origen: D-02, RQ-02. Verificación: una sola actualización por cambio de tarea.
- [x] 3.4 [M] Integrar crear, editar y eliminar con bloqueo de doble envío. Área/archivos: `js/workflow/Webworkflow.js`. Origen: D-02, RQ-02. Verificación: máximo una mutación por acción.

## 4. UI, seguridad y accesibilidad

- [x] 4.1 [M] Renderizar datos con API segura de texto. Área/archivos: `js/workflow/Webworkflow.js`. Origen: D-03, RQ-03. Verificación: sin `innerHTML` para Notas y prueba XSS negativa.
- [x] 4.2 [M] Implementar estados de carga, vacío, error, éxito y conflicto. Área/archivos: adaptador y marcado. Origen: D-03, RQ-03. Verificación: controles recuperables.
- [x] 4.3 [M] Encapsular estilos y controles accesibles. Área/archivos: `.workflow-centro-trabajo-moderno`. Origen: D-03, RQ-03. Verificación: foco, teclado, Escape y objetivos de 40 px.
- [x] 4.4 [M] Implementar la composición visual tomando como base el modelo aprobado. Área/archivos: `Doc/Actualizacion/workflow/Notas/Exploracion/modelo-ui-notas-workflow-moderno.html`, `workflow/Webworkflow.aspx`, estilos del consumidor. Origen: D-03, RQ-03. Verificación: lista cronológica, estados y jerarquía visual coinciden con el HTML de referencia sin introducir una grilla nueva.

## 5. Pruebas y QA

- [x] 5.1 [M] Probar contenido especial, cambio de tarea y conflictos. Área/archivos: pruebas cliente. Origen: D-03, RQ-03. Verificación: Unicode, XSS y conflictos cubiertos.
- [x] 5.2 [M] Ejecutar QA responsive. Área/archivos: matriz de evidencia. Origen: D-04, RQ-04. Verificación: 375/768/1024/1440 px y ambas orientaciones móviles.
- [x] 5.3 [L] Ejecutar E2E autorizada con `tools/e2e`. Área/archivos: `tools/e2e`. Origen: D-04, RQ-04. Verificación: evidencia saneada y gate apagado.

## 6. Documentación y cierre

- [x] 6.1 [M] Crear la documentación técnica del DOC-43 siguiendo la estructura documental vigente. Área/archivos: `Doc/Actualizacion/workflow/Notas/DOC-42-TRANSACIONES-NOTAS/` como referencia estructural y nueva carpeta documental del DOC-43. Origen: D-01, RQ-01. Verificación: índice, arquitectura, contrato, flujo/seguridad, pruebas/evidencia, inventario, liberación y rollback enlazados.
- [x] 6.2 [M] Documentar la correspondencia entre el modelo visual y la implementación. Área/archivos: `Doc/Actualizacion/workflow/Notas/Exploracion/modelo-ui-notas-workflow-moderno.html` y documentación del DOC-43. Origen: D-03, RQ-03. Verificación: cada estado, control y breakpoint del modelo tiene referencia en la matriz QA o en el diseño.
- [x] 6.3 [S] Registrar comandos, resultados y limitaciones sin secretos. Área/archivos: evidencia del cambio. Origen: D-04, RQ-04. Verificación: revisión de saneamiento.
- [x] 6.4 [S] Validar OpenSpec y trazabilidad. Área/archivos: `openspec/changes/doc-43-integrar-centro-trabajo-nota`. Origen: D-04, RQ-04. Verificación: validate strict y OPSXJ PASS.
