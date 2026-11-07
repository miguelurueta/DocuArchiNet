# DOC-111: recursos, destino y protección de solicitudes

Estado del 2026-10-27: **PASS local de componentes Framework y regresión DOC-110**, compilación completa correcta. Tareas 5.3–5.8 verificadas a nivel de componente; progreso **38/130**. No hubo escrituras en la base, concesiones de permisos, cargas ni cambios del perfil 24 durante este corte. Estos componentes aún requieren integración en la fachada operativa; no se afirma aceptación HTTP/E2E real.

## Nivel y gabinete sin administración implícita

`ImportacionCarpetasContextAdapter.ResolverDestino` obtiene nuevamente identidad/facultad y autoriza el nivel pedido en RA. El solicitante usa su propiedad del nivel o los permisos existentes del nivel compartido, siempre con estado vigente y pertenencia empresarial. La tabla de capacidades aplicada es:

| Acción interna | Permiso sobre nivel compartido |
|---|---|
| ABRIR, PREPARAR, CONFIRMAR | agregar_expediente y carga_archivo |
| CREAR_EXPEDIENTE | agregar_expediente |
| RECIBIR_ARCHIVO, ALMACENAR_DOCUMENTO | carga_archivo |

Las acciones no enumeradas se rechazan; los comandos de consulta/seguimiento se integrarán con su autorización específica en sus tareas. No se inventan ni deducen capacidades de una acción de creación a partir de otra de carga. Las condiciones proceden de `ClassGaProducionDocumental.Activa_agregar_expediente_producion` y `Solicitar_agregar_documento_a_carpeta_expediente`.

`AutorizarRecursos` reconstruye ese destino y compara solicitante, empresa, ruta y sello con la referencia persistida. La entrada operativa estrecha `PerfilImportacionCatalogAdapter.AutorizarGabineteImportacion` comparte con el camino administrativo la relación RA→DA y la comprobación recíproca del usuario, la huella del origen DA y el predicado de ALMACENA_IMAGEN por usuario/grupo. Exige además INVENTARIO_DOCUMENTAL, APLICA_TRD y ASIGNA_UNIDAD, tal como hace la carga documental existente. Solo consulta metadatos/permisos; no almacena documentos.

No se construye ContextoPerfilImportacion con PuedeAdministrar=true para importar. La lectura operativa general de catálogos de 6.3 continúa pendiente; esta entrada nueva cubre únicamente autorización del gabinete. El acceso administrativo anterior mantiene sus condiciones y pasa la regresión existente.

## Ruta y sello

La ruta visible procede de los nombres de la cadena real nivel→ancestros; no es una identidad ni un destino recibido del cliente. Cada nodo debe existir, estar activo y pertenecer a la empresa comprobada. Un ciclo, referencia ausente o cadena de más de 128 nodos se rechaza.

El sello SHA-256, esquema 1, vincula empresa, huella de contexto físico RA y nodos ordenados con ID, padre, nombre, propietario y estado. Así, dos rutas con los mismos rótulos después de mover el nivel no comparten sello. El origen se consulta en RA y solo se incorpora al hash: no se devuelve servidor, base o conexión en la ruta. El nivel no se sustituye al cambiar selección en otra pestaña.

Las pruebas locales cubren nivel no autorizado, capacidades de creación/carga separadas, revocación del permiso importador antes de consultar recursos, padre ausente, ciclo, renombre, movimiento con rótulos iguales, propietario cambiado, cambio de origen RA/DA, gabinete distinto/sin permiso, relación RA/DA ausente y cada capacidad documental deshabilitada. Los datos de estas pruebas son dobles explícitos, no mutaciones sobre niveles o gabinetes reales.

## JSON, multipart y token operativo

`ImportacionCarpetasRequestGuard.vb` usa un token aleatorio de 32 bytes con claves de sesión propias y cabecera `X-Importacion-Carpetas-CSRF`. Lo vincula a sesión, identidad, empresa y propósito DOC111_OPERACION, sin exigir administración ni vincularlo a un único nivel que invalide otras pestañas. Reutiliza solamente la comparación criptográfica de tokens de DOC-110. El token administrativo no reemplaza al operativo.

El guard comprueba que el contexto servidor corresponda a la sesión de Gestión, permiso operativo verdadero, POST, token, origen y tipo de contenido. El dispatcher futuro debe obtener el contexto fresco mediante el adaptador en cada solicitud; el guard no convierte un contexto antiguo en una nueva concesión.

Para JSON vuelve a leer el cuerpo con límite de bytes, UTF-8 estricto y el codec de forma cerrada del sobre ASMX `solicitud`. Rechaza propiedades desconocidas, duplicadas, tipos incorrectos, ausencia de solicitud y bytes inválidos. Conserva la posición original del stream. El máximo efectivo respeta tanto el cupo de manifiesto como el límite de representación del codec administrativo que se reutiliza, sin ampliar sus reglas.

Para multipart comprueba autoridad/token/cabeceras antes de acceder a Form o Files. Solo admite loteId, archivoId y claveIntento, cada uno exactamente una vez, y un archivo. Los IDs deben ser canónicos y la clave de intento acotada. La envoltura usa como cupo de metadatos MaximoBytesManifiesto y respeta MaximoBytesArchivo sin desbordar la suma. No usa el nombre recibido como ruta y no crea staging. La lectura/validación real del contenido y su persistencia pertenecen al bloque 8. ASP.NET/IIS puede recibir o almacenar temporalmente datos antes de invocar el código; esta entrega no modifica sus límites globales ni afirma controlar ese procesamiento previo.

LIMITE_EXCEDIDO identifica un rechazo por capacidad; su traducción HTTP 413 se integrará junto con la fachada operativa. Los tests actuales verifican códigos de excepción de los componentes, no respuestas de un sitio desplegado.

## Pruebas y límites

El arnés existente verifica JSON correcto, token estable entre pestañas, token ausente/inválido/de otra sesión/administrativo, método/MIME/charset/origen incorrectos, contexto cambiado, JSON desconocido/duplicado/inválido, política ausente y límites antes de consumir cuerpo. Construye un multipart real en memoria y prueba caso correcto, campos duplicados/ajenos, archivo ausente, exceso de archivo/envoltura y boundary duplicado. Comprueba que un multipart sin token no se analice antes del rechazo.

La matriz local cubre las cuatro combinaciones importar/administrar: importar sin administrar puede usar recursos y guard operativos; solo administrar no obtiene importación; ambos conservan sus vías; ninguno queda denegado. Los casos administrativos anteriores siguen ejecutándose. No hay login, revocación real de permiso ni solicitud HTTP a un servidor; 5.9 y las verificaciones de integración correspondientes siguen pendientes.

Comandos: `node --test tests/doc110-perfil-importacion-behavior.test.cjs` y Build Debug de `GestionDocumental-Docuarchi.net.vbproj` con DeployOnBuild=false, salida `.tmp/doc111-build`: PASS. No se reemplazó el binario de la aplicación activa.
