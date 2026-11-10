# DOC-118 — lectura resuelta y continuación detenida antes de login

La visibilidad de la consola se resolvió:la anterior estaba en CodexSandboxDesktop. Se preservó por hash su reporte pendiente y se abrió la misma auditoría en el escritorio normal mediante ejecución aprobada. El usuario pudo completar la captura SELECT.

## Resultado durable del lote25

La lectura terminó con25 SELECT nuevos:archivo42/expediente672/intento74 CONFIRMADO, documento16235/inventario23339 y verificación física por los mismos IDs. Archivo43/expediente673 sin almacenamiento iniciado y ausencia física verificada. Lote EN_CURSO, sin detención ni reclamaciones activas; snapshot estable. No se repitió el almacenamiento antiguo. Evidencia:doc118-auditoria-lote25-20261110.json. Este resultado reduce la incertidumbre del lote, pero no acredita OTHER_FILE, el corte controlado del retorno ni COORD03.

## Nueva ventana y consumo conservado

Se preparó una ventana adicional finita para los dos recorridos existentes, bajo la instrucción de continuar las tareas y el mismo ambiente/cuenta:2 sesiones compartidas,2 lotes nuevos,4 raíces/PDFs/expedientes,3 documentos máximos,1000 HTTP/240 SELECT y una señal de detención. Los topes son seleccionados por el agente bajo esa delegación, no una enumeración literal atribuida al usuario. El histórico acumulado1578 HTTP/172 SELECT/11 sesiones se conserva íntegro.

IIS estaba iniciado sin worker activo. Una sola HEAD gestor.aspx respondió200, sin redirects/retries y con contexto cerrado; quedó cargada al contador nuevo, no al libro histórico. Después el observador se conectó al worker25212 y enlazó la fuente. No hubo ARM, Freeze ni entrada al escritor.

La consola se detuvo en captura o en su comprobación previa al login:history vacío,phaseIndex0,0 intentos de sesión,0 SELECT,0 planes/lotes/documentos. El código genérico de la versión ejecutada no distingue la entrada concreta; no se atribuye el fallo a una contraseña, cuenta o base. SDK cerrado/cleanupConfirmed=true, secretos eliminados, sesiones cerradas y lease liberado. No hubo intento de autenticación que reintentar. Ventana consumida:1 HTTP/0 SELECT/0 sesiones; acumulado1579/172/11. Los recursos nuevos siguen sin crearse.

Se añadieron etapas cerradas de captura para una futura ejecución:APPLICATION_USER/APPLICATION_PASSWORD/SELECT_USER/SELECT_PASSWORD/RUNTIME_RECHECK y códigoCAPTURE_NOT_COMPLETED, sin valores ni causa privada. La regresión completa candidata posterior pasó268/268 Node;157 casos V2 internos incluidos sin sumarlos. Esa instrumentación posterior no se atribuye a la consola que ya había terminado. Falta aclarar si se completaron las cuatro entradas antes de revisar otra captura. No hay reintento automático ni aumento de presupuesto.

## Estado y procedencia

32/38 tareas;pendientes4.3/4.4/4.5/4.7/4.8/5.1. La verificación del lote25 es parcial respecto del inventario de todos los casos. Fuentes de almacenamiento compartido/SII, adaptadores protegidos, DLL/PDB y Web.config intactos. Archivo administrativo anterior intacto; Jira En curso. Los siete deltas de herramientas/pruebas se encadenan desde65f2bb4, conservando baselines anteriores. Los artefactos de ejecución y las credenciales no se versionan. Fuente:doc118-integracion-continuacion-20261110.json.
