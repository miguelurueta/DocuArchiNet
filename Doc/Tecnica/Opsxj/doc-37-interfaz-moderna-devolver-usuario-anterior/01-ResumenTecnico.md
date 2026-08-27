# Interfaz moderna — Devolver a usuario anterior

- Ticket: DOC-37
- Cambio OpenSpec: doc-37-interfaz-moderna-devolver-usuario-anterior
- Clasificacion: cross_cutting

## Objetivo

Sustituir el postback heredado de **Devolver → Usuario anterior** por una única experiencia moderna que consume los endpoints seguros implementados en DOC-36. La interfaz no recibe ni selecciona destino: el servidor determina el único usuario histórico y actividad válidos, y entrega un token opaco para la ejecución.

## Alcance y compatibilidad

El cambio afecta `workflow/Webworkflow.aspx`, sus archivos code-behind y diseñador, `Styles/workflow-transition-modern.css`, los adaptadores `workflow-return-user-previous-ui.js` y `workflow-return-user-previous-confirmation.js`, y el runner de `tools/e2e/`. Se elimina solo la ruta `D-TWU-ANT`, el botón oculto y el handler de Usuario anterior. Devolver a actividad anterior, Enviar a usuario, Enviar a grupo y Continuar flujo conservan sus rutas, eventos y contratos. El rollback consiste en revertir este conjunto de archivos; no existen cambios de base de datos, endpoint ni configuración de ambiente.

## Arquitectura de pruebas reutilizada

DOC-37 registra su propio perfil y contrato de recursos en `tools/e2e/`, reutilizando de DOC-36 la sesión efímera, las huellas ODBC de solo lectura y la reserva local de tareas. El perfil DOC-37 se deriva de uno DOC-36 y recibe dos tareas descartables distintas: una para ejecución y otra para `ui-lock`. El perfil no contiene credenciales, cookies, tokens, usuario, actividad, destino ni autorización; esos valores se solicitan en TTY o proceden del preview vigente y se eliminan del proceso hijo al terminar.

La extensión no crea un ambiente. Solo cuando se autoricen expresamente el ambiente y las cuentas de prueba puede derivarse un perfil con `create-doc37-workflow-user-previous-ui-profile.cjs` y ejecutarse el runner por etapas. DOC-37 rechaza combinaciones de etapas: cada invocación abre una sesión y selecciona una sola tarea autorizada con el comando oficial de la bandeja. Las huellas se toman después de esa precondición; la prueba no escribe campos ocultos, sesión ni invoca servicios internos para simularla. La comprobación inicial y final exige que `WorkflowCentroTrabajoModernActive` permanezca apagado, sin usuarios ni grupos, y que las consultas de estado y auditoría sean un único `SELECT` parametrizado.
