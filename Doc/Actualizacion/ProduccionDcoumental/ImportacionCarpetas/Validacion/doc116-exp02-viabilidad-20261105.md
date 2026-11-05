# DOC-116 — viabilidad de la ventana de fallo interna

Resultado: COMPOSICION_REAL_NO_DISPONIBLE en la copia local. Inspeccionados AdaptadorExpedienteImportacion.vb, .Legacy.vb y ComposicionCreacionImportacion.vb. El constructor Friend permite delegados del arnés; la composición real fija InvocarConservacion y RegistrarResultadoCreacion, exige HttpContext.Current/sesión/autoridad y repositorio vigentes. No expone un punto de fallo HTTP autorizado.

Las pruebas locales sí ejercitan ThrowWriter, FailCheckpoint y LoseCommitResponse. Crear un HttpContext ficticio, reutilizar un endpoint de prueba o borrar filas no acreditaría el recorrido autenticado real y puede fabricar autoridad o incertidumbre. No se implementa ninguna de esas vías.

El soporte nuevo permite descartar solo el resultado del cliente de una creación confirmada y distingue esa observación. No se identifica como fallo interno ni acredita pérdida de respuesta de red. Una consulta posterior a resultado desconocido no desbloquea otra creación.

Responsable de habilitar la aceptación interna: responsable del ambiente con un mecanismo interno autorizado que conserve la autoridad real y el punto exacto de fallo. Antes de ejecutarlo debe revisarse su contrato, presupuesto y recuperación. Hasta entonces esa variante queda NO_EJECUTADO y bloquea su cierre; no se modifica producto para obtener una prueba verde.

## Ampliación de viabilidad — 2026-11-05

Se compiló una candidata de laboratorio desactivada mediante overlay externo, con autoridad HTTP real conservada y dos ventanas del delegado de checkpoint. No se instaló en IIS ni se ejecutó ningún fallo. Reutilizaría lote 18/raíces 29/30 para evitar nuevo setup. Se requiere aprobación explícita de la vía, seguida de preparación y revisión del driver/activación/restauración antes de autorizar efectos. Detalle: `doc116-exp02-interno-propuesta-20261105.md`; evidencia compilación: `doc116-exp02-interno-candidata-20261105.json`. El bloqueo de aceptación continúa.

Ejecutor y restauración preparados: 108/108 pruebas locales PASS. Candidata acotada al lote 18/raíces 29/30 compilada; DLL real intacto. Falta aprobación específica de activación temporal y efectos, seguida de ejecución/revisión real. Detalle vigente: `doc116-exp02-interno-ejecucion-20261105.md`. Los cortes anteriores de indisponibilidad describen el runtime normal sin instrumentación.

## Resultado vigente

La vía de laboratorio fue preparada, autorizada y ejecutada con autenticación real: ambas ventanas PASS, DLL original restaurada y dos copias comprobadas byte por byte. Informe: `doc116-exp02-interno-resultados.md`. La indisponibilidad del runtime normal sin instrumentación y los cortes anteriores permanecen como historia; no se requiere repetir la prueba ni cambiar producto. Intento 41 permanece EN_CURSO sin recreación ni reconciliación.
