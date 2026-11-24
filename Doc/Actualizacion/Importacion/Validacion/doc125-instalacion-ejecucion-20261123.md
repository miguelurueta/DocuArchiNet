# DOC125: instalación y despliegue, 2026-11-23

La instrucción del usuario «ejecuta» delega la aplicación administrativa de los SQL entregados y el despliegue necesario para los cinco casos E2E locales anunciados. Esta excepción al flujo manual se realizó fuera de los tests; no introduce instalación desde el sistema ni autoriza otros destinos.

SQL18 ejecutó diez controles SELECT con la cuenta real del módulo: MySQL 5.1.37-community, InnoDB, 22 columnas operativas, SELECT global y TRIGGER acreditados, sin triggers ni FK en el registro de ejecución. Los fixtures, fuentes y plantillas estaban ausentes. No se concedieron privilegios.

SQL19 creó dos tablas descartables. SQL20 se confirmó tras LISTO_PARA_COMMIT: fuentes 6 y 7, activas en revisión2, empresa2, CLAVE/VALOR e historial ALTA/ACTIVAR canónico. SQL21 se confirmó tras LISTO_PARA_COMMIT: plantillas29 CREAR,30 AGREGAR y31 REEMPLAZAR, revisión1, conexión6, Datos, dos VARCHAR(250), más tres filas sintéticas iniciales. El destino CREAR sigue ausente. Relectura mediante sesiones nuevas; huellas completas de CONTRATOS (625 filas), EGRESOS, CRÉDITOS y configuración previa sin cambios. No repetir estos scripts.

Se respaldó y sustituyó sólo la DLL de aplicación por el build verificado 0646c354af0d6a7b1aaa5f61e3a880b9c58697a1a0e75a6e055d833e221d34c9; WSDL200 con Confirmar/ConsultarResultado, Web.config intacto. Se conservan versiones de framework/conector, límites20000filas/10000000bytes y dependencias existentes.

La consola privada de la E2E existente está abierta para credenciales vigentes. Este corte acredita instalación/despliegue; todavía no acredita login, llamadas autenticadas ni cinco casos reales. Scope congelado: empresa2/usuario141/DA11/conexión6, tablas imp_02_doc125_e2e_,1000/1000/1000/300/10filas, una fila externa, máximos4000filas previstas/1002eliminaciones de fixture; conservar todos los efectos. Acuse ignorado por cliente no equivale a corte físico de red.

Tarea4.5 completa;27/31. Pendientes5.3,5.4,5.5 y5.7. Resultado posterior en evidencia separada; las comprobaciones locales previas no se presentan como ejecución real. Las credenciales permanecen sólo en memoria de la consola, excluidas de evidencia y Git.
