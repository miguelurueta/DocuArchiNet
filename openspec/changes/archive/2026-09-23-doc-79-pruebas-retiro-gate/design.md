<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

## Context

DOC-79 cierra la transición verificable de Importar Servicio Web bajo el gate canónico. El repositorio ya contiene contratos, módulos frontend, pruebas focales y una plataforma E2E reutilizable; faltan consolidación, cobertura específica de UI/gate y criterios de retiro legacy.

## Goals / Non-Goals

**Goals**
- Validar localmente los contratos frontend y de coexistencia sin red.
- Demostrar gate backend completo y alternancia UI reversible.
- Consolidar evidencia E2E existente y documentar inventario/rollback.

**Non-Goals**
- Eliminar controles, handlers, ASMX o almacenamiento legacy.
- Crear infraestructura E2E, perfiles, autenticación o configuración paralela.
- Activar el gate o ejecutar pruebas autenticadas sin autorización explícita.

## Decisions

### D-08 — Fidelidad mínima al modelo UX validado

La experiencia moderna conserva la composición esencial del prototipo de exploración: modal ajustado al viewport, contexto y etapas, tabla semántica con encabezado fijo, acciones visibles sin depender del desplazamiento horizontal y visor embebido. La transición a preparación elimina explícitamente el modo preview para impedir composiciones superpuestas.

### D-09 — La existencia física prevalece sobre el antecedente de importación

Una intención completada demuestra que el documento existió, no que continúa en el repositorio. El estado del listado cruza `intent_id + document_id` con el gabinete registrado y comprueba la fila física. Si fue eliminada, la inscripción vuelve a `Disponible`; si no se puede determinar un gabinete seguro, queda `ConNovedad` y no se habilita una reimportación a ciegas.

### D-10 — Recibo y código de barras se resuelven desde la tarea confiable

Los campos ocultos del navegador no son autoridad y pueden contener valores transitorios como `-1`. `QueryItems` resuelve conjuntamente recibo y código de barras mediante el contexto validado de tarea y ruta, devuelve el recibo para presentar el radicado real y usa el código de barras exclusivamente para consultar SII. `CreateImportIntent` repite la resolución y sobrescribe cualquier radicado enviado por el cliente para impedir inconsistencias o manipulación entre consulta y confirmación.

### D-11 — `NoAplica` es un resultado final válido y la ausencia física verificada tiene precedencia

En modo sin expediente, relación y caché terminan legítimamente en `NoAplica`; no son errores ni estados pendientes. La clasificación histórica acepta `Confirmado` o `NoAplica` como efectos resueltos. Cuando existe al menos una importación completada y verificable cuyo documento ya no está físicamente, esa ausencia permite reimportar aunque haya intentos fallidos anteriores. La presencia de cualquier documento físico conserva la precedencia y mantiene `Importado`.

### D-12 — Reconciliación visual parcial sin recarga completa

La finalización no recarga toda la página. Cuando no existe una inserción DOM segura, reutiliza `Button_actualiza_trevie_seleccion`, control WebForms existente dentro del `UpdatePanel` de documentos, para solicitar la proyección autoritativa. El modal conserva su estado de resultado mientras ocurre la actualización parcial. Los controles de preparación reciben estilos propios y no dependen de reglas globales legacy.

### D-13 — Cierre gobernado por resultado terminal

Desde la creación de la intención hasta recibir un resultado terminal, el modal no puede cerrarse por botón, backdrop ni teclado. El bloqueo es explícito y accesible. Un resultado exitoso o parcial se proyecta mediante la actualización parcial y cierra automáticamente el modal al concluir; un fallo terminal libera el cierre pero permanece visible para no ocultar información diagnóstica.

### D-14 — Visor PDF compatible sobre recurso local mediado

