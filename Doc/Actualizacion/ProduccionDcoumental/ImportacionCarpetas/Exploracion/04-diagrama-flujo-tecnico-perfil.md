# Referencia: flujo técnico del perfil de importación

Lectura de las decisiones del diagrama: «errores que bloquean» depende del modo y ámbito de integridad, no significa cualquier error individual. En parcial, el servicio devuelve unidades elegibles y `puedeConfirmar`, manteniendo rechazados visibles. Los valores de expediente aún inexistentes se representan como fuentes diferidas hasta creación. El recorrido es conceptual: no presupone un worker desatendido ni un framework MVC. Prevalecen los contratos transversales de la guía 00.

Precisión funcional posterior: antes del recorrido de selección/aplicación, el usuario selecciona un nivel en el árbol existente de Producción Documental y desde allí abre la ventana de importación. La ventana recibe ID/ámbito del nivel, muestra su ruta como solo lectura y consulta perfiles compatibles. No vuelve a seleccionar ni crea estructuras. El contexto se valida en servidor y se conserva en el plan/lote; cambiar de perfil o navegar en otra pestaña no cambia el destino de ese lote.

Fecha: 2026-10-19.

Estado: referencia de exploración; operaciones propuestas, no implementadas.

El perfil tiene dos recorridos: configuración y publicación, y selección para una importación. Ambos pertenecen a la nueva integración; SII permanece intacto.

Los nombres de operaciones son propuestos. Controller representa la entrada HTTP compatible con la aplicación ASP.NET. Los diagramas se conservan en texto, con cajas, flechas y decisiones.

