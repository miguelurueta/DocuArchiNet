# Evidencia

## Revisión UX del 23 de septiembre de 2026

Se revisó la grabación local aportada por el usuario (43,4 segundos) y se contrastó con `Exploracion/02-modelo-ui-importar-servicio-web-moderno.html`. La evidencia mostró: ausencia de encabezados, acciones desplazadas fuera del área inicial, fallo visual del visor PDF y superposición de la preparación con la lista. La corrección incorpora contexto y etapas, encabezado fijo, acciones ancladas, modal ajustado al viewport, permisos mínimos del visor PDF y limpieza del modo preview antes de preparar. No se copia el video al repositorio ni se registran datos sensibles.

Una verificación visual posterior detectó que la preparación no recibía las tipologías resueltas por `ResolveCapabilities` y que el encabezado consultaba el campo de radicado equivocado. El adaptador ahora conserva el catálogo autoritativo de capacidades y el bootstrap utiliza `Hidden_radicado_seleccion`, que representa la fila activa del centro de trabajo.

## Consistencia tras eliminación documental

La consulta del listado antes infería `Importado` exclusivamente desde la intención completada. Sin embargo, la eliminación legacy retira la fila del gabinete y conserva correctamente la historia. El estado actual ahora cruza la intención con `workflow_import_related_document`, valida el nombre del gabinete y consulta por `ID` la fila física. Una ausencia confirmada permite reimportar; una evidencia incompleta o no verificable queda bloqueada como novedad. La verificación es local, parametrizada y no agrega llamadas al proveedor SII.

## Contexto autoritativo y estado visible

La captura de la tarea `220584` evidenció un radicado `-1` y acciones deshabilitadas sin explicación. El valor provenía de estado transitorio del navegador. La consulta moderna ahora obtiene recibo y código de barras conjuntamente desde la tarea y ruta validadas en servidor, presenta el recibo retornado y repite esa resolución al crear la intención. El radicado enviado por el navegador ya no decide la persistencia. Además, la tabla incluye la columna `Estado`, de modo que el usuario puede distinguir un documento disponible de uno importado o con novedad; si el documento importado fue eliminado físicamente, la revalidación lo devuelve a `Disponible`.

Una captura posterior mostró una carga mixta: radicado autoritativo nuevo y tabla anterior sin `Referencia` ni `Estado`. También mostró vacía la inscripción porque el contrato backend publica ese metadato con código `registration` y el mapper del navegador solo reconocía alias de `inscription`. Se agregaron los alias `REGISTRATION/REGISTRO`, una ayuda contextual en `Preparar` cuando está bloqueado y una versión conjunta `doc79ux4` para mapper, adaptador, UI y estilos.

El bloqueo persistente reveló además que el clasificador histórico exigía `Confirmado` para relación y caché, aunque el flujo válido sin expediente concluye ambos efectos como `NoAplica`. Esto convertía una ejecución completada en `ConNovedad` antes de evaluar correctamente su eliminación. La corrección trata `Confirmado` y `NoAplica` como estados resueltos y separa la ausencia física verificada de las novedades históricas: un documento ausente vuelve a `Disponible`, un documento todavía presente conserva `Importado` y un antecedente imposible de verificar permanece `ConNovedad`.

La grabación local de 17,94 segundos aportada posteriormente mostró los botones internos con estilo nativo y una pantalla gris transitoria después de crear la intención. La revisión cuadro por cuadro confirmó que la causa era el fallback `window.location.reload()` usado al reconciliar la lista, no el cierre del backdrop. Se reemplazó por el control existente `Button_actualiza_trevie_seleccion`, contenido en el `UpdatePanel` de documentos, y se añadieron estilos explícitos para cierre, tipología, cancelación y creación de intención. El video y los fotogramas temporales no se incorporan al repositorio.

La captura final evidenció que el resultado exitoso permanecía abierto y que el modal conservaba rutas de cierre durante la ejecución. Se incorporó un bloqueo central desde la creación de intención hasta el resultado terminal, aplicado al botón superior, backdrop y Escape mediante el mismo guard. Después de un resultado exitoso o parcial, la UI espera el `endRequest` del `UpdatePanel` y cierra automáticamente. Ante fallo terminal libera el cierre y conserva el resultado visible.

Una captura posterior mostró el descriptor de preview disponible, pero el visor PDF del navegador bloqueado dentro del iframe sandboxed. El recurso continúa mediado por `ImportarServicioWebPreview.ashx`, ligado a usuario, tarea y proveedor, limitado a tipos permitidos y protegido con `inline`, `nosniff` y `SAMEORIGIN`. Se retiró exclusivamente el atributo `sandbox` incompatible con el visor PDF integrado y se añadieron estados reales de carga: la UI no anuncia documento listo hasta `load`; ante `error` oculta el marco roto y ofrece renovar el recurso.

La selección múltiple se completó con un checkbox accesible en la cabecera. Marca o desmarca exclusivamente las inscripciones con acción de importación autorizada, refleja selección parcial mediante estado indeterminado y mantiene sincronizado `Preparar seleccionados`; no altera filas `Importado` o `ConNovedad`.

La clasificación múltiple predetermina la tipología para evitar repetir la misma selección por inscripción. Usa la única opción del catálogo autorizado o una coincidencia única y normalizada de `Constancia de Inscripción`; si el catálogo es ambiguo no adivina y conserva la selección manual. La elección sigue siendo editable y atraviesa el mismo preflight autoritativo antes de crear la intención.

