<!-- opsxj:refinement-traceability version=1 artifact=design decisions=D-01,D-02,D-03,D-04,D-05,D-06,D-07 -->

## Context

La capacidad `ANEXOS_RADICADO_ENLASE` ya consulta y previsualiza anexos por el proveedor SII moderno. La escritura histórica entra por `WebService_integracion_sii.SeviceGuardaDocumentoAnexoSII` y termina en `ClassAlmacenamiento.PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`, que depende de sesión, descarga el recurso, arma metadatos, llama `AlmacenaDocumentoTareaWorkflow`, puede actualizar la imagen de la tarea y retorna texto. La plataforma moderna ya dispone de preflight, intención, guard de concurrencia, máquina de estados, pasos de ejecución y reconciliación.

## Goals / Non-Goals

**Goals**

- Reutilizar el núcleo moderno para importar uno o varios anexos con una sola intención.
- Aislar el almacenamiento legacy y confirmar sus efectos autoritativamente.
- Evitar duplicados frente a reintentos, concurrencia y respuestas perdidas.
- Entregar estados por elemento y conservar el documento interno autorizado.

**Non-Goals**

- Reescribir `AlmacenaDocumentoTareaWorkflow` o duplicar `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`.
- Asignar, cerrar o avanzar la tarea.
- Modificar la semántica de constancias SII.
- Simular progreso mediante múltiples ejecuciones independientes.

## Decisions

### D-01 — Preparación como colección y tipología autorizada

`PreflightImport` seguirá siendo el punto único de preparación. El backend resolverá cada tipología con el catálogo y el trámite autorizados. Individual y múltiple usarán la misma colección; un valor predeterminado solo se aplicará si existe una coincidencia única. Ausencia, ambigüedad o divergencia bloquean antes de escritura.

### D-02 — Una intención idempotente por selección

`CreateImportIntent` creará o reutilizará una intención para toda la selección normalizada. La huella canónica incluirá usuario/contexto de tarea, ruta, trámite, proveedor, capacidad y pares de identidad externa/tipología; excluirá URL temporales. `IImportIntentConcurrencyGuard` serializará la ejecución y el repositorio conservará resultados por elemento.

### D-03 — Revalidación ENLASE inmediatamente antes del efecto

El endpoint reconstruirá el contexto desde sesión y fuentes autorizadas. `ValidadorContextoImportacion` y la resolución de capacidad comprobarán usuario, tarea operable, actividad/ruta, trámite, proveedor y `ANEXOS_RADICADO_ENLASE`. El orquestador repetirá la validación después de adquirir el guard y antes de descarga/almacenamiento. Una diferencia termina sin efectos y con código seguro.

### D-04 — Adaptador único para almacenamiento legacy

Un adaptador que implemente `IImportDocumentStoragePort` traducirá el comando moderno a `PreAlmacenaDocumentoAnexosEnlaceIntegracionSII`. No habrá copia del cuerpo ni llamada HTTP al ASMX legacy. El adaptador aislará la dependencia de sesión, capturará el ID devuelto y mapeará texto/`YES` a resultado estructurado y sanitizado. `YES` solo indica que la llamada terminó; la confirmación se decide en D-05.

### D-05 — Evidencia lógica y física para confirmar

La identidad externa se persistirá asociada con el ID interno. El repositorio de reconciliación buscará la intención en el mismo contexto y comprobará: registro del elemento, documento interno, relación con la tarea y recurso físico accesible. Coincidencia completa produce `Disponible`; registro lógico sin archivo produce un estado recuperable que permite una nueva ejecución controlada; contradicción o imposibilidad de saber produce `ResultadoIncierto`/`Inconsistente` sin exponer el ID como confirmado.

### D-06 — Agregación honesta y reconciliación explícita

`ImportServiceOrchestrator` conservará una transición por elemento. La respuesta distinguirá guardado, omitido idempotente, fallido antes de persistencia, parcial e incierto. Si algún elemento no queda confirmado, el agregado no será `Completado`. Un resultado incierto no se reintenta automáticamente; `ReconcileImportIntent` vuelve a consultar evidencia autorizada por intención y, opcionalmente, identidad externa.

### D-07 — Límites, pruebas y rollback

El flujo no invocará asignación/cierre ni pasos de expediente exclusivos de constancias. Los errores públicos serán códigos y mensajes saneados. Se agregarán pruebas de caracterización legacy, unitarias, integración de repositorios, concurrencia y antirregresión. La prueba E2E real reutilizará la plataforma y solo se ejecutará con autorización explícita. El rollback retirará composición/adaptador ENLASE y conservará intacta la ruta legacy.

## Risks / Trade-offs

- La dependencia legacy de `HttpContext.Session` exige un adaptador estrecho; cualquier contexto implícito no caracterizado debe bloquear el punto afectado.
- Un `YES` con respuesta perdida puede corresponder a un efecto real; por eso se prioriza reconciliar sobre reintentar.
- Verificar existencia física puede requerir una abstracción adicional sobre el repositorio documental; una verificación parcial nunca se elevará a éxito.
- Permitir recuperación por archivo ausente debe protegerse con guard e identidad externa para no duplicar registros válidos.

## Migration Plan

1. Incorporar contratos/resultados faltantes de manera aditiva.
2. Implementar repositorios/validadores y pruebas sin activar la ejecución ENLASE.
3. Incorporar adaptador y pasos del orquestador detrás de la capacidad existente.
4. Ejecutar suites estáticas/unitarias/integración y caracterización no mutadora.
5. Preparar E2E real en la plataforma existente; ejecutarla solo con autorización explícita y confirmar restauración del gate.
6. Rollback: retirar la composición ENLASE moderna; no revertir ni modificar datos confirmados y reconciliar intenciones abiertas.

## Open Questions

No quedan preguntas de diseño que bloqueen el inicio. La implementación debe detenerse y documentar una desviación si no puede verificar físicamente el recurso o si la función legacy requiere autoridad adicional no derivable del contexto servidor.