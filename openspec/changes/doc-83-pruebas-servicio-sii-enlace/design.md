<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09 -->
## Context

DOC-83 cierra la validación transversal de la importación moderna de anexos SII ENLASE implementada por DOC-80, DOC-81 y DOC-82. El alcance es pruebas, evidencia, gate, compatibilidad y operación reversible; no es una cuarta implementación funcional.

## Goals / Non-Goals

**Goals**
- Convertir riesgos altos y muy altos en una matriz trazable de prueba, evidencia y criterio de cierre.
- Reutilizar la plataforma E2E y las suites focales existentes.
- Demostrar separación entre lectura, importación y asignación.
- Mantener gate y legacy restaurables y fallar cerrado ante evidencia incompleta.

**Non-Goals**
- Crear proveedor, endpoint, modal, login, gate o almacenamiento nuevos.
- Retirar legacy o cambiar la política de asignación.
- Ejecutar E2E real sin autorización vigente, o repetir una mutación ya demostrada sin necesidad técnica.
- Corregir producción debilitando una expectativa de prueba.

## Decisions

### D-01 — Cierre orientado a evidencia, no nueva funcionalidad

DOC-83 añade pruebas, validadores y documentación. Una falla productiva se conserva como hallazgo y requiere refinamiento explícito antes de cualquier corrección de lógica.

### D-02 — Inventario único de cobertura

Una matriz versionada relacionará cada riesgo y condición de cierre con requisito, prueba determinista, escenario E2E, autorización, mutabilidad, controles y evidencia. La prueba documental fallará si falta una fila obligatoria o referencia registrada.

### D-03 — Reutilización de tres recorridos existentes

`import-sii-enlase-read` demuestra lectura/preview sin cambios; `import-sii-enlase-ui` demuestra interacción real no mutadora; `import-sii-enlase-execution` demuestra intención, persistencia física y ausencia de transición. DOC-83 los orquesta y extiende solo donde exista una brecha verificable.

### D-04 — Fronteras de autorización independientes

Lectura exige ambiente y gate. Importación exige además ejecución y recurso descartable. Una asignación exitosa, si debe ejecutarse, usa autorización y reserva E2E independientes de la importación. Por defecto se prepara otra tarea; reutilizar la misma exige autorización expresa y una precondición funcional nueva verificable. Ninguna autorización se hereda entre corridas.

### D-05 — Gate global existente y rollback verificable

Se conserva `WorkflowCentroTrabajoModernActive`; no se crea gate ENLASE paralelo. La transformación `Web.Release.config` fija `true` y mantiene vacías las listas de usuarios y grupos, por lo que cualquier sesión Workflow válida accede a la capacidad. La configuración base local/E2E permanece en `false`; el runner es el único autorizado para habilitarla temporalmente y debe restaurar exactamente esa línea base en `finally`. Gate apagado conserva la superficie legacy y no enlaza bootstrap moderno.

### D-06 — Evidencia mínima, saneada y autoritativa

Los controles son `SELECT` registrados. La evidencia conserva códigos, conteos, huellas, aserciones y eventos de recurso; excluye cuerpos SII, credenciales, cookies, tokens, cadenas de conexión e identidades innecesarias. Una señal visual no sustituye persistencia física o controles de tarea.

### D-07 — Idempotencia, reimportación explícita y aislamiento por tarea

La matriz debe cubrir intención única, selección múltiple, repetición/recuperación sin duplicados, existencia física y rechazo de respuestas de otra tarea. La identidad es `ExternalKey`, no la tipología: dos claves diferentes del mismo tipo son documentos distintos. Una clave confirmada solo puede volver a importarse cuando la selección transporta `ReimportRequested=true`; el plan registra `DOCUMENT_REIMPORT_EXPLICIT`. La reimportación es aditiva y conserva el documento anterior para auditoría; este cambio no implementa eliminación ni sustitución destructiva. Un antecedente cuyo recurso físico verificable fue eliminado vuelve a estar disponible. La tarea ENLASE se determina por su estado activo actual y no por ciclos históricos finalizados.

### D-08 — Asignación explícita conserva autoridad legacy

