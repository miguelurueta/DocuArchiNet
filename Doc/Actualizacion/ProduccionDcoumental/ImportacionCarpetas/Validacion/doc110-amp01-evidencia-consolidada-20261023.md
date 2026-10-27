# DOC-110 — Evidencia consolidada de proveedores

Fecha: 2026-10-23. Estado: verificación local aprobada; aceptación integrada pendiente. Este informe actualiza el estado vigente sin convertir resultados históricos o simulados en aceptación real.

| Comprobación | Resultado | Alcance y límite |
|---|---|---|
| Compilación ASP.NET/VB | PASS | Build aislado; no despliegue a IIS ni copia de DLL a bin. |
| Harness .NET, cliente y editores | 20 PASS, 0 fallos | Incluye autorización exclusiva por empresa, ausencia de autorización, ámbito ajeno, filtros anteriores, claves de texto y ambigüedad. Repositorios/consultas simulados en pruebas de servicio. |
| Chromium: perfil y administrador de proveedores | 2 PASS | Interfaz real con transporte simulado: alta, configuración, validación, ensayo, publicación y segunda versión. No acredita ASMX/BD autenticada. |
| SQL 06 en RA | PASS | SELECT confirma tinyint unsigned NOT NULL DEFAULT 0. |
| Asignación de permisos | PENDIENTE | Cero filas con ADMINISTRAR_PROVEEDORES_IMPORTACION=1 y ADMINISTRAR_PERFILES_IMPORTACION=1. Conteo global, no login de usuario. |
| Política de fuentes del servidor | PENDIENTE | No se encontró configuración efectiva no vacía en el Web.config del repositorio. No se consultó configuración externa de otro despliegue. |
| Esquema DA.imp_02_egresos | PASS | Seis columnas varchar(250), nullable, coincidentes con el esquema aportado. SELECT real por conexión del módulo DOCUARCHI CONTENEDOR. |
| Calidad de CONSECUTIVO | REQUIERE DECISIÓN | 25.153 filas; 2 claves NULL o vacías tras TRIM; 738 grupos no vacíos con más de una fila según comparación de MySQL. |
| Formato FECHA | PENDIENTE 14.33 | El valor aportado identifica dd/MM/yyyy H:mm:ss; no se implementó ni probó su conversión en esta ejecución. El proveedor lo entrega como texto. |
| Creación/publicación real por interfaz | NO EJECUTADA | Requiere permiso, fuente autorizada, código disponible en el ambiente y sesión autenticada. |
| Concurrencia de transacciones MySQL | NO EJECUTADA | No se sustituye con el rechazo de revisiones obsoletas probado mediante dobles. |

## Hallazgo de datos

`CONSECUTIVO` no es único en los datos actuales. Los 738 grupos duplicados no equivalen a 738 filas: cada grupo contiene al menos dos. El conteo respeta la comparación/collation de MySQL, como la consulta del proveedor. No se extrajeron ni guardaron valores de consecutivos, NIT o beneficiarios.

El comportamiento implementado es consultar con clave parametrizada y LIMIT 2: cero coincidencias produce SIN_COINCIDENCIA; más de una, AMBIGUO. No se selecciona la primera fila ni se modifica la tabla para fabricar unicidad. El usuario debe decidir cómo resolver esos casos antes de aceptar el flujo completo: mantener rechazo, depurar los datos o definir un criterio adicional. No se presupone que CODIGOTIPO + CONSECUTIVO sea único.

## Comandos ejecutados

Desde la raíz:

```text
node --test tests/doc110-perfil-importacion-behavior.test.cjs tests/doc110-perfil-importacion-client.test.cjs tests/doc110-perfil-importacion-editor.test.cjs tests/doc110-proveedor-admin-editor.test.cjs
MSBuild.exe GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:OutputPath=.tmp/doc110-amp01-build/ /p:OutDir=.tmp/doc110-amp01-build/ /p:IntermediateOutputPath=.tmp/doc110-amp01-obj/ /clp:ErrorsOnly /nologo
```

Desde tools/e2e:

```text
npx.cmd playwright test tests/doc110-perfil-guided.spec.cjs --config=playwright.config.cjs
```

Consultas de evidencia real, usando conexiones resueltas desde el catálogo ODBC existente sin imprimirlas:

```sql
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='imp_02_egresos'
ORDER BY ORDINAL_POSITION;

SELECT COUNT(*),
 SUM(CASE WHEN CONSECUTIVO IS NULL OR TRIM(CONSECUTIVO)='' THEN 1 ELSE 0 END)
FROM imp_02_egresos;

SELECT COUNT(*) FROM (
 SELECT CONSECUTIVO FROM imp_02_egresos
 WHERE CONSECUTIVO IS NOT NULL AND TRIM(CONSECUTIVO)<>''
 GROUP BY CONSECUTIVO HAVING COUNT(*)>1
) d;
```

Lectura de permisos: 2026-10-23T10:26:46.9811181Z. Lectura de fuente: 2026-10-23T10:27:29.6438763Z. Las consultas tuvieron límite de ejecución de cinco segundos; los conteos son una observación, no una garantía de unicidad futura ni una instantánea transaccional conjunta.

No se ejecutaron DDL, INSERT, UPDATE o DELETE; no se modificaron imp_02_egresos, permisos, Web.config ni SII. No se desplegó código. Las tareas de aceptación real permanecen abiertas.
