# Diagnóstico de GA_MANAGER_CONFIGURACION

## Decisión posterior aprobada

El usuario aprobó el 2026-10-20 permisos específicos separados: ADMINISTRAR_PERFILES_IMPORTACION para DOC-110 e IMPORTAR_CARPETAS para la entrega de carga, default 0 y sobre infraestructura existente. Se descarta reutilizar GA_MANAGER_CONFIGURACION. Las referencias siguientes a esa clave como candidata son antecedentes de investigación, no la política vigente. Ver D-05 en design.md.

Fecha: 2026-10-20. Investigación de código actual, archivos locales de código y puntos del historial Git. Sin consultas a datos operativos ni cambios de permisos.

## Conclusión

Es una capacidad almacenada en el perfil de Gestión y copiada a sesión. En el código revisado no existe un consumidor que utilice esta clave para permitir o rechazar una acción. Por tanto, no puede afirmarse que actualmente autorice la administración de perfiles de importación ni otra pantalla concreta.

DOC-110 podría consumirla sin crear otra columna de permiso, pero asignarle ese significado sería una política nueva sobre un dato existente, no mera reutilización de una autorización ya implementada. Antes de habilitarla hay que comprobar su población/configuración real y decidir quién debe recibir esa capacidad. No usar `GA_MANAGER_GESTION` como sustituto automático: sus consumidores muestran que es otro permiso con alcance propio.

## Recorrido verificado

```text
Global.Session_Start
  -> Session("GA_MANAGER_CONFIGURACION") = 0

ClassGestorSesion.FinalizeLogin
  -> identifica usuario de Gestión (directo o relacionado con otro módulo)
  -> AsignaPermisosPerfilUsuarioGestion(id)
     -> SELECT de remit_dest_interno_perfil_produccion por usuario
     -> Session("GA_IDUSUARIOGESTION") = id
     -> Session("GA_MANAGER_CONFIGURACION") = fila("MANAGER_CONFIGURACION")
     -> registra columnas en DETALLE_SESION como diagnóstico
  -> continúa construcción de sesión o devuelve fallo de finalización

Consulta de esta clave para autorizar acciones:
  -> no encontrada en el código revisado
```

## Evidencia precisa

| Punto | Ruta/símbolo | Resultado |
|---|---|---|
| Valor inicial | `Global.asax.vb:524`, Session_Start | Cero, no habilitación predeterminada. |
| Origen | `Gestion/Class_remit_dest_interno_perfil_produccion.vb:9` | Tabla `remit_dest_interno_perfil_produccion`; filtro `remit_dest_interno_idremit_dest_interno = IdUsuarioGestion`. |
| Asignación | Mismo archivo, línea 28 | Copia directa de columna `MANAGER_CONFIGURACION` a sesión; no convierte explícitamente a Boolean ni verifica 0/1. |
| Cardinalidad | Mismo archivo, acceso a `Rows(0)` | Usa primera fila; la función no demuestra unicidad del perfil por usuario. El esquema real debe confirmarlo. |
| Perfil inexistente | Mismo archivo, líneas 108–113 | Devuelve error. La función no limpia de forma explícita esta clave antes de la lectura/fallo. No se concluye que una sesión anterior sea explotable: requiere revisar la invalidación del flujo completo. |
| Integración de login | `Defaul/ClassGestorSesion.vb:2404`, `FinalizeLogin`; llamadas 2452/2552/2622/2690 | El permiso se carga durante finalización de acceso directo/relaciones de módulos; los errores se propagan por `LegacyFinalizationFailure`. |
| Otro recorrido | `Defaul/ClassGestorSesion.vb:2103`, `inicio_adplicacion_web_gestion_publico`; llamada 2157 | También carga perfil de usuario de consulta pública. Existencia de sesión/id positivo no demuestra que sea un administrador autenticado. No se inspeccionaron valores de ese perfil. |
| Diagnóstico genérico | Cargador, líneas 103–104 | Las columnas se agregan a `DETALLE_SESION`; esto es registro, no decisión de autorización. |
| Consumidores | Búsqueda literal y variantes en fuentes VB/C#/JS/ASPX/SQL/XML | Solo inicialización y asignación explícitas encontradas; no If, menú, ASMX o validación que consuma esta clave. |
| Administración del valor | Búsqueda de tabla/columna en fuentes y SQL versionados | No se identificó pantalla ni sentencia INSERT/UPDATE de este permiso. Puede administrarse fuera de este repo; no se afirma que no exista. |
| Historial | `41723871e` (carga inicial), `950fcf8e8` (copia legacy), cambios detectados con `git log --all -S` | Los puntos examinados muestran la misma inicialización/asignación y copias antiguas; no aportan un consumidor histórico. No se certifica todo software externo o binario. |

## Qué sí tiene consumidores

`GA_MANAGER_GESTION`, cargado desde la columna distinta `MANAGER_GESTION`, aparece en `ClassGaExpediente`, `ClassGaProducionDocumental`, `ClassGestionArchivo`, `ClassUnidadConservacion` y `WebFormGatoponimica`. Los permisos administrativos CCD/TRD/estructura también tienen consumidores. Esa evidencia respalda la infraestructura de permisos, pero no transfiere automáticamente sus semánticas a `GA_MANAGER_CONFIGURACION`.

## Hallazgo relacionado: ámbito existente

`FinalizeLogin` resuelve `GA_IDEMPRESA` mediante `Gestion/ClassAdmonEmpresa.vb:82`, `Retorna_id_empresa_usuario_gestion`. La consulta relaciona `remit_dest_interno.Empresa_Gestion_Documental_id_empresa` con `empresa_gestion_documental.ID_EMPRESA`. Es una fuente de empresa de negocio, distinta de la cadena de conexión, y candidata verificable para el ámbito del perfil. La función puede devolver `YES` con empresa cero cuando no encuentra fila: el nuevo adaptador debe rechazar contexto inválido, no aceptar cero como ámbito global.

No se define aquí unicidad entre instalaciones/bases ni un permiso nuevo. El namespace del ámbito debe acordarse con el contrato de despliegue antes de persistir identificadores canónicos.

## Recomendación para DOC-110

1. Reutilizar identidad, empresa y carga de perfil existentes; no otro login ni sistema de roles.
2. Tratar `GA_MANAGER_CONFIGURACION` como candidato, no permiso aprobado por su nombre.
3. Si se adopta, documentar exactamente crear/editar/publicar/desactivar, consultar el permiso en servidor y aceptar solo un valor habilitado válido; ausente, nulo, inválido o contexto de empresa no válido se rechaza.
4. Comprobar revocación/actualización de permisos durante una sesión: el cargador observado es una copia al iniciar, no evidencia de refresco automático.
5. Verificar en entorno autorizado tipo/default/unicidad y distribución agregada del campo, y localizar su administrador real. No publicar listas de usuarios ni valores sensibles. No ejecutar UPDATE para facilitar pruebas.
6. Antes de E2E real, disponer de cuenta autorizada y restringida sin modificar permisos globales; conservar PER-02 como prueba de rechazo por servidor.

## Corrección de la inferencia anterior

Que el campo exista no demuestra un permiso administrativo funcional ya utilizado. La calificación anterior de «candidato natural» era una inferencia por nombre; ahora queda acotada por la evidencia: dato cargado, sin consumidor de autorización localizado. Esto no es una incidencia de ejecución ni un fallo CSRF.
