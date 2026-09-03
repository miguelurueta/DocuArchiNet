# Inventario y trazabilidad

## Comando reproducible

```powershell
rg -n "Eliminar_nota_tarea_workflow|Eliminar_nota_service_workflow|Service_.*nota_tarea_workflow|WebFormAnotacion|Panel_Buttonanotacion|ImageButtonanotacion|GridView_lista_notas|WorkflowNotesModern|PuedeGestionar|NotOwner|workflow-notes-modern-access|endRequest" workflow webservice js DTOs Modelo Infrastructure Styles tools/e2e GestionDocumental-Docuarchi.net.vbproj
```

## Matriz ruta–consumidor–decisión

| Ruta o símbolo | Consumidor comprobado | Decisión |
| --- | --- | --- |
| `Eliminar_nota_tarea_workflow` | Sin consumidores productivos | Retirar; D-01/RQ-01 |
| `Eliminar_nota_service_workflow` | `Service_delete_nota_tarea_workflow` | Conservar; D-02/RQ-02 |
| `Service_*_nota_tarea_workflow` | Radicación y Correspondencia | Conservar contrato compartido; D-02/RQ-02 |
| `WebFormAnotacion.aspx(.vb)` | Incluida en proyecto y con uso no descartado | Conservar; D-02/RQ-02 |
| Controles, handlers y llamadas legacy de Notas en `Webworkflow` | Consumidor sustituido por DOC-43/44 | Retirar; D-05/RQ-05 |
| `WebServiceWorkflowNotesModern.asmx` | Cliente moderno y suites de Notas | Conservar y fortalecer; D-05/D-07 |
| DTO/modelo/repositorio modernos | Contrato de listado y mutaciones | Agregar `PuedeGestionar` y `NotOwner`; D-07/RQ-07 |
| `workflow-notes-modern-access` y diálogos | Centro de Trabajo | Única presentación moderna; D-05/D-08 |
| `workflow-notes-modern.css` | Diálogos, scroll, estados y foco | Conservar presentación estable y accesible; D-05/D-07 |
| `workflow-tareas-modernas.css` y `workflow-centro-trabajo-moderno.css` | Acciones `primary`, `info`, `warning`, `success` e índice | Restaurar contraste local sin cambiar eventos; D-09/RQ-09 |
| Listener delegado y `PageRequestManager.endRequest` | Barra reemplazada por `UpdatePanel` | Mantener acceso operativo; D-10/RQ-10 |
| Runner y suite `doc44-workflow-notes` | Regresión oficial reutilizada | Extender sin arnés paralelo; D-06/RQ-06 |
| Modo `test:doc45:empty-notes` | Estado inicial sin notas | Reutilizar el mismo runner y suite; D-08/RQ-08 |

Después del retiro no existen referencias productivas a `Eliminar_nota_tarea_workflow` ni al consumidor visual legacy dentro de `Webworkflow`. Los contratos compartidos permanecen únicamente para consumidores externos comprobados.
