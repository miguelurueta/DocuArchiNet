# DOC-110: descubrimiento administrativo y ordinario de metadatos

Fecha: 2026-10-27. Verificación acumulada de 14.25 y 15.7, D-09/RQ-12. Esta continuación amplía pruebas locales; no cambia código productivo ni inicia autenticación o conexión real.

## Dos facultades y dos recorridos

ConsultarMetadatosFuente exige AutorizarFuentes del contexto servidor antes de MetadatosFuenteImportacion. Permite al administrador autorizado descubrir candidatos RA/DA de su sesión sin depender de una fuente ya registrada. Aplica nombres seguros, prefijo literal imp_02/excepciones exactas y consulta únicamente information_schema de DATABASE(), con páginas de 25, parámetros y timeout de cinco segundos. No recibe esquema físico, credenciales ni SQL.

ConsultarMetadatosProveedor exige AutorizarProveedores y carga la política RA. MetadatosProveedorIndices ofrece solo alias/tablas/columnas autorizados del ámbito actual; registrar un proveedor no concede facultad para descubrir candidatos adicionales. CONEXIONES/TABLAS se derivan de la política y COLUMNAS solo resuelve el origen tras comprobar pertenencia de tabla. La denegación por facultad separada se prueba localmente antes de I/O; no se atribuye a esta continuación una nueva corrida con otra cuenta.

Las respuestas usan opciones de nombre/etiqueta/tipo/longitud; la selección física utiliza parámetros o identificadores previamente autorizados, no texto SQL del cliente. No se devuelven cadenas de conexión ni filas de negocio. Las conexiones se resuelven exclusivamente en servidor por alias RA/DA.

## Nuevas pruebas de comportamiento

PoliticaFuentesProveedorBehaviorTests construye 26 autorizaciones RA, una DA y una tabla de otro ámbito. Exige alias lógicos ordenados, segunda página de conexiones vacía, páginas de tablas de 25/1/0 y 26 nombres únicos sin el ajeno. La tabla de otro ámbito, un nombre manipulado con expresión y DATABASE() se rechazan antes de resolver origen. Las páginas 0/1001 del descubrimiento ordinario se rechazan; contador de resolución de conexiones permanece cero.

FuenteImportacionBehaviorTests añade candidatos administrativos con página cero, alias DATABASE(), tipo SELECT, identificador SQL manipulado y prefijo parecido impX02: todos se rechazan antes del callback de conexión. Mantiene las pruebas previas de falta de facultad y contexto inválido. No simula una consulta SQL exitosa para afirmar que se leyó information_schema real.

```powershell
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

**1 prueba Node PASS**, que compila y ejecuta el arnés completo de componentes .NET Framework, incluidas las nuevas comprobaciones. No es build completo WebForms ni integración MySQL. Sin nuevos tests que repliquen solo cadenas de implementación: se comprueban opciones, paginación y rechazo antes de I/O sobre las clases productivas.

## Evidencia autenticada existente

| Evidencia | Qué acredita |
|---|---|
| [Restricción de fuentes, corrida 2026-10-25](doc110-continuacion-20261024.md) | --source-scope, 1 passed (18.7 s): metadatos RA/DA, prefijo literal, nombres ajenos rechazados por conexión y seis columnas TEXTO(250) de egresos. La salida no demuestra suficientes tablas reales para más de una página; esa condición se prueba localmente. |
| [Alta y continuidad de créditos](doc110-fuentes-aceptacion-preparada-20261026.md) | Candidatos de conexión DA disponibles antes del alta, CONSECUTIVO TEXTO y FECHADOCUMENTO FECHA, fuente registrada por UI disponible en proveedor y columna retirada excluida de sus metadatos. --continue-source, 1 passed (23.8 s). |
| [Diagnóstico de fuente revocada](doc110-fuente-referencias-guardadas-20261027.md) | --provider-diagnostics, 1 passed (17.4 s): referencia publicada conservada, diagnóstico sin sustitución y sin consultar columnas de tabla retirada desde UI. |
| [Guard de fuentes](doc110-guard-fuentes-20261027.md) | 17 rechazos HTTP, identidad enviada por cliente rechazada; fuente e historial sin cambios. No equivale a una nueva prueba de aislamiento entre empresas. |

## Estado y límites

14.25 y 15.7 completas con implementación, arnés de comportamiento y evidencia autenticada acumulada. OpenSpec estricto y diff de espacios PASS. No se amplía la política de nombres ni se modifica Web.config, permisos, DA o SII; sin SQL ni despliegue.

15.19 continúa pendiente del proveedor previamente persistido fuera del prefijo: la corrida histórica no encontró candidato y quedó skipped, no PASS. Tampoco se cierran aislamiento empresarial, concesión/revocación externa de permisos, concurrencia/rollback de fuentes ni regresión de carga individual. No se crean excepciones o fuentes inseguras para fabricar esos escenarios.
