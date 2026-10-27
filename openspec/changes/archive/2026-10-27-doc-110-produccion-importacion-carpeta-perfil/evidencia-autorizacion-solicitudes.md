# DOC-110 — Evidencia de autorización y protección de solicitudes

## Decisión posterior aprobada

El usuario aprobó el 2026-10-20 permisos específicos separados: ADMINISTRAR_PERFILES_IMPORTACION para DOC-110 e IMPORTAR_CARPETAS para la entrega de carga, default 0 y sobre infraestructura existente. Se descarta reutilizar GA_MANAGER_CONFIGURACION. Las referencias siguientes a esa clave como candidata son antecedentes de investigación, no la política vigente. Ver D-05 en design.md.

Fecha: 2026-10-20. Revisión estática, sin credenciales, cambios de configuración, consultas BD, peticiones autenticadas ni pruebas de explotación.

## Qué se verificó

| Evidencia | Hecho observado | Aplicación a DOC-110 |
|---|---|---|
| `Gestion/Class_remit_dest_interno_perfil_produccion.vb`, `AsignaPermisosPerfilUsuarioGestion`, líneas 9–29 | Consulta `remit_dest_interno_perfil_produccion` por usuario y carga identidad y permisos, entre ellos `GA_MANAGER_CONFIGURACION` y `GA_MANAGER_GESTION`. | Existe infraestructura de permisos administrativos. Reutilizarla; no crear otro sistema de roles ni inferir autorización de un permiso de carga. |
| Mismo archivo, líneas 55–65 | Carga permisos específicos de instrumentos, estructura y Producción. | Son permisos con semánticas distintas; no usar CCD/TRD o estructura como equivalentes arbitrarios del perfil nuevo. |
| `Defaul/ClassGestorSesion.vb`, llamadas a `AsignaPermisosPerfilUsuarioGestion` en líneas 2157, 2452, 2552, 2622 y 2690 | La construcción de sesión incorpora el perfil de Gestión desde varios recorridos de acceso. | Reutilizar sesión existente y comprobar contexto del módulo. No crear login administrativo paralelo. |
| `Global.asax.vb`, líneas 524 y 570 | Inicializa `GA_MANAGER_CONFIGURACION` y `PRODUCCION_MANAGER` en cero. | Son claves existentes; no activarlas desde frontend, pruebas o arranque de DOC-110. Ausencia o valor inválido debe negar operación en el adaptador nuevo. |
| `Gestion/ClassGagestorInicio.vb`, líneas 123, 140 y 147 | Permisos específicos condicionan nodos administrativos. | Reutilizar presentación por capacidades, complementada por autorización de cada operación servidor. |
| `Gestion/WebFormGatoponimica.aspx.vb`, `Button_me_active_men_dive_Click`, líneas 682–687 | Comprueba `GA_MANAGER_GESTION` o `GA_ADMINISTRACION_ESTRUCTURA_ARCHIVO` antes de continuar una acción administrativa. | Hay precedente servidor de comprobación de permisos; no significa que esos mismos permisos autoricen publicar perfiles. |
| `Gestion/ClassGaProducionDocumental.vb`, `SolicitaCargarDocumentoExpediente` | Obtiene `GA_IDUSUARIOGESTION` y verifica propiedad/carga del nivel. | Identidad y elegibilidad del nivel se apoyan en mecanismos existentes; el permiso de administrar configuración es distinto. |
| `Web.config`, línea 77 | Declara cabecera `Access-Control-Allow-Origin: *`. | No se interpreta como protección CSRF, autorización ni prueba de explotación. No modificar CORS global para DOC-110. |

## Alcance de búsqueda CSRF

Se buscaron en fuentes VB/C#/JavaScript y en archivos versionados términos `csrf`, `xsrf`, `antiforgery`, `ViewStateUserKey`, `UrlReferrer`, `X-Requested-With`, validación de origen y variantes de token. Se revisaron entrada ASMX de Producción, código de sesión, configuración relevante y arranque global. La búsqueda de archivos versionados complementó `rg` para no depender exclusivamente de sus exclusiones.

No se identificó un componente explícito de CSRF reutilizable para el nuevo ASMX en las fuentes revisadas. No se verificaron módulos IIS externos, configuración efectiva del despliegue ni comportamiento del framework con peticiones de otro origen. Por tanto, esto NO acredita ausencia absoluta de protección, vulnerabilidad explotable ni regresión.

Una sesión autenticada, un permiso administrativo o un token de operación Workflow no prueban por sí solos protección de solicitudes de administración. Tampoco un PASS de OpenSpec/prompt-review verifica el comportamiento HTTP.

## Decisiones para continuar

1. **Reutilización de permisos confirmada.** `GA_MANAGER_CONFIGURACION` es candidato natural para administración del perfil por su origen y nombre; la inspección de consumidores versionados solo encontró carga/inicialización de esa clave, no una operación previa que demuestre su alcance exacto. Su asignación a crear/editar/publicar/desactivar debe quedar explícita como propuesta de política, no como comportamiento ya existente ni aprobación implícita.
2. Mantener lectura/selección y permisos de nivel separados de administración. Revalidar pertenencia y ámbito en servidor; no otorgar permisos por cambiar un nodo o un valor cliente.
3. **CSRF como requisito de implementación verificable.** Priorizar mecanismo compatible existente si se identifica en el entorno o mediante investigación adicional. Si no existe uno aplicable, diseñar protección acotada del ASMX nuevo y sus pruebas antes de habilitar mutaciones, sin cambiar SII, login ni políticas globales. Esto no impide continuar modelos, contratos o pruebas puras.
4. No solicitar nuevas credenciales ni cambios de permisos para redactar el diseño. Pruebas autenticadas siguen necesitando ambiente/cuentas/datos descartables autorizados.
5. No marcar `refinement:approved` solo para pasar el comando. Diferenciar definición de política, evidencia de implementación y evidencia de ejecución.

## Corrección del diagnóstico anterior

La afirmación de que faltaba identificar infraestructura administrativa era incompleta: el repositorio sí la contiene. Sigue pendiente concretar su mapeo a la capacidad nueva, no construir un sistema de permisos desde cero. Para CSRF se conserva una conclusión limitada a evidencia estática, no un incidente confirmado.
