# DOC-121 — confirmación y verificación parcial de instalación, 2026-11-21

El usuario informó **«achivos aplicados»**. Se registra como confirmación general de ejecución manual, sin atribuirle todavía una lista específica de scripts, cuenta dedicada o destino RA/DA. Se solicitó precisar esos puntos. Este corte complementa la [entrega contractual del 20 de noviembre](doc121-revision-fase01-20261120.md); no sustituye su evidencia histórica.

## Comprobación realizada

Se reutilizó la resolución de conexión local del helper existente `tools/e2e/scripts/inspect-doc111-permissions-readonly.ps1`, con comparación de la huella servidor/base del ambiente DOC-120 previamente inspeccionado. Credenciales cargadas solo en memoria, sin imprimirlas ni guardarlas. Se realizaron **8 SELECT**, cero DDL, cero DML y cero HTTP. El [resultado saneado](doc121-verificacion-instalacion-20261121.json) contiene la huella del registro local excluido de Git y los hashes de los siete scripts entregados.

Se contrastaron tipos, nulabilidad, defaults, charset/collation, ordinales e índices del CREATE entregado con los metadatos observados: **37 comprobaciones aprobadas, cero diferencias en la estructura base 01–03**. MySQL reporta 5.1.37-community e InnoDB disponible.

| Paso | Resultado en la base física inspeccionada |
|---|---|
| 01, permiso RA | ADMINISTRAR_CARGAS_IMPORTACION existe como TINYINT UNSIGNED NOT NULL DEFAULT 0. Esto verifica la columna, no la habilitación de una cuenta de aplicación. |
| 02, cabecera DA | CAMPO_UNICO VARCHAR(64) ASCII/binario nullable en ordinal 11 y REVISION_CARGA INT UNSIGNED NOT NULL DEFAULT 1 en ordinal 12; diez ordinales históricos conservados. |
| 03, ejecución DA | imp01_carga_ejecucion InnoDB/ascii_bin con 22 columnas conforme al CREATE, PK y tres índices secundarios completos, sin FK o triggers nuevos. Se observan las dos FK históricas entre catálogos. |
| 04, claves de fuente | En esta base, CONSECUTIVO y NCONTRATO admiten NULL y no hay índice UNIQUE en sus tablas. Ejecutar el generador SELECT no aplica sus ALTER propuestos. La adecuación no queda acreditada. |
| 05, cuenta dedicada | No verificado: falta identificar la cuenta/host y el GRANT específico aplicado por el DBA, sin solicitar ni publicar su contraseña. La cuenta usada para inspeccionar metadatos no acredita esa concesión. |

## Alcance y diferencias que necesitan contexto

La observación corresponde a la base física local de la configuración inspeccionada, no a una resolución autenticada de RA/DA por sesión de aplicación. El usuario debe confirmar si esos son los destinos donde aplicó los archivos o indicar los destinos correctos. Si DA es otra base, las tablas fuente y sus índices deben verificarse allí; no extrapolar esta observación a otro ambiente.

BENEFICIARIO aparece como VARCHAR(250) en la base inspeccionada. El usuario había informado una ampliación de su fuente/perfil: esta lectura por sí sola **no demuestra una regresión de esa ampliación**, porque no se ha acreditado que se trate del mismo destino DA utilizado por ese perfil. Ningún SQL de esta entrega modificó BENEFICIARIO desde el agente.

Estado: **ejecución manual informada; esquema base 01–03 verificado por SELECT en la base inspeccionada; índices específicos, cuenta dedicada y correspondencia RA/DA por precisar**. No se exige repetir DDL ya aplicado. Un objeto compatible se verifica y se omite. Cualquier diferencia que requiera modificación produce otro SQL específico para ejecución manual.

No se abrieron sesiones E2E, importaciones o cargas; no se certifican permisos funcionales, capacidad Excel, rollback o concurrencia por esta comprobación de metadatos. Los archivos SQL entregados se conservaron sin cambios.
