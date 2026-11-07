<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06 -->
# DOC-113 — Ventana integrada: reutilización y brechas

## Contexto y alcance

Base integrada main `1b579739da06b33b9a638b97309176d2f6ee93da`, con DOC-110/111/112. El encargo es interfaz ejecutable y evidencia, a diferencia del cierre documental DOC-112. El contenedor, las etapas, servicios y seguimiento ya existen. No se implementa otra ventana ni se convierte toda la lista del ticket en funcionalidades nuevas. Este refinamiento autoriza planificar; los checkboxes son trabajo futuro y la aceptación editorial aún no se registra.

La fuente de contratos sigue en `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Diseno/`; se referencia el inventario existente sin copiar otro diseño físico. El contexto Jira se conserva íntegro en `specs/produccion-importacion-carpeta-venta-integrada/jira-context.md`.

## Decisiones refinadas

### D-01 — Reutilizar la ventana y fijar fronteras

Conservar el host RenderImportacionCarpetas, el controlador, inventario, API, vista y CSS existentes. DOC-113 no crea otra ventana, árbol, backend o cliente HTTP; mantiene etapas 1–3 y el contenedor de resultados, preservando consulta/recuperación/CSV ya integrados por 11.

Fuente: Gestion/WebFormProducionDocumental.aspx:67; .aspx.vb:RenderImportacionCarpetas; ImportacionCarpetas.js:montar/inicializar; contratos comunes DOC-112. Origen: RQ-01.

### D-02 — Rechazar respuestas que la vista no puede consumir

Validar la estructura de los datos de AbrirImportacion, ListarPerfilesImportacion y PrepararPlan antes de asignarlos al modelo. Cubrir objetos anidados y elementos de colecciones usados por la vista; una respuesta inválida muestra error accesible, invalida confirmación y conserva una pantalla utilizable. Validación de forma cliente, sin duplicar reglas de negocio del servidor.

Fuente: ImportacionCarpetas.js:action preparar/run; ImportacionCarpetasVista.js:render; reproducción local con Incidencias=[null]. Origen: RQ-02.

### D-03 — Acreditar el ciclo de vida existente

Conservar mountedHost, reutilización del host y destrucción de instancia al sustituirlo. Añadir regresión para inicialización repetida, sustitución/desaparición del host con apertura en vuelo y respuesta tardía, cierre/reapertura y foco vigente. No añadir hooks ni cambiar WebForms por inferir que falta limpieza.

Fuente: ImportacionCarpetas.js:inicializar/destruir/cerrar; Vista:destruir/teclado; falta caso explícito de reemplazo/UpdatePanel en suite cliente. Origen: RQ-03.

### D-04 — Cubrir UI-01/02/03 mediante el DOM real

Añadir una especificación focal dentro de tools/e2e que use la ventana y el árbol reales. UI-01 comprueba destino A fijo al elegir perfil y navegar a B; UI-02 selección nativa de raíces homónimas/subcarpetas e invalidación de revisión; UI-03 teclado/foco, reapertura, postback parcial y error visible. Reutilizar helpers de sesión y consultas, sin sustituir las acciones por invocaciones REC.

Fuente: Ticket UI-01/02/03; matriz final DOC-111 RQ-11 delimita reapertura/driver frente al recorrido visual completo. Origen: RQ-04.

### D-05 — Reutilizar autenticación y exigir alcance de prueba

Reutilizar openSession, authenticated-workflow-session y interactive-e2e-console. Un selector fino puede registrar la suite UI en el paquete existente; no crea login, .env, configuración Playwright o plataforma paralela. No heredar autorización de cargas DOC-111: UI-02 puede persistir planes o confirmar lote y requiere aprobación específica de ambiente/cuentas/recursos/efectos. No ejecutar recepción, creación o almacenamiento en estos escenarios.

Fuente: tools/e2e/tests/support/doc111-importacion-carpetas.cjs:openSession/validateProfile; scripts/support/interactive-e2e-console.cjs; AGENTS.md y runbook. Origen: RQ-05.

### D-06 — Separar regresión local, integración real y cierre

Mantener evidencia por caso y SHA/huella, distinguiendo Chromium sintético de IIS autenticado. Ejecutar regresión cliente y MSBuild Framework conforme al ticket, con salidas aisladas y sin despliegue. UI-01/02/03 no se dan por aceptados con mocks; si falta autorización o ambiente se registra NO_EJECUTADO, dependencia y responsable, sin cerrar la entrega integrada.

Fuente: 13 pruebas cliente PASS el 2026-11-03; reproducción sintética sin tráfico HTTP; suites reales UI-01/02/03 aún no implementadas ni ejecutadas en DOC-113. Origen: RQ-06.

## Brecha funcional reproducida

PrepararPlan puede devolver un sobre tipado con resultado.Incidencias=[null]. El controlador comprueba que sea array pero no cada elemento; la vista accede a RaizIdCliente y lanza TypeError en el render posterior. La reproducción local dejó un diálogo sin mensaje y sin botón de confirmar, con busy=false. Dos operaciones sintéticas (abrir/preparar), cero solicitudes externas, sin credenciales ni servidor. Se corrige el límite de consumo de datos antes de asignar el modelo; no se acepta ocultar la excepción y dejar una revisión parcialmente válida.

