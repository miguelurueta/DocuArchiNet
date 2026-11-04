# DOC-115 — integración independiente 07-A propuesta

Estado: preparación local; ejecución MySQL real NO_EJECUTADO. Se reutiliza run-doc111-persistence.ps1 con modo Doc115, el compilador y el arnés Framework existentes. No se ejecuta el modo histórico que activa perfil o exige tablas vacías. No se añade proyecto, login, endpoint ni configuración de pruebas paralelos.

## Alcance a autorizar

Ambiente local existente, cuenta de contexto 141/empresa 2, nivel 7, perfil 24/version 38 y gabinete 10. Tres planes de fixture con dos raíces cada uno: nombres DOC115-20261104-07A-01 a 06. Hasta dos lotes confirmados (cuatro raíces/cuatro archivos operativos); un intento de recepción y un PDF sintético íntegro de 677 bytes en staging. Cuatro eventos nuevos confirmados. No son expedientes ni documentos definitivos. Todos los recursos existentes y los nuevos se conservan.

Las escrituras usan exclusivamente MySqlImportacionCarpetasRepository y la conexión operativa registrada, resuelta en memoria por el mecanismo del ejecutor existente. No se pide URL MySQL ni contraseña de aplicación; la consola pedirá solo cuenta/contraseña MySQL exclusivamente SELECT para controles. No se imprimen ni persisten conexiones ni credenciales del módulo. Se verifican la huella del ambiente, autoridad IMPORTAR_CARPETAS, nivel y publicación actuales antes de escribir. El contexto es una fixture explícita de repositorio con sello propio; no pretende certificar resolución HTTP del destino ni sustituye las pruebas RECIB.

## Operaciones y aserciones

- Dos confirmaciones concurrentes del mismo plan con conexiones físicas nuevas por invocación (Pooling=false solo en la cadena efímera de test); convergencia por identidad y replay. Payload diferente bajo clave existente se rechaza.
- Dos planes diferentes compiten concurrentemente por otra clave: exactamente un lote y un conflicto.
- El plan perdedor permanece REVISABLE. Se reutiliza el observador interno existente para tres fallos controlados: antes de evento, después de evento y después de confirmar plan. Se demuestra rollback real de filas y conservación del lote confirmado.
- Dos reservas concurrentes del mismo archivo conservan una identidad de intento; una revisión obsoleta con otra solicitud se rechaza. Se recibe una única copia de PDF en staging.
- Un cuarto fallo controlado tras UPDATE provisional de recepción comprueba rollback de la fila sin borrar el completo de disco. Dos confirmaciones de recepción concurrentes conservan una sola revisión, intento y evento positivos.

Presupuesto de llamadas de repositorio: tres GuardarPlan, diez ConfirmarPlan (incluidas las fallidas y el replay de rollback), tres ReservarIntento y tres ConfirmarRecepcion (una fallida y dos concurrentes). El DDL no cambia ni se aplica; MySQL puede consumir identidades durante conflictos/rollback, sin prometer numeración continua.

## Protección y recuperación

Controles externos solo SELECT con cuenta de lectura. Las aserciones sobre filas provisionales se hacen mediante SELECT en la propia transacción del repositorio, porque otra conexión no ve los cambios sin commit. Se comparan huellas de todas las filas operativas preexistentes, perfiles, versiones, reglas y permisos; se verifica ausencia de nuevos expedientes/documentos/inventarios definitivos. También se comprueban los conteos exactos de nuevas filas. Usar una ventana sin otras importaciones que alteren las tablas observadas.

El diario conserva IDs de cada plan/lote/archivo/intento tan pronto se conocen. Ante incertidumbre se detiene; no repite ni borra registros. La disponibilidad del perfil, sus reglas, permisos, Web.config, backend y SII permanecen intactos. El arnés se compila en .tmp y no despliega DLL al sitio. La huella histórica del ambiente es una precondición: si la configuración registrada cambió, se detiene antes de cualquier escritura.

## Validación local

92/92 pruebas Node PASS, incluidas compuertas de autorización y compatibilidad de modos anteriores. 2/2 pruebas del harness Framework PASS: compila la parcial nueva y comprueba sus guards sin abrir BD. Sintaxis PowerShell PASS. Las carreras MySQL, rollbacks y recibo de esta fase aún no se ejecutaron. Comando de revisión: powershell.exe -NoProfile -File tools/e2e/scripts/run-doc111-persistence.ps1 -Mode Doc115 -ResourcesFile <archivo-propuesto> -RuntimeRoot <sitio-local> -Check. No usar -AuthorizedDoc115 antes de registrar la autorización específica.

Modo -Check de los recursos propuestos: PASS, sin compilacion ni conexiones. OpenSpec estricto y diff --check: PASS. Tras las ultimas comprobaciones SELECT se repitieron los tres tests del runner: PASS. No hay cambios de archivos VB, vbproj o Web.config de producto.
