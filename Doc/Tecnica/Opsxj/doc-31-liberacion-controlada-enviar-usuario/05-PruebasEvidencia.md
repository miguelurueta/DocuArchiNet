# Pruebas y evidencia — Liberación controlada de Enviar a usuario

- Ticket: DOC-31
- Cambio OpenSpec: doc-31-liberacion-controlada-enviar-usuario
- Clasificacion: cross_cutting

## Evidencia requerida

DOC-31 reutiliza la evidencia aprobada de DOC-30: compilación MSBuild sin errores, 66 pruebas CJS correctas, inspección estática y QA visual no autenticada. La matriz de ambientes conserva la decisión de solicitar aprobación operativa y no incluye secretos ni credenciales.

## QA/E2E WebForms

No se ejecutaron E2E, carga, cambios de ambiente, activación de gate ni despliegue. El runbook solo permite comprobaciones documentales o `SELECT` sanitizados una vez que un ambiente concreto reciba autorización explícita y nombre responsables, versión y ventana.
