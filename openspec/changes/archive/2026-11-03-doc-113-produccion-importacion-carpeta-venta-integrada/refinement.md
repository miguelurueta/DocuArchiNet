<!-- opsxj:refinement version=1 state=approved -->

# DOC-113 — Refinamiento de brechas de la ventana integrada

## Fuente y alcance

Ticket DOC-113; fuente Jira inalterada en specs/produccion-importacion-carpeta-venta-integrada/jira-context.md. Tecnología observada: WebForms/VB.NET Framework 4.6.1, JavaScript y ASMX/ASHX; MySQL solo en servicios existentes. Base main 1b579739 con DOC-112 integrado. Este corte no modifica producto ni ejecuta autenticación/carga.

## Contexto inspeccionado

- `js/gestion/ImportacionCarpetas/InventarioImportacion.js`.
- `js/gestion/ImportacionCarpetas/ImportacionCarpetas.js`.
- `js/gestion/ImportacionCarpetas/ImportacionCarpetasApi.js`.
- `js/gestion/ImportacionCarpetas/ImportacionCarpetasVista.js`.
- `js/gestion/ImportacionCarpetas/ImportacionCarpetas.css`.
- `tests/doc111-importacion-carpetas-client.test.cjs`.
- `Gestion/WebFormProducionDocumental.aspx`.
- `Gestion/WebFormProducionDocumental.aspx.vb`.
- `js/gestion/WebFormProducionDocumental.js`.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/00-guia-ejecucion-prompts.md`.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/03-prompt-implementacion-carga-carpetas.md`.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Prototipo/README.md`.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/02-contratos-operacion.md`.
- `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc111-matriz-final-20261102.md`.
- `tools/e2e/tests/support/doc111-importacion-carpetas.cjs`.
- `tools/e2e/scripts/support/interactive-e2e-console.cjs`.
- `tools/e2e/tests/doc111-importacion-carpetas.spec.cjs`.
- `tools/e2e/AGENT-RUNBOOK.md`.

## Decisiones refinadas

| ID | Decisión verificable | Evidencia | Design | Requirement | Tasks |
|---|---|---|---|---|---|
| D-01 | Conservar el host RenderImportacionCarpetas, el controlador, inventario, API, vista y CSS existentes. DOC-113 no crea otra ventana, árbol, backend o cliente HTTP; mantiene etapas 1–3 y el contenedor de resultados, preservando consulta/recuperación/CSV ya integrados por 11. | Gestion/WebFormProducionDocumental.aspx:67; .aspx.vb:RenderImportacionCarpetas; ImportacionCarpetas.js:montar/inicializar; contratos comunes DOC-112. | D-01 | RQ-01 | Origen: D-01, RQ-01 |
| D-02 | Validar la estructura de los datos de AbrirImportacion, ListarPerfilesImportacion y PrepararPlan antes de asignarlos al modelo. Cubrir objetos anidados y elementos de colecciones usados por la vista; una respuesta inválida muestra error accesible, invalida confirmación y conserva una pantalla utilizable. Validación de forma cliente, sin duplicar reglas de negocio del servidor. | ImportacionCarpetas.js:action preparar/run; ImportacionCarpetasVista.js:render; reproducción local con Incidencias=[null]. | D-02 | RQ-02 | Origen: D-02, RQ-02 |
| D-03 | Conservar mountedHost, reutilización del host y destrucción de instancia al sustituirlo. Añadir regresión para inicialización repetida, sustitución/desaparición del host con apertura en vuelo y respuesta tardía, cierre/reapertura y foco vigente. No añadir hooks ni cambiar WebForms por inferir que falta limpieza. | ImportacionCarpetas.js:inicializar/destruir/cerrar; Vista:destruir/teclado; falta caso explícito de reemplazo/UpdatePanel en suite cliente. | D-03 | RQ-03 | Origen: D-03, RQ-03 |
| D-04 | Añadir una especificación focal dentro de tools/e2e que use la ventana y el árbol reales. UI-01 comprueba destino A fijo al elegir perfil y navegar a B; UI-02 selección nativa de raíces homónimas/subcarpetas e invalidación de revisión; UI-03 teclado/foco, reapertura, postback parcial y error visible. Reutilizar helpers de sesión y consultas, sin sustituir las acciones por invocaciones REC. | Ticket UI-01/02/03; matriz final DOC-111 RQ-11 delimita reapertura/driver frente al recorrido visual completo. | D-04 | RQ-04 | Origen: D-04, RQ-04 |
| D-05 | Reutilizar openSession, authenticated-workflow-session y interactive-e2e-console. Un selector fino puede registrar la suite UI en el paquete existente; no crea login, .env, configuración Playwright o plataforma paralela. No heredar autorización de cargas DOC-111: UI-02 puede persistir planes o confirmar lote y requiere aprobación específica de ambiente/cuentas/recursos/efectos. No ejecutar recepción, creación o almacenamiento en estos escenarios. | tools/e2e/tests/support/doc111-importacion-carpetas.cjs:openSession/validateProfile; scripts/support/interactive-e2e-console.cjs; AGENTS.md y runbook. | D-05 | RQ-05 | Origen: D-05, RQ-05 |
| D-06 | Mantener evidencia por caso y SHA/huella, distinguiendo Chromium sintético de IIS autenticado. Ejecutar regresión cliente y MSBuild Framework conforme al ticket, con salidas aisladas y sin despliegue. UI-01/02/03 no se dan por aceptados con mocks; si falta autorización o ambiente se registra NO_EJECUTADO, dependencia y responsable, sin cerrar la entrega integrada. | 13 pruebas cliente PASS el 2026-11-03; reproducción sintética sin tráfico HTTP; suites reales UI-01/02/03 aún no implementadas ni ejecutadas en DOC-113. | D-06 | RQ-06 | Origen: D-06, RQ-06 |

