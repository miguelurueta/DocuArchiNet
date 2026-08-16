<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05 -->
## 1. Refinamiento

- [x] 1.1 Inspeccionar el enlace Continuar, `GridView_envia_flujo`, modal legacy, ASMX, feature gates y DTOs actuales. Origen: D-01, RQ-01.
- [x] 1.2 Registrar la diferencia entre el gate de página y `IWorkflowModernFeatureGate`, y el mapeo real de los campos ASMX. Origen: D-02, RQ-02.
- [x] 1.3 Aprobar la representación limitada al DTO actual: usar radicado, tipo y grupo actual; omitir trámite y actividad actual legible sin ampliar el backend. Origen: D-03, RQ-03.
- [x] 1.4 Marcar el refinamiento aprobado y sincronizar la trazabilidad con `opsxj:refine -- DOC-12 --sync`. Origen: D-05, RQ-05.

## 2. Integración Presentation

- [x] 2.1 Implementar el bootstrap de servidor que evalúa `IWorkflowModernFeatureGate` y emite el atributo de activación de la lista. Origen: D-02, RQ-02.
- [x] 2.2 Integrar en `workflow/Webworkflow.aspx` el host del modal y las referencias condicionales de JS/CSS, registrar los assets en `GestionDocumental-Docuarchi.net.vbproj` y preservar el enlace, grid y modal legacy con el gate inactivo. Origen: D-01, RQ-01.
- [x] 2.3 Crear `workflow-transition-ui.js` con inicialización exclusiva cuando el bootstrap está activo e intercepción segura de Continuar sin postback moderno. Origen: D-02, RQ-02.
- [x] 2.4 Implementar el cliente de `PreviewEnviarTarea`: solicitud de mismo origen, validación del envoltorio ASMX y normalización de errores de red o contrato. Origen: D-03, RQ-03.
- [x] 2.5 Renderizar el contexto permitido y los estados cargando, sin destinos y error controlado mediante APIs DOM seguras. Origen: D-03, RQ-03.
- [x] 2.6 Renderizar destinos como tabla compacta en escritorio y como tarjetas en móvil, mostrando únicamente nombre, destinatario/grupo y tipo disponibles. Origen: D-03, RQ-03.
- [x] 2.7 Implementar selección de destino y callback con tarea, conector, token y resumen visible; verificar que no invoca ninguna operación de envío. Origen: D-04, RQ-04.
- [x] 2.8 Implementar foco inicial, cierre con Escape, foco atrapado, navegación por teclado y retorno de foco al enlace disparador. Origen: D-04, RQ-04.
- [x] 2.9 Crear `workflow-transition-modern.css` para modal, estados, foco visible, contraste y adaptación tabla/tarjetas sin afectar estilos globales. Origen: D-04, RQ-04.

## 3. Pruebas y evidencia

- [x] 3.1 Agregar pruebas focales del mapeo ASMX, contexto permitido y estados de carga, vacío y error. Origen: D-03, RQ-03.
- [x] 3.2 Agregar pruebas focales de gate inactivo, selección, callback y ausencia de llamada a `EjecutarEnvioTarea`. Origen: D-04, RQ-04.
- [x] 3.3 Compilar el proyecto o solución afectada y registrar el comando, resultado y limitaciones reales. Origen: D-05, RQ-05.
- [x] 3.4 Ejecutar QA manual reproducible en escritorio y móvil para foco, Escape, teclado, contraste, gate activo y gate inactivo. Origen: D-05, RQ-05.
- [x] 3.5 Ejecutar E2E autenticada solo con ambiente y cuentas de prueba autorizados; comprobar que abrir o seleccionar no cambia tarea ni auditoría. Origen: D-05, RQ-05.

## 4. Documentación y cierre

- [x] 4.1 Crear `00-indice.md`, `01-arquitectura.md` y `02-contrato.md` en `Doc/Actualizacion/workflow/Terminar/04-lista-moderna/`. Origen: D-01, RQ-01.
- [x] 4.2 Crear `03-flujo-y-seguridad.md`, diagramas Mermaid y documentar callback, interfaz legacy preservada, piloto y rollback. Origen: D-04, RQ-04.
- [x] 4.3 Crear `04-pruebas-y-evidencia.md`, registrar resultados y ejecutar la validación OpenSpec y de gobierno. Origen: D-05, RQ-05.
