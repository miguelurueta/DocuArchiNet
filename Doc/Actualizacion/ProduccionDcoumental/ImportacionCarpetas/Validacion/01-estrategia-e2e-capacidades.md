# Comprobación progresiva de capacidades

Estado: estrategia documental; no hay nuevas suites implementadas ni E2E ejecutadas por redactarla. Los escenarios concretos y sus aserciones están en cada prompt 02–13 bajo «E2E POR CAPACIDAD Y CRITERIO DE CIERRE».

## Secuencia de verificación

| Hito | Qué debe comprobarse | Condición de avance |
|---|---|---|
| 04 Diseño | Contratos, fixtures sintéticos, observables y propietarios. | Revisión documental; no requiere aplicación nueva ejecutándose. |
| 02 Perfil | Crear, validar, publicar, recargar y proteger versiones/permisos. | PER-01–03 comprobados contra administración real; SQL aplicado por el equipo. |
| 07-A Fundación | Transacciones, unicidad, lectura/escritura y recuperación de repositorios. | Pruebas de integración de persistencia; no requieren UI ni 06. |
| 06 + 05 | Selección del nivel, inventario y preparación de plan. | PLAN y UI pasan con servicios reales; sin efectos documentales. |
| 07-B | Confirmación idempotente y recepción recuperable. | RECIB pasa; contenido íntegro y cero documentos definitivos. |
| 08/09 aislados | Contratos de adaptadores, efectos y ventanas de fallo. | Pruebas internas de integración; E2E pendientes hasta 10, sin endpoints auxiliares. |
| 10 + 08/09 + ventana | Expedientes, almacenamiento, barreras y recuperación. | EXP, ALM y COORD pasan desde recorrido integrado. |
| 11 | Consulta, CSV, reanudación, árbol y limpieza. | SEG pasa con estados persistidos. |
| 12 | Recorrido completo y regresión transversal. | REC e INT reutilizan escenarios anteriores y completan evidencia. |
| 13 opcional | Continuidad durable, reinicios y exclusión. | WORK pasa tras activar alcance y autorizar host. |

Una entrega puede acreditar sus pruebas de componente mientras su integración está pendiente. No convertir ese avance en aprobación E2E del comportamiento aún no disponible. Una capacidad con un caso crítico fallido o sin ejecutar permanece pendiente de validación; el informe indica la causa y el responsable.

## Infraestructura y ejecución

Reutilizar `tools/e2e/package.json`, `tools/e2e/playwright.config.cjs`, utilidades y autenticación compatibles. El repositorio dispone de `tools/e2e/tests/support/authenticated-workflow-session.cjs`; comprobar su aplicabilidad a Producción antes de usarlo, sin inventar tareas Workflow para una importación. Si requiere extensión, hacerla compatible y probar sus consumidores. No crear otro login, configuración o proyecto Playwright. Los scripts Workflow existentes no validan automáticamente importación por carpetas.

Durante implementación, inventariar archivos de prueba y registrar comandos concretos por capacidad en el mismo paquete E2E. No anunciar comandos nuevos como disponibles antes de crearlos. Antes de autenticar, leer AGENTS y runbook; la experiencia moderna sigue siendo oficial, sin reintroducir claves de activación históricas aunque existan ejemplos antiguos en el runbook.

Separar credenciales de ejecución funcional y cuenta de observación de solo lectura. Los casos positivos de creación requieren autorización de escrituras en datos descartables mediante la aplicación; las consultas de control usan SELECT parametrizado. Preparación SQL la realiza el equipo. Limpieza y fallos deliberados requieren autorización específica, no actualizaciones directas de estados para hacer pasar un caso.

Usar fixtures mínimos: dos raíces distinguibles, raíces homónimas, subcarpeta, patrones de nombre distintos, ceros iniciales, archivo inválido y transferencia incompleta. Resolver desde el catálogo autorizado gabinete, nivel, tipología y responsables; no fijar IDs reales ni tratar el ejemplo de orden como formato universal.

Para fallos posteriores a persistencia, definir antes el punto de interrupción y el observable. Interrumpir una respuesta no demuestra por sí solo que la escritura terminó: verificar IDs, relaciones y estados. Las inyecciones deben quedar restringidas al entorno de pruebas y no introducir controles públicos ni modificar SII. Si no puede reproducirse con seguridad, registrar el caso pendiente.

## Evidencia por corrida

Registrar en `Validacion/resultados.md` (a crear al ejecutar) una fila por escenario: ID, SHA, ambiente autorizado, precondiciones, pasos/comando real, resultado esperado/observado, comprobación persistida, evidencia saneada, estado y causa del bloqueo. Capturas/logs se guardan según la política existente, sin secretos ni documentos reales.

La UI prueba presentación; HTTP prueba contrato; la observación de expediente, gabinete, relación e índice prueba efectos. Un HTTP 200, un porcentaje completo o un JSON PASS del revisor de prompts no sustituyen esas comprobaciones.