Importar nunca asigna. La cobertura verifica que `Buttonaceptar_Click` revalida documentos obligatorios y que una asignación real solo se prueba con autorización mutadora separada. DOC-83 no inventa `ValidateAssignment` ni anuncia habilitación preventiva.

### D-09 — Cierre fallando cerrado

No se declara validación completa si falta autorización E2E, recurso descartable, proveedor, integridad legacy, restauración o evidencia. El resultado se registra como bloqueo explícito; OPSXJ solo continúa cuando la matriz obligatoria está satisfecha o documenta una limitación externa aceptada.

## Risks / Trade-offs

- Las evidencias DOC-80/81/82 pertenecen a corridas y recursos concretos; deben citarse sin presentarlas como cobertura universal de ambientes.
- El gate global acopla temporalmente varias capacidades modernas, pero crear otro gate en un ticket de pruebas aumentaría el riesgo y excedería el alcance.
- Asignar una tarea es una mutación distinta de importar; ambas operaciones conservan reservas y autorizaciones independientes. Reutilizar la tarea subyacente es una excepción trazada, no una autorización heredada.
- Casos cero/uno/múltiples dependen de datos externos reales; la matriz debe distinguir cobertura determinista de observación E2E.
- La cardinalidad documental legacy continúa dependiendo de `UNICO` en la configuración del trámite. Los trámites que admiten dos recibos de caja deben configurarlos con `UNICO=0`; DOC-83 no modifica datos de configuración del ambiente.
- El estado remoto del SII puede impedir una corrida; el fallo se conserva y no se reemplaza con mocks cuando el criterio exige integración real.

## Validation Plan

1. Inventariar suites, escenarios, perfiles, controles y evidencias DOC-80/81/82.
2. Ejecutar pruebas deterministas de contratos, autorización, gate, legacy, UI, persistencia, idempotencia y documentación.
3. Añadir escenarios/políticas faltantes sin duplicar infraestructura.
4. Solicitar autorización solo al llegar a una E2E real concreta y declarar su mutabilidad.
5. Comprobar restauración del gate y ausencia de cambios no autorizados incluso ante fallos.
6. Registrar resultados y limitaciones en documentación DOC-83 y evidencia OPSXJ.

## Rollback

Los cambios de DOC-83 son pruebas/documentación. Su rollback retira esos artefactos sin tocar producción. Durante E2E, el runner restaura el contenido original de configuración; si falla la integridad final, la corrida se detiene y no se declara éxito ni se continúa con otra etapa.

## Refinamiento de proyección visual ENLASE

La prueba manual demostró que `GridView_list_documento_relacion` y `GridView_list_documento_relacion_wf` no comparten contrato. ENLASE usa `id_rad`/`idd_rad`; sus interacciones requieren `gabinete|documentId|radicado|DBT|nombre|taskId|firma|icono`. Insertar solamente ID, nombre y tarea produce una fila visible pero inválida: `SolicitaIdTipoImagen` recibe un gabinete vacío al abrirla.

La corrección no hace `DataBind`, postback ni segunda consulta. `LegacyEnlaseImportDocumentStorageAdapter` convierte la `stru_datos_image_lista` que ya devuelve el almacenamiento exitoso en una proyección interna tipada. La ejecución la publica mediante un DTO de proyección solamente para documentos confirmados. Los adaptadores de progreso y reconciliación conservan esa proyección y el adaptador de lista valida identidad, tarea y campos obligatorios antes de traducirla, exclusivamente en la frontera cliente, al contrato histórico consumido por `insert_row_documento_relacionado(..., "rad", 1)`.

La rama legacy sin tipología no asigna `DBT` ni `tipodocumental`, pero sí retorna `extension` como parte de la misma `stru_datos_image_lista`. `CrearProyeccion` usa `DBT` cuando existe y, únicamente cuando falta, usa esa extensión física ya confirmada. No consulta base de datos, no modifica `ClassAlmacenamiento.vb` y no inventa una clasificación documental.

El flujo convencional conserva el destino `wf`. ENLASE usa un adaptador y destino dinámicos para `GridView_list_documento_relacion`; nunca reutiliza el contrato mínimo de la tabla modernizada. Si falta la proyección completa, la ventana de resultado permanece abierta y no se crea una fila parcial. El DTO moderno no publica `dato_lista` ni permite que JavaScript invente gabinete, radicado o tipo físico.

