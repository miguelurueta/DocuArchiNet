# Validación local — 2026-08-10

## Contexto

- Cambio: `doc-1-contrato-regresion-workflow` / DOC-1.
- Rama: `feature/DOC-1`.
- Alcance validado: artefactos documentales y compilación del baseline WebForms; no se modificó código de aplicación.

## Ejecuciones

| Verificación | Resultado | Evidencia |
| --- | --- | --- |
| `npm.cmd --prefix tools\\opsxj test` | Aprobada | Vitest: 10 archivos y 63 pruebas aprobadas. |
| `MSBuild GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false` | Aprobada | Exit code 0; se generó `bin\\GestionDocumental-Docuarchi.net.dll`. |
| `openspec validate doc-1-contrato-regresion-workflow --strict` | Aprobada | Cambio válido. |
| `git diff --check` | Aprobada | Sin errores de whitespace. |

## Advertencias de compilación

MSBuild emitió advertencias heredadas de resolución de ensamblados .NET 4.6.1 y advertencias VB de variables posiblemente no inicializadas. La compilación no tuvo errores y DOC-1 no modifica los archivos referidos por esas advertencias.

## Límite de esta evidencia

No existe en el workspace un ambiente IIS/QA, corte aprobado de JIRA-00, cuentas con/sin permiso ni datos de prueba controlados. Por ello no se ejecutaron ni se declaran aprobados los casos de navegador R-01 a R-10; permanecen pendientes según `05-MatrizRegresionBase.md`.

