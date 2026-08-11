# DOC-2 — Pruebas y evidencia

## Evidencia local — 2026-08-10

- `npm.cmd --prefix tools/opsxj test`: **PASS**, 12 archivos y 71 pruebas. Incluye `doc2WorkflowActivation.test.js`, que verifica flag apagado, piloto de servidor, cutover, orden de recursos, scope CSS y adaptador sin mutaciones prohibidas.
- `MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m`: **PASS**. Persisten advertencias de dependencias y variables sin inicializar preexistentes; no hay errores DOC-2.
- `openspec.cmd validate doc-2-infraestructura-visual-aislada-reversible --strict`: **PASS**.
- `manual_qa`: no disponible localmente; requiere URL de ambiente, cuenta dentro y fuera del piloto y datos Workflow controlados.

## Recorrido QA obligatorio

1. Con `WorkflowCentroTrabajoModernEnabled=false`, confirmar ausencia de clase y recursos DOC-2, y ausencia de errores durante carga y postback.
2. Con flag `true` y perfil fuera de lista, comprobar el mismo resultado apagado.
3. Con flag `true` y perfil piloto, comprobar clase raíz, cuatro subcapas y recursos después de `Webworkflow.js`.
4. Retirar individualmente `actions`, `documents` y `a11y`; comprobar que retrocede solo esa presentación.
5. Restaurar `false`, recargar forzado y navegar internamente; comprobar que no quedan mutaciones visuales ni se habilitan acciones ocultas por servidor.

Las capturas de 1366, 1024, 768 y 375 px y la ejecución manual se asociarán al SHA desplegado.