La reconciliación normaliza tanto `Completada` como `Completado` al estado autoritativo `Completado`; una confirmación recuperada puede permanecer como `Disponible`. La regla de cierre ya reconoce ambos estados. El adaptador de lista debe usar exactamente el mismo conjunto cerrado (`Disponible`, `Completado`) antes de proyectar. El hallazgo E2E de `fix13` demostró que aceptar únicamente `Disponible` descartaba silenciosamente una importación efectivamente persistida antes de llamar a `insert_row_documento_relacionado`.

`ProjectExecutionResult` sigue siendo la autoridad que reemplaza el resultado de ejecución por el snapshot reconciliado. Como la proyección visual se deriva del resultado efímero de almacenamiento y no se persiste, esa sustitución debe conservarla únicamente para ENLASE y solo cuando coincidan `ClientItemId`, `ExternalKey`, documento, tarea y todos los campos obligatorios con el ítem autoritativo `Disponible`. Una consulta posterior no reconstruye ni inventa la proyección; ante cualquier diferencia se omite y la interfaz falla cerrada.
## Aceptación visual manual gobernada

La comprobación inmediata no puede realizarse dejando `Web.config` abierto fuera de la plataforma. El escenario `import-sii-enlase-manual-visual` reutiliza autenticación, selección oficial ENLASE, controles ODBC, reserva local, saneamiento e integridad del runner. Por ser una importación real, su etapa es `execution` y exige `environment`, `gate`, `execution` y `discardable-resource`.

El navegador se abre visible únicamente para este escenario. Después de seleccionar la tarea, el runner espera que `PageRequestManager` no tenga un postback asíncrono en curso, valida el bootstrap moderno, abre `#a_adj_service_web` y no habilita el hito manual hasta observar el modal en estado `resultados` con al menos una acción `Importar/Reimportar`. Solo entonces captura los identificadores existentes sin retirar ni ocultar filas. Envuelve temporalmente `insert_row_documento_relacionado`, resuelve el GridView vivo en cada llamada y registra únicamente filas nuevas con destino `rad`; también cuenta cualquier postback posterior a la línea base. El responsable ejecuta la importación y confirma en TTY cuando aparece una fila nueva. El runner verifica que el marcador continúe, que el modal haya cerrado, que no hubo postback, que el inserter agregó la fila y que `idd_rad` contiene los ocho campos con documento y tarea coherentes. Una segunda confirmación permite abrir esa fila y falla si aparece un diálogo de error.

Ambas confirmaciones comparten un plazo total máximo de diez minutos. Timeout, rechazo, cierre del navegador, recarga, ausencia de fila, contrato incompleto, diálogo o controles inconsistentes detienen la corrida. El `finally` común cierra navegador y sesión, elimina secretos, restaura el gate, comprueba legacy y conserva solo evidencia saneada.

## Refinamiento visual de columnas SII

La revisión posterior al E2E encontró que `white-space: nowrap` permitía que valores largos se pintaran fuera de celdas cuyo `max-width` no imponía recorte. Ambas variantes de la lista asignan ahora clases semánticas a encabezados y datos, anchos explícitos por columna, elipsis y `title` con el valor completo. La columna fija de acciones conserva ancho y fondo opacos. El contenedor sigue siendo el único dueño del scroll horizontal y vertical; no se ocultan columnas ni se desplaza la página.

## Refinamiento de preparación masiva y preview

La preparación conserva el preflight autoritativo, pero elimina la lista visual redundante “Plan previsto”. El panel se organiza como columna de altura acotada: encabezado y estado no crecen, únicamente `preparation-items` tiene scroll, y el footer con `Cancelar`/`Crear intención` permanece fuera de ese scroll. El botón de creación continúa deshabilitado hasta que el preflight termine correctamente.

La vista previa deja de forzar la lista oculta a mostrarse en escritorio. Sustituye temporalmente el contenido de lista, conserva siempre visible `Volver a documentos` y distribuye encabezado, ayuda, estado, iframe y acciones en una columna flex. El iframe ocupa solamente el espacio restante y no impone una altura que desplace verticalmente los controles.
