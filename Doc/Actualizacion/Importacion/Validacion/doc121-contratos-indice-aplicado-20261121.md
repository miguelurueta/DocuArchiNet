# DOC-121 — índice de CONTRATOS aplicado y verificado, 2026-11-21

El usuario autorizó expresamente **«aplica por mi»** después de recibir el SQL específico de CONTRATOS. Esa instrucción posterior sustituye la ejecución manual por el usuario únicamente para el archivo 08 de esta operación. No extiende la autorización a corregir EGRESOS, conceder privilegios, ejecutar otros scripts o desplegar producto.

**Resultado: APLICADO Y VERIFICADO.** [Evidencia saneada](doc121-contratos-indice-aplicado-20261121.json). Se ejecutó una sola vez el ALTER entregado de [08-indice-contratos-ncontrato-doc121.sql](../Sql/08-indice-contratos-ncontrato-doc121.sql) en la base local cuya huella coincide con la inspección anterior. MySQL reportó 5.1.37-community. No repetir ese ALTER en esta base.

## Verificación previa y ejecución

Se volvió a comprobar motor InnoDB, VARCHAR(250), charset latin1, collation latin1_swedish_ci, atributos, ausencia de índices, cero nulos/vacíos y cero duplicados. Se validó el hash del archivo contra la entrega d3efffba y se restringió la ejecución al único ALTER de imp_02_contratos.NCONTRATO. No se generó otro DDL durante la ejecución.

Se habilitó STRICT_ALL_TABLES únicamente en la conexión de esta operación, conservando los demás modos de sesión. No se modificó configuración global. Así, una diferencia de datos no se convierte silenciosamente en una adecuación de valores. El ALTER fue reconocido por el servidor y se verificó posteriormente mediante SELECT, sin reintentos.

Balance de comandos explícitos: **15 SELECT, un ALTER, un SET SESSION; cero DML y cero HTTP**. Las credenciales se resolvieron en memoria mediante la configuración local existente, sin imprimirlas o guardarlas. No se utilizó esta operación para acreditar las concesiones de una cuenta dedicada futura.

## Resultado comprobado

| Propiedad | Antes | Después |
|---|---|---|
| NCONTRATO | VARCHAR(250), latin1_swedish_ci, nullable | Mismo tipo/charset/collation, NOT NULL |
| Índice | Ninguno | ux_doc121_NCONTRATO, UNIQUE de una columna completa, SUB_PART NULL |
| Filas | 625 | 625 |
| Claves NULL/vacías/duplicadas | 0/0/0 | 0/0/0 |
| Valores de FECHA/NCONTRATO/NIT/NOMBRE | Huella binaria de contenido | Misma huella |

La huella compara las cuatro columnas completas, incluyendo distinción entre NULL y texto vacío, en orden binario estable por NCONTRATO. No se guardaron los valores de negocio; solo conteos y hashes. También se contrastaron sin diferencias las definiciones de las otras tres columnas. La cabecera del SQL se actualizó para registrar su aplicación; su sentencia ALTER conserva exactamente el contenido entregado, con huellas de archivo y payload separadas en la evidencia.

## Alcance restante

EGRESOS no se modificó por esta operación. Sigue bloqueado por las incidencias observadas en el [corte de preparación](doc121-preparacion-indices-20261121.md): dos claves vacías y 738 grupos duplicados en la base inspeccionada. El usuario debe confirmar el destino y revisar su resolución antes de proponer otro SQL.

El paso 05 sigue sin verificar: falta identificar cuenta MySQL, host autorizado y DA, sin contraseña. Tampoco se acredita una resolución autenticada RA/DA de la aplicación, carga Excel, rollback de carga o recorrido E2E por agregar este índice. El índice no registra ni activa la tabla como fuente y no asigna CAMPO_UNICO a una plantilla.

Los cortes anteriores conservaron correctamente el estado «sin aplicar» de sus momentos de entrega. Este registro contiene la autorización y ejecución posteriores; la política manual sigue vigente para los demás scripts salvo instrucción expresa distinta del usuario.
