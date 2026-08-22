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

## DOC-29 — Evidencia local de interfaz

Ejecutado el 2026-08-21 desde la raíz del repositorio:

```powershell
node --test tests\workflow-user-send.test.cjs tests\workflow-user-send-ui.test.cjs tests\workflow-user-send-confirmation.test.cjs tests\workflow-group-send.test.cjs tests\workflow-transition-ui.test.cjs tests\workflow-transition-confirmation-integration.test.cjs tests\workflow-transition-page-presentation.test.cjs tests\workflow-modern-feature-gate.test.cjs
msbuild .\GestionDocumental-Docuarchi.net.sln /t:Build /p:Configuration=Debug /m /verbosity:minimal /clp:ErrorsOnly
```

Resultados: CJS 61/61 correctas y MSBuild con código 0. Las pruebas verifican el disparador sin gate, ausencia de fallback Web Forms de usuario, contrato paginado por cursor, búsqueda/debounce, descarte de respuesta obsoleta, selección, confirmación, bloqueo, cancelación, doble clic, teclado, foco, aislamiento respecto de Continuar flujo y actualización parcial con mensaje propio.

## QA/E2E Web Forms

No se ejecutaron QA manual autenticada, E2E autenticado, carga, activación de gate ni transición real: no hubo autorización explícita de ambiente y cuentas para DOC-29. Las pruebas locales no conceden esa autorización.

Cuando exista autorización, el recorrido manual debe: seleccionar una tarea descartable, abrir **Enviar a usuario** con el gate apagado, verificar foco/Tab/Escape, buscar y navegar páginas, cancelar una selección, confirmar un destino válido, comprobar que solo cambia la fila/visor/contador y registrar evidencia sanitizada. Antes de cualquier E2E autenticado se debe leer `tools/e2e/AGENT-RUNBOOK.md`; no se guardan credenciales, cookies ni cadenas de conexión y el gate debe finalizar en `false` con usuarios/grupos vacíos.

## DOC-30 — Verificación transversal

DOC-30 ejecutó la compuerta no mutante sobre el snapshot integrado: inspección estática de contratos, revalidación con lock, respuesta y auditoría; la batería CJS de nueve suites focales terminó con 66/66 correctas y MSBuild terminó sin errores. La QA visual no autenticada verificó apertura, búsqueda, transición visual, selección, recarga, cierre y geometría estable del modal. No se ejecutaron E2E autenticados, carga, activación de gate, cambios de ambiente ni despliegue en esta etapa.

El dictamen técnico es **apto para solicitar aprobación operativa**. El detalle de matriz, riesgos, diagramas y límites se conserva en `../03-verificacion-transversal-doc-30/`; esa conclusión no autoriza un ambiente ni revierte una transición confirmada.
