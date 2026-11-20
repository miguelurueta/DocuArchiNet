# DOC-120 — primer expediente en el árbol

El usuario informa que recuperó el lote y terminó la importación, pero el expediente no apareció en la interfaz. Ese reporte se conserva como antecedente del usuario; este corte no vuelve a autenticar ni comprobar negocio.

Causa reproducida localmente: WebForms no emite nXNodes para un nivel sin hijos. Doc111ActualizarRama abandonaba la proyección si ese contenedor no existía y el asistente ignoraba su retorno. El código anterior falla con0 relaciones donde se esperaba1.

Corrección: para el destino original único visible con estructura válida, se reutiliza create_trenode_treview para crear el contenedor y expansor del primer hijo. Solo se proyectan identidades confirmadas; se comprueba la relación lógica única después de insertar. Se mantienen selección y expansión de ramas existentes; el primer hijo nuevo queda visible. Búsqueda, contador y clic usan las funciones existentes. No se insertan niveles ni destinos ocultos/ambiguos. Si la proyección no puede completarse, el asistente muestra un aviso y Actualizar estado puede intentarlo otra vez mediante consulta, sin repetir creación/recepción/almacenamiento. Los tres JS afectados quedan versionados en el ASPX.

74/74 casos locales únicos; cinco focales pertenecen a esa regresión y se repiten tras añadir la comprobación del expansor con TreeView.js real de System.Web. Sin sumar esos cinco otra vez. Se cubren primer expediente, deduplicación, clic/búsqueda/contador, texto escapado, selección, rama contraída, destino oculto/ajeno/ambiguo, aviso ante insertor fallido y recuperación visual mediante consulta. Sintaxis correcta en los tres JS. Sin delta VB/proyecto/esquema/backend, no se repite Framework ni se anuncia compilación nueva. Fuente/log/captura saneados con huellas en el JSON.

Uso: Ctrl+F5, Consultar importación o Consultar último lote, seleccionar el lote terminado y Actualizar estado. No volver a preparar/confirmar ni recibir archivos. Si el destino no está visible, localizarlo y consultar de nuevo. Las raíces corresponden a la página consultada y sus filtros vigentes; no se promete un barrido automático de todas las páginas.

57/61 tareas,36 escenarios; permanecen5.2,6.4,6.5,6.6.0 HTTP de aplicación,0 SELECT,0 sesiones/efectos reales del agente. La recuperación exitosa reportada no sustituye la validación autenticada de esta actualización visual ni la aceptación integral.
