# Pruebas, evidencia y dictamen

- Ticket: DOC-34
- Cambio OpenSpec: `doc-34-verificacion-transversal-devolver-tarea`
- Fecha de verificación local: 2026-08-25

## Matriz de verificación

| Área | Evidencia | Resultado |
| --- | --- | --- |
| Compilación Web Forms | `MSBuild GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /nologo` | Correcta: 0 errores; 1 advertencia heredada `MSB3247` de resolución de ensamblados. |
| Preview y contrato DOC-32 | `workflow-return-activity.test.cjs` y política DOC-32 | Aprobado: permiso fail-closed, lecturas Ruta/Flujo entrantes, cursor, límite, token y ausencia de dependencias mutantes. |
| Ejecución, lock y auditoría | pruebas DOC-32 y revisión de servicio/adaptador | Aprobado: revalidación dentro del lock, una transición efectiva en la evidencia previa de concurrencia y sin métodos de respuesta nuevos. |
| UI DOC-33 | `workflow-return-activity-ui.test.cjs`, confirmación y política DOC-33 | Aprobado: preview, selección, payload mínimo, bloqueo, cancelación, cierre, accesibilidad y retiro del postback de actividad anterior. |
| Compatibilidad | suites de usuario, grupo, transición, confirmación y gate | Aprobado: los contratos vecinos permanecen aislados. |
| Batería local integrada | 10 suites CJS focales | Aprobado: 83 pruebas, 0 fallos. |
| Reconfirmación de políticas | 5 suites CJS de devolución | Aprobado: 35 pruebas, 0 fallos. |

## Inspección estática

`MySqlDevolverActividadRepository` contiene consultas `SELECT` parametrizadas para Ruta y Flujo, con `ORDER BY`, `LIMIT` y conectores de entrada diferenciados. La revisión de los scripts de devolución no encontró referencia a `WorkflowCentroTrabajoModernActive`. La revisión específica no encontró invocaciones a `After_envio_usuario_workflow`, `Reasigna_respuesta_envia_tarea_usuario` ni `Cambia_Estado` en los componentes de devolución.

El uso de `Terminar_Tarea_Workflow` está contenido en el adaptador de ejecución aprobado de DOC-32 y no equivale a un recorrido de respuestas ni a un fallback Web Forms.

## QA manual no autenticada

Se abrió `workflow/Webworkflow.aspx` sin sesión en la instancia local, en anchos de 1366 px y 375 px. Ambas vistas respondieron HTTP 200 y conservaron modal oculto con `aria-hidden`, diálogo `role="dialog"` y `aria-modal="true"`, título asociado, región de estado `aria-live="polite"`, búsqueda de tipo `search`, paginación, cierre de tipo `button`, viewport responsive y los dos scripts de devolución cargados.

Sin una tarea seleccionada el disparador no se publica, por diseño; por ello la QA no invocó preview ni ejecución. Búsqueda, paginación, cancelación, éxito/error simulado, bloqueo, conector manipulado, Ruta/Flujo, concurrencia contractual y timeout están cubiertos por las CJS focales. La evidencia E2E autorizada de DOC-32 y DOC-33 respalda preview no mutante, ejecución única, concurrencia y bloqueo UI; no se repitió en DOC-34.

## Riesgos y dictamen

- La advertencia `MSB3247` es heredada de referencias .NET Framework y no impidió la compilación; debe seguirse observando por mantenimiento general.
- No se detectó hallazgo de contrato, seguridad, UI o compatibilidad que requiera corrección de código.
- La QA anónima solo revisa la carcasa visual porque el disparador requiere una tarea seleccionada; los comportamientos dinámicos se sustentan en CJS y evidencia E2E previa, ambos saneados.

**Dictamen técnico: apto para solicitar la fase 04.** Este dictamen no despliega, no activa el gate y no autoriza E2E, carga ni cambios de ambiente.
