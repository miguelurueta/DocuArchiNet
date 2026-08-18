# Flujo, seguridad y rollback

## Secuencia funcional

1. La lista moderna solicita `PreviewEnviarTarea` y presenta destinos de solo lectura.
2. La persona elige un destino; la lista publica el detalle normalizado.
3. El adaptador abre la confirmación con campos disponibles, aviso y acción contextual.
4. El diálogo evita doble envío, bloquea cualquier cierre durante `enviando` y el adaptador invoca el ASMX con la terna seleccionada.
5. El servidor revalida feature gate, sesión, tarea, token, destino, concurrencia, requisitos y autorización vigente.
6. Tras éxito correlacionado, el callback de página retira la fila si sigue representada, restablece la lista y oculta contexto, visor y acciones de la selección. El aviso de éxito se oculta a los seis segundos. Bloqueo o error conservan el contexto.

## Límites de seguridad

- No se ejecutan autorización, permisos, cambios de estado, firma, expediente, copia, balanceo ni eventos dinámicos en cliente.
- El componente y el adaptador no llaman controles ocultos, `Terminar_Tarea_Workflow`, `Cambia_Estado`, SQL, repositorios o Session.
- Los mensajes visibles proceden de contratos funcionales o de mensajes técnicos genéricos; no exponen excepciones, trazas, HTML, credenciales ni SQL.
- Antes del envío, cierre o cancelación descartan la apertura. Durante `enviando`, `X`, Cancelar, fondo, Escape, reemplazo y cierre programático se bloquean; la navegación solicita confirmación nativa del navegador.
- Cerrar la pestaña pese al aviso no es una cancelación garantizada: el servidor determina el resultado y, al regresar, la lista se consulta de nuevo.
- La actualización de fila, visor y contador se ejecuta solamente desde `applySuccess` después de éxito y token correlacionados.

## Autorización vigente

El adaptador no implementa una autorización alternativa. Si `ServicioTransicionTarea` o sus adaptadores detectan un bloqueo funcional, el diálogo restaura acciones conforme a `EsReintentable` y presenta la causa segura entregada por servidor.

## Riesgos y recuperación

| Riesgo | Mitigación |
| --- | --- |
| Doble clic o solicitudes paralelas | Bloqueo visual y guard de concurrencia del servidor |
| Cierre o recarga durante envío | Acciones de cierre bloqueadas, aviso accesible y `beforeunload`; el servidor conserva la autoridad si el navegador finaliza la sesión |
| Token o respuesta obsoleta | Secuencia local y comparación de `TokenVersion` |
| Feature gate desactivado entre preview y envío | Revalidación ASMX y bloqueo funcional sin fallback automático |
| Regresión legacy | Assets condicionados por gate; no se alteran modales, postbacks ni motor legacy |
| Rollback | Desactivar `WorkflowCentroTrabajoModernActive` y recargar la página |
