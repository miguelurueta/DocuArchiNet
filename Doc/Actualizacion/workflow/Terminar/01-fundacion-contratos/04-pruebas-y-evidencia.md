# Pruebas y evidencia

- Ticket: DOC-9
- Cambio OpenSpec: doc-9-contrato-terminar-tarea-workflow
- Clasificacion: cross_cutting

## Evidencia requerida

| Validación | Comando o procedimiento | Resultado |
|---|---|---|
| Compilación .NET Framework 4.6.1 | `& 'C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe' 'GestionDocumental-Docuarchi.net.vbproj' /t:Build /p:Configuration=Debug /m /verbosity:minimal` | Correcto el 2026-08-13. Se generó `bin\GestionDocumental-Docuarchi.net.dll`. |
| Validación focal de compilación inicial | Mismo comando antes y después de corregir el identificador reservado VB `Error`. | El error `BC30183` quedó corregido usando `[Error]` sin alterar el nombre serializado del contrato. |
| Verificación focal automatizada | `powershell -ExecutionPolicy Bypass -File tools\validation\Verify-Doc9Foundation.ps1` | Correcto el 2026-08-13: gate fail-closed, adaptador inerte, sin dependencias Web Forms, sin llamadas nuevas al motor legacy, estructura `DTOs/Workflow/Terminar` / `Services/Workflow/Terminar`, `Infrastructure/Workflow/Terminar` e `Infrastructure/Repositories/Workflow`, e infraestructura compartida sin acoplamiento Workflow. |
| Integridad de UI legacy | Comparar el diff de DOC-9 para `workflow/Webworkflow.aspx` y `workflow/Webworkflow.aspx.vb`. | No se modificaron ambos archivos. |

La compilación produce advertencias existentes de referencias .NET y de variables legacy potencialmente no inicializadas. No son errores de DOC-9 y no se modificaron porque están fuera de la fundación.

## QA/E2E WebForms

No se agregó UI, endpoint ni navegación en esta fase, por lo que no aplica una E2E automatizada nueva. La regresión funcional debe verificarse manualmente antes de habilitar cualquier fase posterior:

1. Iniciar sesión con un usuario Workflow fuera de cualquier piloto moderno futuro.
2. Abrir `workflow/Webworkflow.aspx` y seleccionar una tarea por ruta; confirmar que el envío sigue mostrando y ejecutando el flujo legacy.
3. Repetir con una tarea que tenga flujo documental y conectores disponibles.
4. Verificar un caso bloqueado por autorización, firma o expediente; debe conservar su mensaje y no cambiar estado.
5. Confirmar que los eventos `PRETERMINARACTIVIAD` / `TERMINARACTIVIDAD`, correo y trazabilidad siguen funcionando según la configuración del ambiente.

### Ejecución registrada

- Fecha: 2026-08-13.
- Ambiente: aplicación Web Forms autenticada.
- Usuario ejecutor: `MELBAA`.
- Procedimiento observado: envío de una tarea mediante el flujo legacy vigente.
- Resultado: el usuario responsable confirmó que el envío se completó correctamente y autorizó el cumplimiento de la tarea 3.3.
- Evidencia disponible: confirmación explícita del usuario responsable; no se adjuntó identificador de tarea, captura, URL, detalle de ruta/flujo ni caso bloqueado.

Esta QA verifica el recorrido principal legado observado. No afirma como ejecutados los escenarios adicionales enumerados arriba que no fueron reportados; quedan como cobertura recomendable para las fases que conecten endpoint, interfaz o piloto moderno.

## Limitaciones reales

El repositorio no contiene un proyecto de pruebas unitarias VB.NET ni una infraestructura de E2E Web Forms reutilizable. Por ello no se incorporó un marco de pruebas nuevo solo para contratos de una fundación; la compilación completa y las verificaciones estáticas cubren la integración. Las siguientes fases deberán agregar pruebas focales para validadores, gate, proveedores y adaptador cuando se conecte un endpoint o una composición ejecutable.

La verificación focal reproducible se agregó en `tools/validation/Verify-Doc9Foundation.ps1` y se registró como evidencia `unit` de OPSXJ. Verifica por reflexión el comportamiento fail-closed del gate y la respuesta inerte del adaptador; además, exige los archivos de la estructura física oficial, revisa los límites de código fuente y rechaza llamadas nuevas al motor legacy.
