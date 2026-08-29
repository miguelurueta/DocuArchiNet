<!-- opsxj:refinement-traceability version=1 artifact=tasks decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08 -->
## 1. Refinamiento documental completado

- [x] 1.1 Delimitar DOC-40 a la fundación interna de Notas en Workflow y registrar exclusiones de UI, datos, endpoints y módulos externos. Origen: D-01, RQ-01
- [x] 1.2 Verificar el patrón actual de gate, puerto de tarea, interfaces y repositorio parametrizado que se reutilizará sin adaptar la clase legacy. Origen: D-02, RQ-05
- [x] 1.3 Definir contratos, resultados funcionales, decisiones diferidas y estrategia de evidencia no productiva. Origen: D-03, RQ-03
- [x] 1.4 Sincronizar decisiones, requisitos, plan de tareas y documentación técnica del cambio. Origen: D-07, RQ-07
- [x] 1.5 Incorporar el patrón de rutas Workflow: contexto `IdRutaWorkflow`, `IdRuta` de tarea y metadatos de ruta resueltos únicamente en servidor. Origen: D-08, RQ-08

## 2. Fundación de código — requiere autorización explícita

- [x] 2.1 Crear modelos y DTOs internos de Notas en ubicaciones coherentes de Workflow para las seis operaciones definidas. Origen: D-04, RQ-04
- [x] 2.2 Crear interfaces de servicio y repositorio, incluido el puerto de acceso a tarea con `contexto` e `idTarea` explícitos. Origen: D-02, RQ-02
- [x] 2.3 Implementar el gate específico de Notas con identidad, grupo y permiso derivados solo de sesión autenticada y comportamiento fail-closed. Origen: D-03, RQ-03
- [x] 2.4 Implementar la base de servicio y resultados funcionales sin exponer ASMX, UI ni operaciones de persistencia. Origen: D-04, RQ-04
- [x] 2.5 Preparar el repositorio parametrizado de Workflow sin copiar, envolver ni extender `Class_anotacion_tarea`. Origen: D-05, RQ-05
- [x] 2.6 Incorporar la validación de ruta en el snapshot autorizado de tarea; no aceptar ruta, nombre de tabla ni metadatos desde una solicitud de Notas. Origen: D-08, RQ-08

## 3. Pruebas locales — requiere autorización explícita

- [x] 3.1 Agregar pruebas unitarias de gate para sesión incompleta, identidad o grupo inválidos y permiso ausente. Origen: D-03, RQ-03
- [x] 3.2 Agregar pruebas de contratos para `idTarea` obligatorio, `idNota` por nota y resultados funcionales seguros. Origen: D-02, RQ-04
- [x] 3.3 Ejecutar la compilación y las pruebas focales sin base real, registrar comandos, resultado y evidencia saneada. Origen: D-07, RQ-07
- [x] 3.4 Agregar pruebas de ruta ausente, inválida e incoherente, incluido el rechazo de cualquier metadato de ruta recibido desde cliente. Origen: D-08, RQ-08

## 4. Límites de integración — requiere decisión y autorización posteriores

- [x] 4.1 Resolver formalmente la política de borrado, histórico, supervisión, retención, contenido, auditoría e idempotencia antes de proponer una operación mutante. Origen: D-06, RQ-06
- [x] 4.2 Diseñar la primera fase que exponga endpoint o consumidor de Workflow y asociar su E2E autorizada al mismo cambio. Origen: D-01, RQ-07

## 5. Cierre técnico — requiere autorización explícita

- [x] 5.1 Revisar compatibilidad con el flujo legacy de Workflow y demostrar ausencia de dependencia de sesión en la tarea objetivo moderna. Origen: D-01, RQ-02
- [x] 5.2 Completar evidencia de pruebas, validar OpenSpec y documentar archivos finales antes de solicitar cierre. Origen: D-07, RQ-07
- [x] 5.3 Revisar que el acceso de Notas conserve la ruta en el snapshot y solo use identificadores técnicos derivados de metadatos validados en servidor. Origen: D-08, RQ-08
