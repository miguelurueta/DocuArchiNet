# Pruebas, evidencia y riesgos

- Ticket: DOC-15
- Cambio OpenSpec: doc-15-base-enviar-grupo
- Clasificacion: cross_cutting

Ejecutado el 2026-08-19 desde la raíz del repositorio:

```powershell
msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /verbosity:minimal
node --test tests\*.test.cjs
npm.cmd --prefix tools\opsxj run opsxj:refine -- DOC-15 --sync
openspec.cmd validate doc-15-base-enviar-grupo --strict
```

Resultados: MSBuild código 0; suite Node 41/41; refinement y OpenSpec PASS. MSBuild conserva advertencias heredadas de conflictos de ensamblados.

La prueba focal cubre contrato sin conector, lectura exclusiva, ruta/flujo, destino fuera de ruta, aprobaciones, token, guard, auditoría, UI accesible, fallback y no regresión de Continuar flujo.

No se ejecutaron E2E autenticada, carga, QA manual autenticada ni activación del gate porque no hay autorización explícita de ambiente y cuentas. Esta evidencia permite solicitar autorización de QA o activación, pero no la concede.

## Decisión de liberación

La decisión técnica es **solicitar aprobación de activación**. La compilación y las pruebas focales están aprobadas, pero no hay autorización operativa por ambiente. Por ello no se modificó `WorkflowCentroTrabajoModernActive`, ni sus listas de usuarios o grupos.

## Matriz de ambientes

| Ambiente | Autorización | Versión | Alcance | Ventana | Responsable | Estado | Rollback |
| --- | --- | --- | --- | --- | --- | --- | --- |
| No definido | Pendiente | DOC-15 | No definido | No definida | No definido | No activar | Gate inactivo y listas vacías |

La matriz debe sustituirse con los ambientes, versión, responsables, ventana, alcance y aprobación explícitamente autorizados. Las pruebas satisfactorias no autorizan ningún ambiente ni implican activación global.

## Runbook para una operación autorizada

1. Confirmar versión, aprobación vigente, ambiente y salud previa mediante verificaciones de solo lectura.
2. Conservar una referencia sanitizada de la configuración y del fallback legado, sin secretos.
3. Aplicar solamente el gate existente y el alcance autorizados.
4. Ejecutar las verificaciones autorizadas y no mutantes; comprobar gate, fallback y la no regresión de `Continuar flujo`.
5. Ante discrepancia, detener la operación y aplicar el rollback aprobado: gate inactivo, usuarios/grupos vacíos y retorno al postback legado para nuevos intentos. No revertir transiciones ya confirmadas.
6. Registrar hora, versión, ambiente, responsables, evidencia sanitizada, decisión y riesgos residuales.

## Riesgos residuales

- La activación queda bloqueada hasta contar con autorización explícita por ambiente, ventana, responsables, versión, alcance y plan de rollback.
- No se efectuaron E2E autenticadas ni carga; requerirán la autorización, cuentas y tareas descartables previstas por el runbook del repositorio.
- El alcance global no se debe inferir de listas vacías: debe verificarse contra la semántica aprobada de la configuración existente.
