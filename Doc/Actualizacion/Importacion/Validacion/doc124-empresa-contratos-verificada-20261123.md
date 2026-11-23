# DOC-124: empresa de CONTRATOS comprobada

La configuración vigente vincula `imp_02_contratos` a la empresa2 en esta
instalación. Se contrastó con la [decisión previa del usuario de empresa
única](../../ProduccionDcoumental/ImportacionCarpetas/Exploracion/criterio-empresa-unica-doc110.md).
No hace falta solicitar nuevamente esa confirmación.

El2026-11-23 se realizaron12 consultas SELECT con los recursos de conexión del
repositorio, sin login web, E2E ni cambios de datos, esquema, permisos o fuentes.
Se reutilizó la resolución del catálogo central configurado en Web.config y las
cuentas de los módulos existentes. No se leyeron credenciales históricas de
`imp01_xmlconexion` ni se guardaron secretos en la evidencia.

| Comprobación actual | Resultado |
|---|---|
| Empresas en catálogo central | Una: ID2. |
| Empresas en RA | Una: ID2. |
| Módulo DA11 / módulo RA12 | Ambos pertenecen a la empresa2; tipos esperados. |
| Módulos que comparten servidor/base DA, incluidos inactivos | Todos pertenecen a la empresa2. |
| Identidad física DA | Coincide con el destino previamente comprobado de DOC124. |
| Referencia histórica6 | Coincide con servidor/base del módulo DA. |
| Filas de CONTRATOS |625. |
| NCONTRATO nulo/vacío/grupos duplicados |0/0/0. |
| Índice físico | UNIQUE completo ux_doc121_NCONTRATO; sin prefijo. |
| Política RA para DA/imp_02_contratos | No existe, activa ni inactiva. |
| Plantilla CONTRATOS/imp_02_contratos | No existe en el catálogo DA inspeccionado. |
| Relación empresarial por columna/FK de CONTRATOS | No existe en su esquema actual. |

La atribución empresarial procede de la decisión expresa previa más la
configuración de módulos comprobada; no se deduce del prefijo de tabla, de los
NIT de los contratistas ni de la ausencia de una columna de empresa. La tabla
no conserva procedencia empresarial por fila: estas consultas no reconstruyen
el origen histórico de sus625 registros.

Para la configuración posterior corresponde CONEXION exclusiva de empresa2,
sujeta al catálogo explícito exigido por el cargador. Esta comprobación no crea
ni activa esa política. Preparar seguirá rechazando ese destino con
POLITICA_DESTINO_NO_DEFINIDA mientras no se configure por la vía autorizada.
También falta una plantilla compatible con NCONTRATO y sus campos de texto.
Las altas se entregarán en SQL separado para aplicación manual; no modificar
CONTRATOS ni repetir el índice08 para resolver esta configuración.

Las consultas comprobaron empresas, identidad/asociaciones de módulos, políticas,
columnas/índices, conteos, referencia histórica, plantillas y relaciones FK.
La evidencia saneada detallada queda localmente en
`.tmp/doc124-empresa-contratos-select.json`; script de solo lectura en
`.tmp/doc124-investigar-empresa-contratos.ps1`, ambos excluidos de Git.
No se exportaron claves, nombres, NIT, cookies ni cadenas de conexión.
No se ejecutó el archivo SQL14 como lote: sus aspectos pertinentes se
contrastaron mediante estas consultas específicas adicionales.

La matriz real AGREGAR/REEMPLAZAR sigue pendiente por configuración, no por
incertidumbre sobre la empresa configurada. DOC124 conserva21/23 tareas.