```text
RECORRIDO 1: CREAR Y PUBLICAR UN PERFIL
=====================================

┌──────────────────────────────────────────────────────────┐
│ CLIENTE — Administración de perfiles                     │
│                                                          │
│ El usuario selecciona «Nuevo perfil».                    │
└────────────────────────────┬─────────────────────────────┘
                             │ Solicitar catálogos
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER                                               │
│                                                          │
│ • Obtiene la identidad autenticada.                      │
│ • Recibe la solicitud.                                  │
│ • Solicita al Service los datos para configurar.         │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE                                                  │
│                                                          │
│ Comprueba autorización para administrar perfiles.       │
└────────────────────────────┬─────────────────────────────┘
                             ▼
                  ┌───────────────────────┐
                  │ ¿Tiene autorización? │
                  └───────────┬───────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ RESPUESTA            │  │ REPOSITORY                     │
│ Acceso rechazado.    │  │                                │
└──────────────────────┘  │ Consulta catálogos existentes: │
                          │ • Clasificación documental.   │
                          │ • Tipos de expediente.         │
                          │ • Gabinetes y sus campos.      │
                          │ • Tipologías y niveles.        │
                          └───────────────┬────────────────┘
                                          │
                                          ▼
┌──────────────────────────────────────────────────────────┐
│ REPOSITORY → SERVICE → CONTROLLER → RESPUESTA             │
│                                                          │
│ Catálogos disponibles para el usuario y contexto.        │
│ No se duplican ni modifican los catálogos existentes.    │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CLIENTE — Completar el perfil                            │
│                                                          │
│ Identificación:                                          │
│ • Nombre y descripción.                                 │
│                                                          │
│ Configuración documental:                                │
│ • Clasificación del expediente.                         │
│ • Valores predeterminados requeridos.                   │
│ • Gabinete destino.                                     │
│                                                          │
│ Interpretación:                                          │
│ • Cómo convertir carpetas en expedientes.               │
│ • Cómo resolver nombre visible y código.                │
│ • Cómo obtener tipología y campos índice.               │
│                                                          │
│ Políticas:                                               │
│ • Coincidencias.                                        │
│ • Restricciones de carga.                               │
└────────────────────────────┬─────────────────────────────┘
                             │
                             ▼
                  ┌───────────────────────┐
                  │ ¿Agregar una regla de │
                  │ asignación de campo?  │
                  └───────────┬───────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              │                    ▼
              │  ┌───────────────────────────────────────┐
              │  │ CLIENTE — Configurar una regla         │
              │  │                                       │
              │  │ Selecciona:                           │
              │  │ • Campo destino.                      │
              │  │ • Fuente del valor.                   │
              │  │ • Extracción, cuando corresponda.     │
              │  │ • Conversión y obligatoriedad.        │
              │  │ • Precedencia ante otras fuentes.     │
              │  │                                       │
              │  │ Fuentes posibles:                     │
              │  │ • Valor constante.                    │
              │  │ • Nombre de carpeta.                  │
              │  │ • Nombre de archivo.                  │
              │  │ • Campo del expediente.               │
              │  │                                       │
              │  │ No hay separador, posiciones ni       │
              │  │ cantidad de partes fijos.             │
              │  └───────────────────┬───────────────────┘
              │                      │
              └──────────────────────┘
                             │ Guardar borrador
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER                                               │
│                                                          │
│ Valida la estructura de la solicitud y envía al Service: │
│ • Datos del perfil.                                     │
│ • Configuración de la versión.                          │
│ • Reglas declaradas.                                    │
│ • Identificador de revisión, si se está editando.        │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE — Validar guardado                               │
│                                                          │
│ • Verifica autorización y ámbito.                       │
│ • Valida los datos que se hayan informado.              │
│ • Comprueba que los destinos de reglas sean permitidos.│
│ • Detecta conflictos de edición concurrente.            │
│                                                          │
│ Un borrador puede tener campos pendientes.              │
│ No permite guardar una configuración mal formada.      │
└────────────────────────────┬─────────────────────────────┘
                             ▼
                  ┌───────────────────────┐
                  │ ¿Puede guardarse?     │
                  └───────────┬───────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ RESPUESTA            │  │ REPOSITORY                     │
│ Errores por campo o  │  │                                │
│ conflicto de edición.│  │ Guarda en una transacción:     │
└──────────────────────┘  │ • Perfil.                      │
                          │ • Versión en borrador.          │
                          │ • Reglas de esa versión.        │
                          └───────────────┬────────────────┘
                                          ▼
┌──────────────────────────────────────────────────────────┐
│ REPOSITORY → SERVICE → CONTROLLER → RESPUESTA             │
│                                                          │
│ • Identificador del perfil y versión.                   │
│ • Estado: BORRADOR.                                     │
│ • Datos pendientes para publicar.                      │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CLIENTE                                                  │
│                                                          │
│ Revisa la configuración y selecciona «Publicar».         │
└────────────────────────────┬─────────────────────────────┘
                             │ ID del perfil y versión
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER → SERVICE                                     │
│                                                          │
│ Solicita publicar la versión guardada.                  │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE → REPOSITORY                                     │
│                                                          │
│ Recupera el borrador persistido y los datos actuales     │
│ de catálogos, campos y permisos necesarios.              │
└────────────────────────────┬─────────────────────────────┘
                             │ Datos recuperados
                             ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE — Validación completa de publicación             │
│                                                          │
│ • Clasificación existente y coherente.                  │
│ • Gabinete válido.                                     │
│ • Campos destino existentes y compatibles.              │
│ • Fuentes definidas para los campos obligatorios.       │
│ • Reglas de extracción y conversión válidas.            │
│ • Precedencia explícita cuando coinciden fuentes.       │
│ • Política de tipología y coincidencias definida.       │
│ • Restricciones compatibles con el sistema.             │
│                                                          │
│ Aquí se valida la configuración. Los valores concretos │
│ de cada archivo se validarán durante la importación.   │
└────────────────────────────┬─────────────────────────────┘
                             ▼
                  ┌───────────────────────┐
                  │ ¿Configuración        │
                  │ completa y válida?    │
                  └───────────┬───────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ RESPUESTA            │  │ REPOSITORY                     │
│                      │  │                                │
│ Mantiene BORRADOR.   │  │ Publica la versión de forma    │
│ Devuelve errores y   │  │ atómica, comprobando que no    │
│ campos pendientes.   │  │ cambió desde su validación.    │
└──────────────────────┘  └───────────────┬────────────────┘
                                          ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE → CONTROLLER → RESPUESTA                         │
│                                                          │
│ Perfil publicado y disponible para usuarios autorizados.│
│                                                          │
│ La versión publicada no se edita.                       │
│ Un cambio posterior se prepara como otra versión.      │
└──────────────────────────────────────────────────────────┘
```