La corrida QA múltiple detectó antes de autenticar una línea base incompatible (`E2E_PLATFORM_GATE_INTEGRITY_FAILED`): el gate estaba versionado activo aunque el runbook exige finalizar apagado. Se corrigió la configuración a `false`, con audiencias vacías. Esto no reinstala restricciones por usuario o grupo: al activarse temporalmente, cualquier sesión Workflow válida conserva acceso.

Un segundo preflight se detuvo con `E2E_PLATFORM_PROVIDER_INTEGRITY_FAILED` porque la plataforma solo admitía proveedor vacío, mientras DOC-79 ya versiona el proveedor canónico `INTEGRACIONSII`. El control ahora admite exclusivamente vacío o ese identificador canónico; continúa rechazando cualquier valor diferente y restaura exactamente la configuración original.

La corrida alcanzó luego la inspección visual y detectó `IMPORT_E2E_PREPARATION_UI_RESPONSIVE_INVALID` a 760 px. La altura móvil de la tabla descontaba solo 16 rem y omitía encabezado, contexto, etapas, estado y acciones. Se corrigió a un espacio acotado por el viewport (`max(12rem, 100dvh - 28rem)`), manteniendo scroll horizontal y vertical dentro de la región accesible.

La siguiente inspección llegó a la preparación masiva y reportó `IMPORT_E2E_PREPARATION_UI_MULTIPLE_INVALID`: el checkbox total seleccionó más de las dos filas mínimas del perfil, pero la aserción heredada exigía exactamente dos selectores. La prueba ahora compara la cantidad preparada con el total real de filas importables, confirmando el alcance completo de seleccionar todos.

El cierre posterior reportó `E2E_PLATFORM_LEGACY_INTEGRITY_FAILED` porque el control usaba `git diff` y confundía cambios legítimos ya presentes en DOC-79 con mutaciones producidas por la E2E. El runner ahora captura ambas páginas Workflow antes de tocar el gate y exige igualdad exacta al finalizar. Se mantiene el fallback por `git diff` para consumidores que no entreguen línea base.

- Ticket: DOC-79
- Cambio OpenSpec: doc-79-pruebas-retiro-gate
- Clasificacion: cross_cutting

## Evidencia local

| Comando | Resultado |
| --- | --- |
| `powershell -File tools/validation/Verify-ImportarServicioWebFrontend.ps1` | 24/24 pruebas; 0 fallos |
| `node --test tools/e2e/tests/importar-servicio-web-modern.spec.cjs` | 22/22 pruebas estructurales; 0 fallos |
| `node --test tests/importar-servicio-web-*.test.cjs` | 452/452 pruebas; 0 fallos |
| `msbuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m` | 0 errores; 310 advertencias preexistentes |

Estas ejecuciones no autenticaron usuarios, no usaron red, no modificaron tareas y no activaron el gate.

## Evidencia E2E real

La corrida de lectura fue autorizada expresamente para el ambiente y la cuenta de prueba, después de leer el runbook obligatorio. Se reutilizó exclusivamente la plataforma E2E existente.

| Campo | Resultado saneado |
| --- | --- |
| Escenario | `import-sii-read` |
| Tarea autorizada | `219877` |
| Recibo | `S002188422` |
| Código de barras | `18221398` |
| Tamaño de muestra efectivo | `1` |
| Controles | `7` |
| Cambios persistentes | `NO` (`sinCambios=SI`) |
| Resultado | Correcto |
| Gate al finalizar | `false`; usuarios y grupos vacíos |

El perfil inicialmente solicitó dos elementos, pero la UI expuso menos de dos elementos seleccionables. La plataforma se detuvo antes de cualquier mutación con `IMPORT_E2E_PREPARATION_UI_MULTIPLE_ITEMS_UNAVAILABLE`. Se ajustó únicamente el perfil runtime a una muestra compatible de un elemento y la repetición autorizada terminó correctamente. La cobertura multidocumento permanece en las suites automatizadas específicas.

### Certificación QA multidocumento final

Después de corregir las incompatibilidades detectadas por las corridas exploratorias, se ejecutó nuevamente la plataforma compartida sobre una tarea con múltiples filas importables. La inspección verificó en navegador real la región responsive, selección parcial e indeterminada, seleccionar/deseleccionar todas las filas habilitadas, preparación del total seleccionado, tipología `Constancia De Inscripción` predeterminada para cada fila y preflight sin solicitudes mutantes.

| Campo | Resultado saneado |
| --- | --- |
| Escenario | `import-sii-read` |
| Tarea autorizada | `220580` |
| Recibo | `S002495168` |
| Código de barras | `18341190` |
| Tamaño mínimo solicitado | `2` |
| Controles | `7` |
| Cambios persistentes | `NO` (`sinCambios=SI`) |
| Resultado | Correcto |
| Gate al finalizar | `false`; usuarios y grupos vacíos |

Mensaje final saneado: `La plataforma E2E terminó correctamente (import-sii-read); controles=7; sinCambios=SI.`

Después de la finalización y restauración comprobada de la corrida, el responsable autorizó explícitamente activar `WorkflowCentroTrabajoModernActive=true` para todos los usuarios Workflow autenticados. `WorkflowCentroTrabajoModernUsers` y `WorkflowCentroTrabajoModernGroups` permanecen vacíos. Esta activación posterior no forma parte de la corrida ni altera su evidencia `sinCambios=SI`.

## Saneamiento

No se registraron credenciales, cookies, tokens, cadenas de conexión ni valores sensibles. Las consultas de control fueron exclusivamente `SELECT`.
