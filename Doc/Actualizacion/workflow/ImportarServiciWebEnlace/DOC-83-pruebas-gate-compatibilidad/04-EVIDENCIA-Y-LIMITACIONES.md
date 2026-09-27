# Evidencia y limitaciones

## Ejecución determinística DOC-83

Fecha: 2026-09-27.

```powershell
npm.cmd --prefix tools/e2e run test:doc83:closure
npm.cmd --prefix tools/e2e run test:doc83:regression
```

Resultados:

- Matriz ejecutable: `4/4` PASS, incluidos tres casos negativos controlados.
- Regresión consolidada: `149/149` PASS.
- MSBuild de GestionDocumental-Docuarchi.net.sln: PASS, 0 errores y 1 advertencia preexistente de redirecciones de ensamblado.
- E2E real `import-sii-enlase-anonymous`: PASS; bloqueo opaco `FEATURE_DISABLED`, cero elementos y sin cambios.
- E2E real `import-sii-enlase-assignment`, camino negativo: PASS; `assignmentAction=EXPLICIT`, `assignmentResult=BLOCKED`, `authoritativeValidation=CONFIRMED`, un control intacto y `sinCambios=SI`.
- Diagnóstico de solo lectura previo a la importación: trámite 290 (`mutacionregmer`), checklist obligatorio 265, `Recibo De Caja` tipo 186, serie 15, subserie 20 y cero coincidencias físicas.
- E2E real `import-sii-enlase-execution` sobre la tarea 220589: PASS; un elemento persistido, siete controles, evidencia física confirmada, sin efectos de expediente ni transición de tarea.
- E2E real `import-sii-enlase-assignment`, camino positivo: PASS; `assignmentAction=EXPLICIT`, `assignmentResult=ASSIGNED`, `authoritativeValidation=CONFIRMED`, cambio en `workflow-assignment-state` y `sinCambios=NO`.
- Importación y asignación usaron autorizaciones y reservas de ciclo de vida independientes. Al finalizar, gate `false`, usuarios/grupos vacíos e integridad legacy confirmada.
- La primera versión del comando consolidado ejecutó desde `tools/e2e` y produjo `26` fallos `ENOENT`; se corrigió el lanzador para fijar la raíz del repositorio. No fue un defecto productivo.

## Evidencia real reutilizada

| Fuente | Resultado registrado |
|---|---|
| DOC-80 `README.md` | Lectura real PASS, siete controles sin cambios, gate restaurado. |
| DOC-81 `04-PRUEBAS-SEGURIDAD-ROLLBACK.md` | Ejecución múltiple real PASS, una intención/ejecución y persistencia física 2/2, sin transición de tarea, gate restaurado. |
| DOC-82 `04-PRUEBAS-SEGURIDAD-Y-OPERACION.md` | UI real PASS, nueve anexos, flujo múltiple/preview/foco/responsive y siete controles sin cambios. |

La evidencia histórica demuestra esos recorridos en las fechas y recursos originales. No concede autorización para una nueva corrida ni demuestra disponibilidad actual del proveedor.

## Limitaciones

- Las pruebas estructurales no prueban por sí solas comportamiento completo, calidad visual ni disponibilidad SII.
- La E2E confirmó tanto el rechazo por requisito faltante como la asignación posterior al satisfacerlo mediante una importación real.
- El artefacto canónico del escenario conserva el resultado más reciente (`ASSIGNED`); el resultado anterior `BLOCKED` permanece en la salida saneada de consola porque el artefacto por escenario se sobrescribe.
- La tarea 220589 se reutilizó por autorización expresa del usuario. Las reservas locales consumidas se respaldaron antes de rearmarlas; no se alteraron datos de negocio fuera de la importación y asignación autorizadas.
- La evidencia corresponde al ambiente y recurso ejecutados; no demuestra disponibilidad futura del proveedor ni generaliza la configuración documental a otros trámites.
- No se almacenan credenciales, cookies, tokens, conexiones ni contenido documental.