El recurso temporal se sirve exclusivamente desde el handler local, ligado a sesión/tarea/proveedor, con allowlist de tipos no ejecutables, `inline`, `nosniff` y `SAMEORIGIN`. El iframe no usa `sandbox`, porque ese aislamiento bloquea el visor PDF integrado del navegador. La UI diferencia descriptor disponible de documento cargado: anuncia carga, confirma en `load` y ante `error` oculta el visor roto y ofrece renovar el descriptor.

### D-15 — Selección total limitada a elementos importables

La cabecera de la lista ofrece un checkbox para seleccionar o deseleccionar en bloque. Solo modifica filas cuya acción `Import` está autorizada; nunca fuerza filas deshabilitadas. Su estado se sincroniza como marcado, desmarcado o indeterminado y gobierna el botón `Preparar seleccionados` junto con los checkboxes individuales.

### D-16 — Tipología predeterminada inequívoca y revalidada

La preparación asigna a todos los elementos la única tipología autorizada cuando el catálogo contiene una sola opción. Si contiene varias, solo predetermina una coincidencia única cuyo nombre normalizado contiene `Constancia` e `Inscripción`. Cero o varias coincidencias conservan la selección manual. La elección permanece editable y no evita el preflight ni la resolución autoritativa del servidor.

### D-01 — Validador local único
`tools/validation/Verify-ImportarServicioWebFrontend.ps1` compondrá las suites Node focales sin autenticación ni red. Las cuatro suites canónicas agregarán cobertura reutilizando contratos existentes. Relacionado con RQ-01.

### D-02 — Gate global en servidor
La validación comprobará las ocho operaciones modernas, el corte anterior a dependencias/efectos y `FEATURE_DISABLED`. Con la bandera activa, cualquier sesión Workflow válida queda habilitada; no se duplicará la autorización funcional mediante listas de usuarios o grupos. El gate visual nunca sustituye la validación backend de sesión. Relacionado con RQ-02.

### D-03 — Alternancia UI sin doble ejecución
La vista conservará el árbol legacy como fallback. Cuando el gate moderno esté activo se ocultará inicialmente ese árbol y existirá una sola entrada y un solo handler efectivo; apagado conservará el recorrido anterior. Relacionado con RQ-03.

### D-04 — Contrato de ejecución visible
Las pruebas verificarán una llamada `ExecuteImportIntent` por intención, espera global sin porcentajes ficticios y reconciliación final de todos los documentos confirmados sin duplicados. Relacionado con RQ-04.

### D-05 — E2E compartida y autorizada
Se extenderá únicamente la suite compartida cuando falte una aserción estructural. Se reutilizarán escenarios y evidencia disponibles; cualquier corrida real seguirá el runbook y restaurará el gate en `finally`. Relacionado con RQ-05.

### D-06 — Retiro legacy separado
DOC-79 producirá inventario de controles, postbacks y handlers, clasificando referencias y evidencia. La eliminación física será otro cambio con autorización propia. Relacionado con RQ-06.

### D-07 — Documentación canónica
El paquete técnico vivirá solo en `Doc/Actualizacion/workflow/ImportarServicioWeb/DOC-79-pruebas-gate-transicion-legacy/`, con evidencia saneada y diagrama. No se recreará `docs/`. Relacionado con RQ-07.

## Risks / Trade-offs

- Las pruebas estructurales pueden acoplarse a texto: se limitarán a contratos públicos e invariantes de seguridad.
- Ocultar legacy sin eliminarlo conserva superficie técnica, pero mantiene rollback inmediato.
- Si la evidencia E2E previa no cubre una aserción nueva, se declarará el bloqueo y se solicitará autorización, sin simular resultados.

## Migration and Rollback

1. Incorporar primero pruebas y validador local.
2. Aplicar el ocultamiento reversible sin borrar controles legacy.
3. Validar con gate apagado y activado solo en un ambiente autorizado.
4. Restaurar siempre `WorkflowCentroTrabajoModernActive=false`, usuarios y grupos vacíos.
5. Ante regresión, apagar el gate y conservar el recorrido legacy; no revertir datos ni reejecutar intenciones.
