# QA manual mínima — DOC-10

Estado: **ejecutada el 2026-08-14** para la ruta 922. Esta validación humana confirma que un navegador autenticado recibe el contrato real. La E2E automatizada cubre sesión anónima, no piloto, flujo 879, ruta 922 y comparación de estado/auditoría antes/después.

## Resultado registrado

| Dato | Resultado |
| --- | --- |
| Ambiente | `http://localhost/GestionDocumental-Docuarchi.net/` |
| Módulo y actor | GESTOR, piloto autorizado |
| Tarea | 922 (`RUTA`) |
| Respuesta | HTTP 200; dos destinos: `CONTADOR` y `SUPERVISOR` |
| Error | `null` |
| Evidencia | [qa-manual-922.json](evidencias/qa-manual-922.json) y capturas de QA |

## Repetir la QA (cuatro pasos)

1. Con autorización, activar temporalmente el gate y limitarlo al piloto.
2. Iniciar sesión en **GESTOR** y abrir la tarea 922.
3. En F12 → Consola, ejecutar:

```javascript
fetch('/GestionDocumental-Docuarchi.net/webservice/WebServiceWorkflowModern.asmx/PreviewEnviarTarea', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json; charset=utf-8', 'X-Requested-With': 'XMLHttpRequest' },
  body: JSON.stringify({ idTarea: 922 })
}).then(async (r) => alert(`HTTP ${r.status}\n${await r.text()}`))
```

4. Confirmar `HTTP 200`, `TipoDecision: "RUTA"`, dos destinos y `Error: null`; guardar una captura.

## Cierre obligatorio

Restaurar `WorkflowCentroTrabajoModernActive=false` y dejar vacíos `WorkflowCentroTrabajoModernUsers` y `WorkflowCentroTrabajoModernGroups`. La comparación de no mutación se reutiliza desde [qa-preview-922.json](evidencias/qa-preview-922.json), que solo usa `SELECT` antes/después.
