# DOC-120 — capacidad de BENEFICIARIO en Egresos

**Resolución posterior2026-11-20:** el usuario confirma ampliadas tabla y perfil y aporta el video111928, con longitud255 visible. [Conciliación manual](doc120-beneficiario-confirmacion-manual-20261120.md):3.130/3.131 completas, sin repetir DDL/publicación. La medición60 y los estados detenidos de este documento/JSON siguen siendo fuentes históricas de esos ejecutores; no describen el estado posterior aportado por el operador.

Estado: **MEDICIÓN REAL COMPLETA; APLICACIÓN DETENIDA ANTES DE ESCRIBIR**. El usuario pidió «hazlo por mi» después del diagnóstico del índice17: medir origen, alinear columna/regla y publicar una versión nueva conservando textos y publicaciones anteriores. No incluye importar documentos, cambiar permisos o modificar el origen/otros índices.

## Medición

Run `0bca49d4e3f441418cceff0267df2be6`:9 SELECT concluidos,0 HTTP/sesiones/efectos; secretos de lectura eliminados. Usuario/empresa se resuelven por la cuenta elegida en consola antes de seleccionar el perfil y su proveedor. No se guardan textos de beneficiarios, cuentas, claves, cookies o conexiones.

| Recurso | Valor observado |
| --- | --- |
| Perfil | Egresos28, publicación43/v1; revisiones de cabecera2 y versión3 |
| Destino | Gabinete10, DA.`CONTABIL.BENEFICIARIO`, índice17; alias `NOMBRE BENEFICARIO` |
| Capacidad | Regla60; columna varchar(60), latin1/latin1_swedish_ci, nullable/defaultNULL/sin extra |
| Origen | DA.`imp_02_egresos.BENEFICIARIO`, varchar(250) |
| Población completa |25.153 registros; máximo77, también77 después de TRIM;230 valores mayores a60 |
| Índice observado | IDl_02, no único, sin prefijo. La aplicación verifica todos sus componentes |

Los ocho casos reportados pertenecen al subconjunto de prueba. Se elige250 para alinear el destino con el contrato del origen, sin recortar datos.

## Aplicación preparada y autorizada

`apply-doc120-egresos-length.cjs --measurement 0bca49d4e3f441418cceff0267df2be6 --run <runId-nuevo> --authorized-change` usa consola enmascarada, autenticación WebForms y safePost existentes. Requiere una cuenta administradora de perfiles y otra MySQL con ALTER en CONTABIL/UPDATE en su metadato; no recupera secretos de Web.config. El flag declara la autorización del usuario y no la sustituye.

1. Autenticar en el localhost autorizado; comprobar Egresos/empresa/gabinete/versión/revisiones y catálogo del índice17 antes de escribir.
2. El auxiliar nativo compara huella del origen DA con el perfil, definición completa, metadato legacy, engine e índice completo. Su máximo conservador de767 bytes evita ampliar una clave incompatible.
3. Ejecutar como máximo un ALTER de CONTABIL.BENEFICIARIO a VARCHAR(250), conservando latin1/latin1_swedish_ci y NULL/defaultNULL, y un UPDATE de detalle_gabienete.TIPO a VARCHAR(250), acotado por índice17/gabinete10/tabla/campo/valor anterior.
4. Verificar definición y conteo/agregados CRC de BENEFICIARIO antes/después. Estos controles detectan cambios; no son una copia de seguridad ni prueba criptográfica de toda la tabla. No se actualizan textos de documentos/origen.
5. Comprobar que el catálogo anuncia250; CrearVersion desde43, GuardarBorrador cambiando solo LongitudMaxima del índice17 y PublicarVersion con sus revisiones. Verificar publicación43 idéntica y resto de configuración/reglas intactos.

Topes:600 HTTP/1 sesión/12 SELECT de control,1 ALTER/1 UPDATE de metadato y máximo una llamada a cada mutación de perfil.0 planes/lotes/recepciones/expedientes/documentos. Primer error detiene sin replay. MySQL5.1 puede hacer commit implícito del DDL: conservar un efecto parcial, sin prometer rollback ni reducir silenciosamente a60. Si falla publicación, la columna ampliada puede coexistir con la regla anterior.

## Pruebas y pendiente operativo

14/14 locales: medición global, separación de límites, rechazo de origen/columna/empresa ajenos, conservación de publicación anterior, cambio único de regla, parada sin replay y rechazo nativo de alcance inválido antes de abrir ODBC. Sintaxis PowerShell y constructor ODBC sin apertura válidos; no sustituyen aplicación real.

Primer intento `d605ad7e452342adabb3682de351fd06` cerrado:96 HTTP/1 sesión/0 SELECT de control, sin ALTER/UPDATE/mutaciones de perfil; credenciales eliminadas. La guardia original rechazaba la fuente tipográfica opcional de gestor.aspx y ocultaba la causa bajo LOGIN_FAILED. La corrección omite esa fuente y la pantalla inicial, permite solamente la consulta vacía de estado del segundo factor ya usada por DOC119, mantiene denegados los demás efectos y conserva causas de login sanitizadas. Dos casos nuevos verifican ese límite.

Continuación `fa22661470ec4f8c940204ea383365a2` cerrada:98 HTTP/1 sesión/0 SELECT de control/0 ALTER/0 UPDATE/0 mutaciones de perfil. Font/landing omitidos y autenticación completada; la lectura de Egresos no coincide con la publicación o revisiones medidas y dispara `DOC120_LENGTH_CHANGE_PUBLICATION_CHANGED`. No se guardó qué comparación exacta difiere; no afirmar que se publicó una versión distinta sin observarla. Credenciales eliminadas y sin repetición automática.

La herramienta ahora conserva solo IDs/revisiones/indicador PUBLICADA en preflight, y valida regla60 antes de invocar el auxiliar físico. Esta mejora pasó localmente, sin otra ejecución autenticada. Falta refrescar la medición/observar la versión vigente y continuar con credenciales nuevas;3.130/3.131 permanecen abiertas. No existe cambio aplicado ni publicación nueva atribuible a estas corridas.

Tras una aplicación futura verificada, elegir la nueva versión de Egresos y preparar otra revisión: planes anteriores conservan su snapshot60. La importación real de1.000 PDF sigue pendiente.
