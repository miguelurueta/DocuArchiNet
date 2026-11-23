# DOC124: SQL17 aplicado y ventana REEMPLAZAR preparada

Después del PASS real de AGREGAR y su cierre, el usuario indicó «continua».
Se aplicó SQL17 conforme a la autorización previa «aplica los archivos por mi»,
cuya condición era aprobar primero AGREGAR. [Resultado SQL17](doc124-sql17-aplicado-20261123.json).
Se fijó el SHA3085b4bd98bc63ed323066349763afe9a3411ebadb8d080ff465e13f5f448833
del archivo entregado y solo se completó en memoria el ID esperado28.
LISTO_PARA_COMMIT y la comparación previa al commit acreditaron cambios únicamente
en ELIMINA_INSERTA_RECORD=1, INSERTA_RECORD=0 y REVISION_CARGA=2 de esa plantilla.
La relectura por el repositorio confirmó REEMPLAZAR y nueva huella
0f0a72e7594cb717db21e8134b01038a5ca4ae7e8e98071136e6ea0b77b7738d.
Los625 registros/esquema de CONTRATOS, campos, otras plantillas, fuentes/historial
y las ejecuciones anteriores conservaron sus huellas. No repetir SQL17.

Ventana nueva519d00f9d7df40a0926b5d6be2154d2c, aún sin ejecución real en este corte.
Reutiliza el mismo runner `--excel-replace`, suite Excel y consola con cuenta
propietaria resuelta por ID141, empresa2, DA11, conexión6, fuente5/revisión2 y
plantilla28/revisión2. Backend e1767f58a8d784bc0f958b8c141efad7363669a793eb0eb5c164aefed071e730
conservado; no hay despliegue nuevo. Necesita confirmación específica en TTY y
credenciales nuevas; las de AGREGAR ya fueron eliminadas.

| Caso | Contenido / resultado exigido |
|---|---|
| Válido,8000 filas | Una clave existente más7999 sintéticas; OK/PREPARADA/REEMPLAZAR. La coincidencia es admisible. Dos sesiones independientes: un ganador y OPERACION_EN_CURSO. |
| Duplicado interno,2 filas | Claves equivalentes por mayúsculas/espacio final; CLAVES_INVALIDAS y CLAVE_DUPLICADA_ARCHIVO, RECIBIDA. |

Se reutilizó el generador Book del harness y el lector DOC123, con encabezados
FECHA/NCONTRATO/NIT/NOMBRE y Hoja1. Ambos libros pasan lectura completa sin
incidencias; el lector conserva exactamente la clave existente y las diferencias
del caso negativo. Solo se guardan datos sintéticos y una clave privada, excluidos
de Git. Esta validación local no acredita aún la igualdad de MySQL en REEMPLAZAR.

Presupuesto de la nueva ventana:60 llamadas Excel/80 SELECT externos/20min;
máximo dos cargas, dos archivos y dos recibos idénticos por ID. Recuperación,
CSRF, permiso restringido, huellas de datos/esquema y catálogos antes/después,
consulta de ejecuciones, archivos y locks propios mediante controles existentes.
La E2E solo prepara: cero DELETE/INSERT de negocio, confirmación, DDL permanente,
guardado de plantillas, fuentes, permisos, limpieza o aplicación de SQL.

El resultado real se documentará aparte. Hasta acreditarlo,2.6/3.2 permanecen
abiertas y21/23 tareas. Los IDs/archivos de las cargas previas se conservan.

Relanzamiento solicitado después por «lanza de nuevo me equivoque»:519d00f9 se
interrumpió durante el ingreso de claves, con0 SELECT/servicios. Se comprobaron
ausentes la consola36332 y sus hijos Node; estado original conservado y registro
de interrupción separado, sin afirmar una limpieza de finally no registrada.
Nueva468f13144505a8faf2dfdd111bcaddbb, consola29028, mismo alcance/libros íntegros,
presupuesto sin consumo y credenciales nuevas. La instrucción expresa autoriza
el relanzamiento del mismo alcance sin repetir SI ni SQL17. No se recuperan claves
ni se reabren cargas de la ventana interrumpida. La apertura no acredita PASS.
