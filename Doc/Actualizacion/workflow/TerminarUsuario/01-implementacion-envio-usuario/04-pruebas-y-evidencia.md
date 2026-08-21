# Pruebas, evidencia y riesgos

- Ticket: DOC-28
- Cambio OpenSpec: doc-28-backend-enviar-usuario-workflow
- Clasificación: cross_cutting

Ejecutado el 2026-08-21 desde la raíz del repositorio:

```powershell
node --test tests\workflow-user-send.test.cjs tests\workflow-transition-ui.test.cjs tests\workflow-transition-confirmation-integration.test.cjs tests\workflow-transition-page-presentation.test.cjs tests\workflow-modern-feature-gate.test.cjs
msbuild .\GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal /clp:ErrorsOnly
npm.cmd --prefix tools\opsxj run opsxj:refine -- DOC-28 --sync
openspec.cmd validate doc-28-backend-enviar-usuario-workflow --strict
```

Resultados: suite Node 38/38; MSBuild código 0; refinement y OpenSpec PASS. MSBuild conserva advertencias heredadas de resolución de ensamblados .NET Framework, sin errores de DOC-28.

La prueba focal cubre contratos sin conector, permiso fail-closed, consultas sin escrituras, filtro, cursor, orden, tamaño de página, revalidación dentro del lease, política de respuesta, adaptador directo, auditoría y composición ASMX. Las pruebas existentes cubren que Continuar flujo conserva el contrato por conector y la presentación anterior.

## Límite de evidencia

No se ejecutaron E2E autenticado, carga, QA manual autenticada, transición real, consultas de ambiente ni activación de gate porque no existe autorización explícita de ambiente y cuentas. DOC-28 no incluye UI; por ello no hay validación visual de esta entrega. Esta evidencia no concede autorización operativa ni de despliegue.

## Decisión de siguiente etapa

La decisión técnica es **habilitar el trabajo de la etapa 02 de interfaz**, no activar ambientes. La implementación backend entrega endpoints, datos mínimos y errores seguros; la etapa 02 debe añadir selección, confirmación, accesibilidad y evidencia visual sin alterar el contrato ni introducir `IdConector`.

## Riesgos residuales

- La transición real y la concurrencia contra MySQL no se probaron en un ambiente autenticado; requieren autorización, tareas descartables y el runbook del repositorio.
- La consulta está protegida por validaciones y pruebas estáticas/locales; cambios futuros en el esquema de Workflow deben volver a revisar las columnas de usuario, grupo y actividad.
- La UI de la etapa 02 no debe tratar preview como autorización: debe enviar siempre usuario, actividad y token, y aceptar bloqueos de la revalidación del servidor.