## Requisitos verificables

| ID | Resultado observable | Criterio de aceptación | Compatibilidad |
|---|---|---|---|
| RQ-01 | Una ventana con destino autorizado estable | El sistema SHALL reutilizar la ventana integrada existente y conservar el nivel capturado al abrir, validado por servidor, aunque se seleccione otro perfil o nodo; preservar la carga individual y el seguimiento ya integrado. | Reutiliza contratos/componentes y preserva almacenamiento, SII y configuración. |
| RQ-02 | Respuesta inválida con error recuperable | El sistema SHALL rechazar datos de respuesta incompatibles con la estructura que consume la interfaz antes de mostrarlos o habilitar confirmación; dejar un error accesible y conservar la posibilidad de revisar o cerrar. | Reutiliza contratos/componentes y preserva almacenamiento, SII y configuración. |
| RQ-03 | Ciclo de vida sin eventos duplicados | El sistema SHALL conservar una instancia por host, liberar recursos al retirarlo o cerrar y rechazar respuestas tardías; teclado y foco deben permanecer utilizables durante reapertura y actualización parcial. | Reutiliza contratos/componentes y preserva almacenamiento, SII y configuración. |
| RQ-04 | Escenarios visuales UI-01 a UI-03 verificables | La entrega SHALL proporcionar recorridos automatizados focales de UI-01, UI-02 y UI-03 sobre el árbol y los componentes reales, con aserciones de UI/HTTP e identificación de efectos autorizados. | Reutiliza contratos/componentes y preserva almacenamiento, SII y configuración. |
| RQ-05 | Autorización y herramientas compartidas | La entrega SHALL reutilizar la infraestructura E2E existente y exigir autorizaciones específicas del escenario antes de autenticar o persistir recursos; no solicitar secretos por chat ni crear infraestructura o configuración paralelas. | Reutiliza contratos/componentes y preserva almacenamiento, SII y configuración. |
| RQ-06 | Evidencia suficiente para cierre | La entrega SHALL identificar por escenario la comprobación local o real, SHA y huella probados, comandos y limitaciones, conservando procedencia de DOC-111 y contratos comunes; no cerrar integración real con pruebas sintéticas. | Reutiliza contratos/componentes y preserva almacenamiento, SII y configuración. |

## Resultado del refinamiento

Se sustituyen ocho tareas genéricas por doce tareas atómicas: tres S y nueve M, sin L. Una corrección de producto demostrada y cobertura local/real focalizada; no otra implementación integral de la ventana. La suite local heredada pasó 13/13 y la sonda reprodujo el fallo de Incidencias=[null], sin login ni tráfico externo. Los seis D/RQ cubren el plan completo.

El marcador permanece draft: no se registra aceptación editorial automática. La ejecución real de UI-01/02/03 requiere aprobación específica del ambiente/cuentas/recursos/efectos; hasta entonces los casos reales se declaran NO_EJECUTADO. No se omiten del criterio de cierre ni se sustituyen por mocks.

## Implementación local autorizada por continuación del flujo

Se completaron diez tareas independientes del ambiente; las corridas reales y su consolidación final siguen abiertas. F-01 se corrige con validación de datos anidados antes del modelo. La regresión de foco identificó F-02 y se corrigió en el mismo controlador, dentro de RQ-03, sin nuevos hooks. Cliente 15/15 y política/driver 43/43 PASS, MSBuild Framework aislado código 0, siete scripts con sintaxis válida y tres casos reales registrados pero sin ejecutar. El selector prepara el perfil base con nombres DOC-113 nuevos y no hereda autorizaciones.

El marcador sigue draft hasta aceptación editorial; la instrucción de continuar permitió implementar localmente y no autoriza login o mutaciones del sistema. Las referencias alternativas de nivel/publicación y el alcance específico se confirman antes de autenticar. No hay despliegue del candidato ni de la DLL compilada.

## Verificación final real — 2026-11-03

Estado posterior a la planificación y notas locales anteriores: implementación técnica 12/12, cliente 16/16 y driver 43/43 PASS; UI-01/UI-02/UI-03 reales EJECUTADO_OK. JavaScript servido verificado por SHA y aplicado con autorización específica DOC-113; DLL solo en salida aislada. A=7/B=8, publicaciones 38/39; planes 8/9 conservados, dos preparaciones, cero confirmaciones/cargas. El fallo inicial de UI-03 se reprodujo como selector de prueba que elegía un diálogo legacy oculto y se corrigió sin modificar el producto. Informe final: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md. Las frases de no ejecución en secciones previas describen la línea base histórica. La aprobación editorial sigue separada: no se cambia el marcador draft por inferencia de la autorización E2E.

## Aprobación editorial — 2026-11-03

El usuario respondió «aprobado» al cierre técnico 12/12 y a la continuación de publicación/cierre. Aprobación vinculada al reporte Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-verificacion-final-20261103.md SHA256 f0d84ee388169f0e40ac7ffbcba2bf0876d373f667e5e8c23469798e06850f42. Los estados draft de cortes previos son históricos. Se conserva el alcance: dos planes ya preparados, cero confirmaciones/cargas; no se autorizan otros efectos del runtime.
