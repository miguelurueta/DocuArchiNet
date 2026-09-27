# Matriz de pruebas y riesgos

La fuente ejecutable es `tools/e2e/validation/doc83-sii-enlase-closure-matrix.json`. El validador comprueba que todos los archivos existan, los escenarios estén registrados, etapa/mutabilidad/autorizaciones coincidan, los controles sean `SELECT` registrados, la evidencia exista y cada brecha real declare autorización y política de recurso.

| Riesgo | Severidad | Cierre determinístico | Evidencia real reutilizada | Estado |
|---|---|---|---|---|
| Contexto o preview ENLASE no autorizado | Alta | contexto, listado y autorización de preview | DOC-80 lectura | Cubierto |
| Lectura con efectos | Alta | mediación y ausencia de llamadas extra | DOC-80/DOC-82, 7 controles invariantes | Cubierto |
| Duplicidad o persistencia física incompleta para N anexos | Muy alta | intención, concurrencia, reconciliación y contrato ENLASE | DOC-81, intención única y evidencia física 2/2 | Cubierto |
| Regresión de UI múltiple, preview, foco o responsive | Alta | UI y accesibilidad | DOC-82, 9 anexos observados | Cubierto |
| Importación asigna o evita revalidación | Muy alta | ausencia de autoasignación y autoridad de `Buttonaceptar_Click` | DOC-83: `BLOCKED` sin cambios y `ASSIGNED` con transición, ambos con validación autoritativa confirmada | Cubierto |
| Gate o legacy quedan alterados | Alta | gate, regresión y huellas legacy | Restauración registrada en DOC-80/81/82 | Cubierto |
| Acceso, perfil o evidencia inseguros | Alta | autorización, perfiles y saneamiento | DOC-83 anónima: bloqueo opaco, cero elementos y sin cambios | Cubierto |
| Resultado tardío contamina otra tarea | Alta | guard, aislamiento y multi-pestaña | DOC-82 | Cubierto |
| Regresión de constancias legacy | Alta | compatibilidad SII, UI legacy e invariancia | No requiere mutación adicional | Cubierto |

## Brecha de asignación cerrada

La tarea descartable 220589 confirmó primero `BLOCKED`: el trámite `mutacionregmer` exigía `Recibo De Caja` (tipo 186, serie 15, subserie 20) y la consulta de control encontró cero coincidencias físicas. Una corrida real y autorizada de `import-sii-enlase-execution` incorporó el documento; una autorización y reserva E2E independientes permitieron accionar nuevamente la asignación, que devolvió `ASSIGNED` y cambió `workflow-assignment-state`.

La misma tarea se reutilizó por autorización expresa del usuario debido a la ausencia de otro recurso. Antes de rearmar la reserva local de asignación consumida se conservaron respaldos `.bak`. No se alteraron datos de negocio para evadir la validación: la importación real cambió la precondición documental y `Buttonaceptar_Click` volvió a decidir de forma autoritativa.