```text
RECORRIDO 2: SELECCIONAR Y APLICAR UN PERFIL
==========================================

┌──────────────────────────────────────────────────────────┐
│ CLIENTE — Nueva importación                              │
│                                                          │
│ Solicita los perfiles disponibles.                      │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER → SERVICE                                     │
│                                                          │
│ Obtiene el contexto autenticado y verifica permiso      │
│ para iniciar importaciones.                             │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ REPOSITORY                                               │
│                                                          │
│ Consulta perfiles activos con una versión publicada     │
│ y su información de acceso.                             │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE → CONTROLLER → RESPUESTA                         │
│                                                          │
│ Lista autorizada: ID, nombre y descripción.             │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CLIENTE                                                  │
│                                                          │
│ Selecciona un perfil y solicita su detalle.             │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER → SERVICE → REPOSITORY                        │
│                                                          │
│ Recupera la versión publicada y sus reglas.             │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE                                                  │
│                                                          │
│ Comprueba que el perfil sigue disponible y autorizado. │
│ Verifica las referencias necesarias para preparar      │
│ una importación con esa versión.                        │
└────────────────────────────┬─────────────────────────────┘
                             ▼
                  ┌───────────────────────┐
                  │ ¿Perfil utilizable?   │
                  └───────────┬───────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ RESPUESTA            │  │ RESPUESTA                      │
│                      │  │                                │
│ Motivo del rechazo.  │  │ Resumen de clasificación,     │
│ No permite continuar.│  │ gabinete, reglas y versión.    │
└──────────────────────┘  └───────────────┬────────────────┘
                                          ▼
┌──────────────────────────────────────────────────────────┐
│ CLIENTE                                                  │
│                                                          │
│ • Revisa el perfil.                                    │
│ • Selecciona carpetas y archivos.                       │
│ • Envía inventario + referencia a la versión.           │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER → SERVICE                                     │
│                                                          │
│ El Service recupera las reglas desde Repository.        │
│ No confía en reglas o valores calculados por el cliente.│
│                                                          │
│ Aplica el perfil al inventario:                         │
│ • Agrupa archivos por expediente propuesto.             │
│ • Completa valores constantes.                         │
│ • Extrae valores de carpetas y nombres de archivos.     │
│ • Convierte y valida los valores.                       │
│ • Resuelve tipología y campos índice.                   │
└────────────────────────────┬─────────────────────────────┘
                             ▼
                  ┌─────────────────────────┐
                  │ ¿Los datos particulares │
                  │ cumplen las reglas?     │
                  └───────────┬─────────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ RESPUESTA            │  │ RESPUESTA                      │
│                      │  │                                │
│ Errores por archivo, │  │ Vista previa:                 │
│ carpeta y campo.     │  │ • Expedientes propuestos.      │
│                      │  │ • Documentos y destinos.       │
│ El usuario corrige   │  │ • Valores índice calculados.   │
│ y vuelve a validar.  │  │ • Conflictos y advertencias.   │
└──────────────────────┘  └───────────────┬────────────────┘
                                          ▼
┌──────────────────────────────────────────────────────────┐
│ CLIENTE                                                  │
│ Confirma el plan de importación.                        │
└────────────────────────────┬─────────────────────────────┘
                             ▼
┌──────────────────────────────────────────────────────────┐
│ CONTROLLER → SERVICE                                     │
│                                                          │
│ Revalida autorización y vigencia del plan.              │
└────────────────────────────┬─────────────────────────────┘
                             ▼
                  ┌───────────────────────┐
                  │ ¿El plan sigue siendo │
                  │ válido?               │
                  └───────────┬───────────┘
                         NO   │   SÍ
              ┌───────────────┘    │
              ▼                    ▼
┌──────────────────────┐  ┌────────────────────────────────┐
│ RESPUESTA            │  │ SERVICE → REPOSITORY           │
│ Solicita actualizar  │  │                                │
│ la revisión previa.  │  │ Registra el lote con:          │
└──────────────────────┘  │ • Perfil y versión utilizados. │
                          │ • Configuración efectiva.      │
                          │ • Valores resueltos.            │
                          │ • Carpetas y archivos.          │
                          └───────────────┬────────────────┘
                                          ▼
┌──────────────────────────────────────────────────────────┐
│ SERVICE → CONTROLLER → RESPUESTA                         │
│                                                          │
│ Lote preparado con configuración identificada.          │
│ Puede continuar la recepción y ejecución documental.   │
│                                                          │
│ Un cambio futuro del perfil no altera este lote.        │
└──────────────────────────────────────────────────────────┘
```

```text
RESPONSABILIDAD DE CADA CAPA
===========================

CLIENTE
  Presenta formularios, recoge selecciones y muestra resultados.
       │
       ▼
CONTROLLER
  Recibe solicitudes y obtiene el contexto autenticado.
       │
       ▼
SERVICE
  Autoriza, interpreta reglas, valida y coordina operaciones.
       │
       ▼
REPOSITORY
  Consulta y guarda configuración mediante acceso a datos.
       │
       ▼
RESPUESTA
  Devuelve resultados, errores y estado al usuario.

Seleccionar o publicar un perfil no crea expedientes.
El perfil define las reglas que utilizará una importación.
```