## Evidencia de partida

La suite cliente existente pasó 13/13 el 2026-11-03 usando Chromium compartido y servicios sintéticos. Sus seis assets/ficheros de prueba coinciden con el worktree de DOC-113. La matriz final DOC-111 acredita REC y reapertura/driver, pero no los tres recorridos visuales completos de este ticket. La reproducción y la línea base se resumen en [gap-analysis.md](gap-analysis.md); no son una aceptación del arreglo todavía inexistente.

## Archivos y fronteras

Existentes: js/gestion/ImportacionCarpetas/ImportacionCarpetas.js, ImportacionCarpetasApi.js, ImportacionCarpetasVista.js, InventarioImportacion.js y CSS; Gestion/WebFormProducionDocumental.aspx/.aspx.vb y js/gestion/WebFormProducionDocumental.js; tests/doc111-importacion-carpetas-client.test.cjs. Solo el controlador tiene una corrección de producto demostrada en este corte; los demás se reutilizan y solo cambian si una regresión concreta lo justifica.

Nuevos propuestos, no creados: `tools/e2e/tests/doc113-ventana-importacion.spec.cjs`, `tools/e2e/tests/support/doc113-ventana-importacion.cjs`, `tools/e2e/scripts/run-doc113-ventana-importacion.cjs`. La suite usa la configuración/reporteros existentes. El selector fino delega en scripts/support/interactive-e2e-console.cjs y helpers de sesión, sin copiar autenticación, DSN o políticas REC. Para nivel B/versiones alternativas se admiten referencias no secretas validadas en argumentos del selector; el perfil base existente conserva su validación cerrada y no se debilita para aceptar campos desconocidos.

## Autorización y capacidades de prueba

No hay autorización nueva para autenticar o escribir por generar este plan. UI-01 exige niveles/perfiles autorizados; UI-02 puede persistir planes y confirmar un lote y debe autorizar esos efectos y recursos concretos; UI-03 usa una actualización parcial real y un error sin efectos dentro del alcance aprobado. El recorrido no pulsa recepción/avance documental. Una respuesta interceptada es evidencia local de UI y no prueba del backend. Los controles reutilizan DSN registrados y cuentas SELECT, nunca URLs MySQL introducidas en el chat. Secretos solo efímeros en consola compartida. AGENTS.md, guía 00 y runbook se aplican antes de autenticar; no se reintroducen flags ni listas de activación.

## Compatibilidad y validación

No se modifican almacenamiento/SII, tablas, DTO de negocio, permisos o Web.config. Se conserva consulta/CSV/reanudación/árbol ya integrados, aunque su propiedad original sea 11. Validación progresiva: regresión cliente y casos adversos locales; sintaxis; MSBuild Framework con salida aislada; suite focal UI real únicamente autorizada; matriz final con estados por caso. La falta de autorización/herramienta se documenta con responsable y no se resuelve simulando éxito. P04/P05 de capacidad/retención siguen bajo operación y no se inventan cuotas o navegadores. La reversa es el diff de controlador/pruebas/registro de DOC-113, sin rollback de recursos del ambiente.

## Implementación local — 2026-11-03

La instrucción de continuar el flujo autoriza esta implementación local; la aprobación editorial final y la autorización de corridas reales siguen separadas. Se validan los datos consumidos por la vista antes de asignarlos al modelo. La nueva regresión de cierre detectó F-02: el disparador se deshabilitaba antes de que la vista capturara el foco anterior, por lo que este quedaba en body al cerrar. El mismo controlador ahora captura el disparador antes de deshabilitarlo y lo enfoca después de restaurar el estado, solo si sigue conectado y el cierre no es forzado. No se añadieron hooks de ciclo de vida.

La suite UI, soporte y selector propuestos ya están implementados localmente; las tres corridas reales continúan NO_EJECUTADO. El registro de evidencia compartido DOC-110 acepta un ámbito explícito de archivos sin cambiar su comportamiento predeterminado. UI-03 altera una sola solicitud de apertura a nivel inválido 0, con aprobación específica, y verifica el rechazo real del servidor; no fabrica una respuesta HTTP. El selector UI-02 solo confirma si se solicita --confirm y se aprueba ese efecto.

El JavaScript servido se compara por SHA256 con el candidato local antes de actuar: no se acepta ejecutar contra otra versión. El candidato y la DLL compilada permanecen en el worktree/salida aislada, sin despliegue al ambiente. Resultados y bloqueos: `Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md`.

## Verificación final real — 2026-11-03

Estado posterior a la planificación y notas locales anteriores: implementación técnica 12/12, cliente 16/16 y driver 43/43 PASS; UI-01/UI-02/UI-03 reales EJECUTADO_OK. JavaScript servido verificado por SHA y aplicado con autorización específica DOC-113; DLL solo en salida aislada. A=7/B=8, publicaciones 38/39; planes 8/9 conservados, dos preparaciones, cero confirmaciones/cargas. El fallo inicial de UI-03 se reprodujo como selector de prueba que elegía un diálogo legacy oculto y se corrigió sin modificar el producto. Informe final: Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc113-ventana-integrada.md. Las frases de no ejecución en secciones previas describen la línea base histórica. La aprobación editorial sigue separada: no se cambia el marcador draft por inferencia de la autorización E2E.
